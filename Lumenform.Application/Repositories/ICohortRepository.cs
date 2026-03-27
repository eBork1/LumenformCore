using Lumenform.Domain.Entities;

namespace Lumenform.Application.Repositories;

public interface ICohortRepository
{
    Task<Cohort?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cohort?> GetByIdForUserAsync(Guid cohortId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cohort>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cohort>> GetByCreatorAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Cohort cohort, CancellationToken cancellationToken = default);
    Task UpdateAsync(Cohort cohort, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cohort cohort, CancellationToken cancellationToken = default);
}