using ValenceHub.Application.Abstractions.Results;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Features.Auth.Abstractions;

public interface IOtpService
{
    Task<Result> IssueAsync(
        Guid userId,
        CommunicationChannel targetType,
        string targetValue, 
        OtpPurpose purpose,
        CancellationToken cancellationToken = default);

    Task<Result> ResendAsync(
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default);

    Task<Result<OtpVerificationServiceResult>> VerifyAsync(
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        string code,
        CancellationToken cancellationToken = default);

    Task<Result<Guid>> ConsumeVerificationTokenAsync(
        CommunicationChannel targetType,
        string targetValue, 
        OtpPurpose purpose,
        string otpOrVerificationToken,
        CancellationToken cancellationToken = default);
}
