using Lumenform.Domain.Entities;

namespace Lumenform.Application.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assignment>> GetByCohortAsync(Guid cohortId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Assignment>> GetTemplatesByCreatorAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default);
}