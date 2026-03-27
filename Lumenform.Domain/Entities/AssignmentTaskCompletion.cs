using Lumenform.Domain.Exceptions;

namespace Lumenform.Domain.Entities;

public class AssignmentTaskCompletion : Entity
{
    public Guid AssignmentTaskId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CompletedAt { get; private set; }
    
    // Navigation
    public AssignmentTask AssignmentTask { get; private set; } = null!;

    // Private constructor for EF Core
    private AssignmentTaskCompletion() { }

    // Factory method
    public static AssignmentTaskCompletion Create(Guid assignmentTaskId, Guid userId)
    {
        return new AssignmentTaskCompletion
        {
            AssignmentTaskId = assignmentTaskId,
            UserId = userId,
            CompletedAt = DateTime.UtcNow
        };
    }
}