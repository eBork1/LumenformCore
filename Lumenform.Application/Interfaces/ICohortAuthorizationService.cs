namespace Lumenform.Application.Interfaces;

public interface ICohortAuthorizationService
{
    Task<bool> IsOwner(Guid cohortId, Guid userId, CancellationToken ct = default);
    Task<bool> IsCoordinator(Guid cohortId, Guid userId, CancellationToken ct = default);
    Task<bool> IsMember(Guid cohortId, Guid userId, CancellationToken ct = default);
}