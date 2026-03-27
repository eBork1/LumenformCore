using Lumenform.Application.Repositories;
using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumenform.Infrastructure.Persistence.Repositories;

public class CohortRepository : ICohortRepository
{
    private readonly ApplicationDbContext _context;

    public CohortRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cohort?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Cohorts
            .Include(c => c.Memberships)
            .Include(c => c.Assignments)
            .Include(c => c.Events)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
    
    public async Task<Cohort?> GetByIdForUserAsync(
        Guid cohortId,
        Guid userId,
        CancellationToken ct = default) 
    {
        return await _context.Cohorts
            .Include(c => c.Memberships)
            .Include(c => c.Assignments)
            .Include(c => c.Events)
            .SingleOrDefaultAsync(cohort =>
                    cohort.Id == cohortId &&
                    (cohort.CreatedByUserId == userId ||
                     _context.CohortMemberships.Any(m => m.CohortId == cohort.Id && m.UserId == userId)),
                ct);
    }

    public async Task<IEnumerable<Cohort>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cohorts
            .Include(c => c.Memberships)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Cohort>> GetByCreatorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Cohorts
            .Where(c => c.CreatedByUserId == userId)
            .Include(c => c.Memberships)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Cohort cohort, CancellationToken cancellationToken = default)
    {
        await _context.Cohorts.AddAsync(cohort, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Cohort cohort, CancellationToken cancellationToken = default)
    {
        //_context.Cohorts.Update(cohort);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Cohort cohort, CancellationToken cancellationToken = default)
    {
        _context.Cohorts.Remove(cohort);
        await _context.SaveChangesAsync(cancellationToken);
    }
}