using System.Security.Cryptography;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Application.Abstractions.Services;
using ValenceHub.Application.Abstractions.Transactions;
using ValenceHub.Application.Features.Auth.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;
using ValenceHub.Infrastructure.Attributes;

namespace ValenceHub.Persistence.Write.Services;

[AutoRegister(ServiceLifetime.Scoped, ServiceType = typeof(IOtpService))]
public sealed class OtpService : IOtpService
{
    private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(30);
    private const short MaxResendCount = 3;
    private const int MaxIpAddressLength = 64;
    private const int MaxUserAgentLength = 512;

    private readonly IOtpCodeRepository _otpCodeRepository;
    private readonly IOtpSender _otpSender;
    private readonly IDateTimeOffsetProvider _clock;
    private readonly IUnitOfWork _unitOfWork;

    public OtpService(
        IOtpCodeRepository otpCodeRepository,
        IOtpSender otpSender,
        IDateTimeOffsetProvider clock,
        IUnitOfWork unitOfWork)
    {
        _otpCodeRepository = Guard.Against.Null(otpCodeRepository);
        _otpSender = Guard.Against.Null(otpSender);
        _clock = Guard.Against.Null(clock);
        _unitOfWork = Guard.Against.Null(unitOfWork);
    }

    public async Task<Result> IssueAsync(
        Guid userId,
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        var utcNow = _clock.UtcNow;
        var latest = await _otpCodeRepository.GetLatestByTargetAndPurposeAsync(targetType, targetValue, purpose, cancellationToken);

        if (latest is not null && latest.LastSentAtUtc.Add(ResendCooldown) > utcNow)
        {
            return Result.Failure(
                Error.Validation(
                    "Otp.Send.Throttled",
                    $"OTP was sent recently. Try again after {latest.LastSentAtUtc.Add(ResendCooldown):O}."));
        }

        var code = GenerateOtp();
        var otpCode = OtpCode.Create(
            userId,
            targetType,
            targetValue,
            purpose,
            ComputeHash(code),
            utcNow);

        if (otpCode.IsFailure)
            return ToFailure(otpCode.Error);

        await _otpCodeRepository.AddAsync(otpCode.Value, cancellationToken);
        await _otpSender.SendAsync(targetType, targetValue, code, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ResendAsync(
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default)
    {
        var latest = await _otpCodeRepository.GetLatestByTargetAndPurposeAsync(targetType, targetValue, purpose, cancellationToken);
        if (latest is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Otp.NotFound",
                    "No OTP request was found for the provided target and purpose."));
        }

        var utcNow = _clock.UtcNow;
        if (latest.LastSentAtUtc.Add(ResendCooldown) > utcNow)
        {
            return Result.Failure(
                Error.Validation(
                    "Otp.Resend.Throttled",
                    $"OTP resend is not allowed until {latest.LastSentAtUtc.Add(ResendCooldown):O}."));
        }

        var code = GenerateOtp();
        var resendError = latest.Resend(
            ComputeHash(code),
            utcNow,
            MaxResendCount);

        if (resendError is not null)
            return ToFailure(resendError);

        _otpCodeRepository.Update(latest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _otpSender.SendAsync(targetType, targetValue, code, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<OtpVerificationServiceResult>> VerifyAsync(
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        string code,
        CancellationToken cancellationToken = default)
    {
        var latest = await _otpCodeRepository.GetLatestByTargetAndPurposeAsync(targetType, targetValue, purpose, cancellationToken);
        if (latest is null)
        {
            return Result<OtpVerificationServiceResult>.Failure(
                Error.Validation(
                    "Otp.Code.Invalid",
                    "The OTP code is invalid or has expired."));
        }

        var verificationError = latest.Verify(ComputeHash(code), _clock.UtcNow);
        if (verificationError is not null)
            return ToFailure<OtpVerificationServiceResult>(verificationError);

        var token = CreateVerificationToken();
        var tokenError = latest.IssueVerificationToken(
                ComputeHash(token),
                _clock.UtcNow);

        if (tokenError is not null)
             return ToFailure<OtpVerificationServiceResult>(tokenError);

        _otpCodeRepository.Update(latest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OtpVerificationServiceResult>.Success(
            new OtpVerificationServiceResult(latest.UserId!.Value, token));
    }
    public async Task<Result<Guid>> ConsumeVerificationTokenAsync(
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        string VerificationToken,
        CancellationToken cancellationToken = default)
    {
        var latest = await _otpCodeRepository.GetLatestByTargetAndPurposeAsync(
            targetType,
            targetValue,
            purpose,
            cancellationToken);

        if (latest is null || latest.UserId is null)
        {
            return Result<Guid>.Failure(
                Error.Validation(
                    "Otp.VerificationToken.Invalid",
                    "Invalid or expired verification token."));
        }

        var credential = VerificationToken.Trim();
        var utcNow = _clock.UtcNow;

        var error = latest.ConsumeVerificationToken(ComputeHash(credential), utcNow);

        if (error is not null)
            return ToFailure<Guid>(error);

        _otpCodeRepository.Update(latest);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<Guid>.Success(latest.UserId.Value);
    }

    private static bool IsOtpCode(string value)
        => value.Length == 6 && value.All(char.IsDigit);

    private static string GenerateOtp()
        => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    private static string CreateVerificationToken()
        => Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

    private static string ComputeHash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value.Trim())));

    private static string Base64UrlEncode(byte[] value)
        => Convert.ToBase64String(value)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');   
    //TODO : Make it shareable that any layer can use it just by call file name
    private static Result ToFailure(ValenceHub.Domain.Common.Results.Error error)
        => Result.Failure(new Error(error.Code, error.Message, ErrorType.Validation));

    private static Result<T> ToFailure<T>(ValenceHub.Domain.Common.Results.Error error)
        => Result<T>.Failure(new Error(error.Code, error.Message, ErrorType.Validation));
}
