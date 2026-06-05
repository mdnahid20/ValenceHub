using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;

namespace ValenceHub.Application.Abstractions.Repositories;

public interface IOtpCodeRepository : IRepository<OtpCode>
{
    Task<OtpCode?> GetLatestByTargetAndPurposeAsync(
        CommunicationChannel targetType, 
        string targetValue,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default);
}
