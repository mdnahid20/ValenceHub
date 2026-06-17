using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ValenceHub.Application;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Persistence;
using ValenceHub.Infrastructure;
using ValenceHub.Api.BackgroundServices;
using ValenceHub.Api.Logging;

var builder = WebApplication.CreateBuilder(args);
var fileLoggerOptions = builder.Configuration
    .GetSection(FileLoggerOptions.SectionName)
    .Get<FileLoggerOptions>() ?? new FileLoggerOptions();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddApplication(); 
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<OutboxProcessingBackgroundService>();
builder.Services.AddHttpContextAccessor();
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

var jwtSection = builder.Configuration.GetSection("Authentication:Jwt");
var signingKey = jwtSection["SigningKey"]?.Trim();
if (string.IsNullOrEmpty(signingKey) || signingKey.Length < 32)
{
    throw new InvalidOperationException(
        "Configuration 'Authentication:Jwt:SigningKey' is required and must be at least 32 characters.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidIssuer = jwtSection["Issuer"] ?? "ValenceHub",
            ValidAudience = jwtSection["Audience"] ?? "ValenceHub",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

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
