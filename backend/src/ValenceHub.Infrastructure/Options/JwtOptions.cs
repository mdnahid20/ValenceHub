namespace ValenceHub.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    public string Issuer { get; set; } = "ValenceHub";
    public string Audience { get; set; } = "ValenceHub";
    
    //Later You have change it dont push code with github.
    public string SigningKey { get; set; } = "valencehub-dev-signing-key-change-this-at-least-32-chars";
    public int AccessTokenLifetimeMinutes { get; set; } = 15;
}
