using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Infrastructure.Attributes;
using ValenceHub.Infrastructure.Options;

namespace ValenceHub.Infrastructure.Services;

[AutoRegister(ServiceLifetime.Singleton)]
public sealed class JwtProvider : IJwtProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly JwtOptions _options;
    private readonly byte[] _signingKey;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.SigningKey) || _options.SigningKey.Trim().Length < 32)
        {
            throw new InvalidOperationException(
                $"Configuration value '{JwtOptions.SectionName}:SigningKey' must be at least 32 characters.");
        }

        _signingKey = Encoding.UTF8.GetBytes(_options.SigningKey.Trim());
    }

    public DateTimeOffset GetAccessTokenExpiresAt(DateTimeOffset utcNow)
        => utcNow.AddMinutes(_options.AccessTokenLifetimeMinutes <= 0 ? 15 : _options.AccessTokenLifetimeMinutes);

    public string GenerateAccessToken(Guid userId, DateTimeOffset issuedAtUtc, DateTimeOffset expiresAtUtc)
    {
        var header = Base64UrlEncode(
            JsonSerializer.SerializeToUtf8Bytes(
                new Dictionary<string, object>
                {
                    ["alg"] = "HS256",
                    ["typ"] = "JWT"
                },
                SerializerOptions));

        var payload = new Dictionary<string, object>
        {
            ["sub"] = userId.ToString(),
            ["iss"] = _options.Issuer,
            ["aud"] = _options.Audience,
            ["iat"] = issuedAtUtc.ToUnixTimeSeconds(),
            ["exp"] = expiresAtUtc.ToUnixTimeSeconds()
        };

        var payloadSegment = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload, SerializerOptions));
        var signingInput = $"{header}.{payloadSegment}";
        var signature = ComputeSignature(signingInput);

        return $"{signingInput}.{signature}";
    }

    private string ComputeSignature(string signingInput)
    {
        using var hmac = new HMACSHA256(_signingKey);
        var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput));
        return Base64UrlEncode(signature);
    }

    private static string Base64UrlEncode(byte[] value)
        => Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
