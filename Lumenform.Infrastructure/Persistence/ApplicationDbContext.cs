using Lumenform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lumenform.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cohort> Cohorts => Set<Cohort>();
    public DbSet<CohortMembership> CohortMemberships => Set<CohortMembership>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<AssignmentTask> AssignmentTasks => Set<AssignmentTask>();
    public DbSet<AssignmentTaskCompletion> AssignmentTaskCompletions => Set<AssignmentTaskCompletion>();
    public DbSet<AssignmentSubmission> AssignmentSubmissions => Set<AssignmentSubmission>();
    public DbSet<CohortEvent> CohortEvents => Set<CohortEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}