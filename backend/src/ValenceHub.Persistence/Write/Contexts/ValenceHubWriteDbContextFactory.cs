using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace ValenceHub.Persistence.Write.Contexts;

public sealed class ValenceHubWriteDbContextFactory : IDesignTimeDbContextFactory<ValenceHubWriteDbContext>
{
    public ValenceHubWriteDbContext CreateDbContext(string[] args)
    {
        var connectionString = ReadWriteConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<ValenceHubWriteDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ValenceHubWriteDbContext(optionsBuilder.Options);
    }

    private static string ReadWriteConnectionString()
    {
        var settingsPath = GetAppSettingsPath();
        using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));

        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) ||
            !connectionStrings.TryGetProperty("WriteDb", out var writeDbElement))
        {
            throw new InvalidOperationException("Connection string 'WriteDb' was not found in appsettings.json.");
        }

        return writeDbElement.GetString()
            ?? throw new InvalidOperationException("Connection string 'WriteDb' is null.");
    }

    private static string GetAppSettingsPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directPath = Path.Combine(currentDirectory, "ValenceHub.Api", "appsettings.json");
        if (File.Exists(directPath))
            return directPath;

        var parentPath = Path.Combine(currentDirectory, "..", "ValenceHub.Api", "appsettings.json");
        if (File.Exists(parentPath))
            return parentPath;

        throw new FileNotFoundException("Could not locate ValenceHub.Api/appsettings.json for design-time DbContext creation.");
    }
}
