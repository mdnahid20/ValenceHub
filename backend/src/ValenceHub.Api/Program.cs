using Microsoft.EntityFrameworkCore;
using ValenceHub.Persistence.Contexts;
using ValenceHub.Application;
using ValenceHub.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ValenceHubDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplication(); 
builder.Services.AddPersistence(builder.Configuration);

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

app.UseAuthorization();
app.MapControllers();
app.Run();
