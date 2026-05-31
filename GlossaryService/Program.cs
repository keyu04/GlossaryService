using System.Threading.RateLimiting;
using Asp.Versioning;
using AuthMicroService.Common.Logger;
using GlossaryService.Common.Setting;
using GlossaryService.Repository.Implementations;
using GlossaryService.Repository.Interfaces;
using GlossaryService.Services.Implementations;
using GlossaryService.Services.Interfaces;
using JobPortalAPI.Middlewares;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

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
.AddMvc();

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
builder.Services.AddSwaggerGen();

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
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();