using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using AuthMicroService.Common.Logger;
using GlossaryService.Common.Setting;
using GlossaryService.Common.Settings;
using GlossaryService.Repository.Implementations;
using GlossaryService.Repository.Interfaces;
using GlossaryService.Services.Implementations;
using GlossaryService.Services.Interfaces;
using JobPortalAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Settings.JwtSecreteKey = builder.Configuration.GetValue<string>("Jwt:Key");
Settings.JwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
Settings.JwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Settings.JwtIssuer,
            ValidAudience = Settings.JwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.JwtSecreteKey!))
        };
    });

// Database
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IKeywordRepository, KeywordRepository>();

// Services
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IKeywordService, KeywordService>();

// ── API Versioning ────────────────────────────────────────────────
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;  // ← no version = v1
    options.ReportApiVersions = true;  // ← response header shows supported versions
})
.AddMvc()
.AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

// ── Rate Limiting ─────────────────────────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    // ── Global fixed window policy ───────────────────────────────
    options.AddFixedWindowLimiter("general", config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.PermitLimit = 100;   // ← 100 requests per minute
        config.QueueLimit = 0;     // ← no queue, reject immediately
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // ── Strict policy for write operations ───────────────────────
    options.AddFixedWindowLimiter("strict", config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.PermitLimit = 20;    // ← 20 requests per minute
        config.QueueLimit = 0;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // ── Custom 429 response ──────────────────────────────────────
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsync("""
        {
            "success": false,
            "statusCode": 429,
            "message": "Too many requests. Please slow down and try again in a minute.",
            "data": null,
            "details": null
        }
        """, cancellationToken);
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Glossary API",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid JWT token in the text input below."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

builder.Logging.ClearProviders();

builder.Logging.AddConsole(options =>
{
    options.FormatterName = CustomConsoleLogger.FormatterName;
});

builder.Logging.AddConsoleFormatter<CustomConsoleLogger, ConsoleFormatterOptions>();

// Optional: also log to Debug window (Visual Studio)
builder.Logging.AddDebug();

var app = builder.Build();

// ── Auto-run migrations on startup ───────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();   // ← runs pending migrations automatically
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseRateLimiter();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Glossary API v1");
        c.RoutePrefix = string.Empty;
    });
}
app.UseHttpsRedirection();
app.MapControllers();
app.Run();