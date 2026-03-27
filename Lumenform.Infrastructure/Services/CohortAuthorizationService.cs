using Lumenform.Application.Interfaces;
using Lumenform.Application.Repositories;
using Lumenform.Domain.Enums;

namespace Lumenform.Infrastructure.Services;

public class CohortAuthorizationService : ICohortAuthorizationService
{
    private readonly ICohortRepository _cohortRepo;

    public CohortAuthorizationService(ICohortRepository cohortRepo)
    {
        _cohortRepo = cohortRepo;
    }

    public async Task<bool> IsOwner(Guid cohortId, Guid userId, CancellationToken ct = default)
    {
        var cohort = await _cohortRepo.GetByIdAsync(cohortId, ct);
        return cohort?.CreatedByUserId == userId;
    }

    public async Task<bool> IsCoordinator(Guid cohortId, Guid userId, CancellationToken ct = default)
    {
        var cohort = await _cohortRepo.GetByIdForUserAsync(cohortId, userId, ct);
        
        if (cohort == null)
            return false;

        return cohort.Memberships.Any(m => 
            m.UserId == userId && 
            m.Role == CohortRole.Coordinator);
    }

    public async Task<bool> IsMember(Guid cohortId, Guid userId, CancellationToken ct = default)
    {
        var cohort = await _cohortRepo.GetByIdForUserAsync(cohortId, userId, ct);
        return cohort != null;
    }
}