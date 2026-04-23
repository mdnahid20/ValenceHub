using Microsoft.EntityFrameworkCore;
using ValenceHub.Persistence.Read.Contexts;
using ValenceHub.Persistence.Write.Contexts;
using ValenceHub.Application;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Persistence;
using ValenceHub.Api.BackgroundServices;
using ValenceHub.Api.Logging;

var builder = WebApplication.CreateBuilder(args);
var fileLoggerOptions = builder.Configuration
    .GetSection(FileLoggerOptions.SectionName)
    .Get<FileLoggerOptions>() ?? new FileLoggerOptions();

builder.Services.AddApplication(); 
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddHostedService<OutboxProcessingBackgroundService>();
builder.Logging.Services.AddSingleton<ILoggerProvider>(sp =>
    new FileLoggerProvider(
        fileLoggerOptions,
        builder.Environment.ContentRootPath,
        sp.GetRequiredService<IDateTimeOffsetProvider>()));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ValenceHub API v1");
        c.RoutePrefix = string.Empty; // access via http://localhost:5000
    });
}

app.Run();
