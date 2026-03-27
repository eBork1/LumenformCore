using Lumenform.Domain.Exceptions;

namespace Lumenform.Domain.Entities;

public class AssignmentTask : Entity
{
    public Guid AssignmentId { get; private set; }
    public string Description { get; private set; }
    public int Order { get; private set; }
    
    // Navigation
    public Assignment Assignment { get; private set; } = null!;
    
    private readonly List<AssignmentTaskCompletion> _completions = new();
    public IReadOnlyCollection<AssignmentTaskCompletion> Completions => _completions.AsReadOnly();

    // Private constructor for EF Core
    private AssignmentTask()
    {
        Description = string.Empty;
    }

    // Factory method
    public static AssignmentTask Create(Guid assignmentId, string description, int order)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Task description cannot be empty");

        return new AssignmentTask
        {
            AssignmentId = assignmentId,
            Description = description,
            Order = order
        };
    }

    // Update methods
    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Task description cannot be empty");

        Description = description;
        UpdateTimestamp();
    }

    public void UpdateOrder(int order)
    {
        Order = order;
        UpdateTimestamp();
    }
}