using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ValenceHub.Persistence.Read.Contexts;

public sealed class ReadDbContextFactory : IDesignTimeDbContextFactory<ReadDbContext>
{
    public ReadDbContext CreateDbContext(string[] args)
    {
        var connectionString = ReadReadConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<ReadDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ReadDbContext(optionsBuilder.Options);
    }

    private static string ReadReadConnectionString()
    {
        var settingsPath = GetAppSettingsPath();
        using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));

        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) ||
            !connectionStrings.TryGetProperty("ReadDb", out var readDbElement))
        {
            throw new InvalidOperationException("Connection string 'ReadDb' was not found in appsettings.json.");
        }

        return readDbElement.GetString()
            ?? throw new InvalidOperationException("Connection string 'ReadDb' is null.");
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
