using Lumenform.Application.Repositories;
using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumenform.Infrastructure.Persistence.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .Include(a => a.Tasks)
            .Include(a => a.Submissions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetByCohortAsync(Guid cohortId, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .Where(a => a.CohortId == cohortId && !a.IsTemplate)
            .Include(a => a.Tasks)
            .OrderBy(a => a.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Assignment>> GetTemplatesByCreatorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .Where(a => a.IsTemplate && a.CreatedByUserId == userId)
            .Include(a => a.Tasks)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await _context.Assignments.AddAsync(assignment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync(cancellationToken);
    }
}