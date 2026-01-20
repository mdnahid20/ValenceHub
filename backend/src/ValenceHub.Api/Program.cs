using Microsoft.EntityFrameworkCore;
using ValenceHub.Persistence.Contexts;
using ValenceHub.Api.Extensions;
using ValenceHub.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Centralized API registrations (controllers, swagger, CORS, filters)
builder.Services.AddApiServices(builder.Configuration);

// DbContext registration (keep your existing connection string key)
builder.Services.AddDbContext<ValenceHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

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

// Add authentication/authorization here when you wire JWT
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
