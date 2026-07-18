using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ValenceHub.Application.Abstractions.Repositories;
using ValenceHub.Domain.Common.Enums;
using ValenceHub.Domain.Otps;
using ValenceHub.Domain.Otps.Enums;
using ValenceHub.Infrastructure.Attributes;
using ValenceHub.Persistence.Write.Contexts;

namespace ValenceHub.Persistence.Write.Repositories;

[AutoRegister(ServiceLifetime.Scoped)]
public sealed class OtpCodeRepository : IRepository<OtpCode>, IOtpCodeRepository
{
    private readonly ValenceHubWriteDbContext _db;

    public OtpCodeRepository(ValenceHubWriteDbContext db)
    {
        _db = db;
    }

    public IQueryable<OtpCode> Query() => _db.OtpCodes.AsQueryable();

    public async Task<OtpCode?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _db.OtpCodes.FindAsync(new object[] { id }, cancellationToken) as OtpCode;

    public async Task<OtpCode?> SingleOrDefaultAsync(
        Expression<Func<OtpCode, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _db.OtpCodes.SingleOrDefaultAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<OtpCode>> ListAsync(CancellationToken cancellationToken = default)
        => await _db.OtpCodes.ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OtpCode>> ListAsync(
        Expression<Func<OtpCode, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _db.OtpCodes.Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(OtpCode entity, CancellationToken cancellationToken = default)
        => await _db.OtpCodes.AddAsync(entity, cancellationToken);

    public void Update(OtpCode entity) => _db.OtpCodes.Update(entity);

    public void Remove(OtpCode entity) => _db.OtpCodes.Remove(entity);

    //TODO : We Should do indexing by TargetType, TargetValue and Purpose for better performance
    public async Task<OtpCode?> GetLatestByTargetAndPurposeAsync(
        CommunicationChannel targetType,
        string targetValue,
        OtpPurpose purpose,
        CancellationToken cancellationToken = default)
        => await _db.OtpCodes
            .AsNoTracking()
            .Where(x =>
                x.TargetType == targetType &&
                x.Purpose == purpose &&
                x.TargetValue == targetValue)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
}
