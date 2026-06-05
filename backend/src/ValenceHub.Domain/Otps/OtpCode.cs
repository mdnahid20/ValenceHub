using System.Security.Cryptography;
using System.Text;
using ValenceHub.Domain.Abstractions;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Common.Results;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Domain.Otps;

public sealed class OtpCode : AggregateRoot<OtpCodeId, Guid>
{
    public const short MaxAttempts = 5;

    private OtpCode(
        OtpCodeId id,
        Guid? userId,
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        string codeHash,
        DateTimeOffset expiresAtUtc,
        DateTimeOffset createdAtUtc,
        DateTimeOffset lastSentAtUtc,
        short resendCount,
        short attempts,
        DateTimeOffset? usedAtUtc,
        string? verificationTokenHash,
        DateTimeOffset? verificationTokenExpiresAtUtc,
        DateTimeOffset? verificationTokenConsumedAtUtc)
        : base(id)
    {
        UserId = userId;
        TargetType = targetType;
        TargetValue = targetValue;
        Purpose = purpose;
        CodeHash = codeHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = createdAtUtc;
        LastSentAtUtc = lastSentAtUtc;
        ResendCount = resendCount;
        Attempts = attempts;
        UsedAtUtc = usedAtUtc;
        VerificationTokenHash = verificationTokenHash;
        VerificationTokenExpiresAtUtc = verificationTokenExpiresAtUtc;
        VerificationTokenConsumedAtUtc = verificationTokenConsumedAtUtc;
    }

    public Guid? UserId { get; private set; }
    public CommunicationChannel TargetType { get; private set; }
    public string TargetValue { get; private set; }
    public OtpPurpose Purpose { get; private set; }
    public string CodeHash { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? UsedAtUtc { get; private set; }
    public short Attempts { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public short ResendCount { get; private set; }
    public DateTimeOffset LastSentAtUtc { get; private set; }
    public string? VerificationTokenHash { get; private set; }
    public DateTimeOffset? VerificationTokenExpiresAtUtc { get; private set; }
    public DateTimeOffset? VerificationTokenConsumedAtUtc { get; private set; }

    public static Result<OtpCode> Create(
        Guid? userId,
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        string codeHash,
        DateTimeOffset now)
    {
        if (!Enum.IsDefined(typeof(CommunicationChannel), targetType))
            return Result<OtpCode>.Failure(OtpCodeErrors.TargetRequired);

        if (string.IsNullOrWhiteSpace(targetValue))
            return Result<OtpCode>.Failure(OtpCodeErrors.TargetValueRequired);

        if (string.IsNullOrWhiteSpace(codeHash))
            return Result<OtpCode>.Failure(OtpCodeErrors.CodeHashRequired);

        if (!Enum.IsDefined(typeof(OtpPurpose), purpose))
            return Result<OtpCode>.Failure(OtpCodeErrors.PurposeRequired);

        return Result<OtpCode>.Success(
            new OtpCode(
                OtpCodeId.New(),
                userId,
                targetType,
                targetValue,
                purpose,
                codeHash.Trim(),
                expiresAtUtc : now.Add(OtpPurposeExpiry.GetDuration(purpose)),
                createdAtUtc: now,
                resendCount: 0,
                attempts: 0,
                usedAtUtc: null,
                lastSentAtUtc: now,
                verificationTokenHash: null,
                verificationTokenExpiresAtUtc: null,
                verificationTokenConsumedAtUtc: null));
    }
    public bool IsExpired(DateTimeOffset utcNow) => ExpiresAtUtc <= utcNow;
    public bool IsUsed => UsedAtUtc is not null;
    public bool CanRetry => Attempts < MaxAttempts;

    public Error? Verify(string providedCodeHash, DateTimeOffset utcNow)
    {
        if (UsedAtUtc is not null)
            return OtpCodeErrors.CodeAlreadyUsed;

        if (ExpiresAtUtc <= utcNow)
            return OtpCodeErrors.CodeExpired;

        if (Attempts >= MaxAttempts)
            return OtpCodeErrors.AttemptsExceeded(MaxAttempts);

        if (!Matches(CodeHash, providedCodeHash))
        {
            Attempts++;

            return Attempts >= MaxAttempts
                ? OtpCodeErrors.AttemptsExceeded(MaxAttempts)
                : OtpCodeErrors.InvalidCode;
        }

        UsedAtUtc = utcNow;
        return null;
    }

    public Error? Resend(
        string newCodeHash,
        DateTimeOffset utcNow,
        short maxResends)
    {
        if (ResendCount >= maxResends)
            return OtpCodeErrors.ResendLimitExceeded(maxResends);

        if (IsUsed)
            return OtpCodeErrors.CodeAlreadyUsed;

        CodeHash = newCodeHash;
        ExpiresAtUtc = utcNow.Add(OtpPurposeExpiry.GetDuration(Purpose));
        Attempts = 0;
        UsedAtUtc = null;
        ResendCount++;
        LastSentAtUtc = utcNow;
        VerificationTokenHash = null;
        VerificationTokenExpiresAtUtc = null;
        VerificationTokenConsumedAtUtc = null;

        return null;
    }

    public Error? IssueVerificationToken(string verificationTokenHash, DateTimeOffset utcNow)
    {
        if (!IsUsed)
            return OtpCodeErrors.CodeIsNotUsed;

        if (string.IsNullOrWhiteSpace(verificationTokenHash))
            return OtpCodeErrors.VerificationTokenInvalid;

        VerificationTokenHash = verificationTokenHash;
        VerificationTokenExpiresAtUtc = utcNow.Add(OtpPurposeVerificationTokenExpiry.GetDuration(Purpose));
        VerificationTokenConsumedAtUtc = null;

        return null;
    }

    public Error? ConsumeVerificationToken(string verificationTokenHash, DateTimeOffset utcNow)
    {
        if (string.IsNullOrWhiteSpace(VerificationTokenHash))
        {
            return OtpCodeErrors.VerificationTokenInvalid;
        }

        if (VerificationTokenConsumedAtUtc is not null)
            return OtpCodeErrors.VerificationTokenAlreadyConsumed;

        if (VerificationTokenExpiresAtUtc is null || VerificationTokenExpiresAtUtc <= utcNow)
            return OtpCodeErrors.VerificationTokenExpired;

        if (!Matches(VerificationTokenHash, verificationTokenHash))
            return OtpCodeErrors.VerificationTokenInvalid;

        VerificationTokenConsumedAtUtc = utcNow;
        return null;
    }

    private static bool Matches(string left, string right)
    {
        var leftBytes = Encoding.UTF8.GetBytes(left);
        var rightBytes = Encoding.UTF8.GetBytes(right);

        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }
}
