using AuthMicroService.Common.Logger;
using GlossaryService.Common.Setting;
using GlossaryService.Repository.Implementations;
using GlossaryService.Repository.Interfaces;
using GlossaryService.Services.Implementations;
using GlossaryService.Services.Interfaces;
using JobPortalAPI.Middlewares;
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
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();