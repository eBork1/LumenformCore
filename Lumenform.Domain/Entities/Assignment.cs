using Lumenform.Domain.Exceptions;
using System.Linq;

namespace Lumenform.Domain.Entities;

public class Assignment : Entity
{
    public string Title { get; private set; }
    public string Content { get; private set; }  // Rich text HTML
    public Guid? CohortId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public bool IsTemplate { get; private set; }
    public DateTime? DueDate { get; private set; } // Can be used for "IsMandatory"
    
    // Navigation properties
    public Cohort? Cohort { get; private set; }
    
    private readonly List<AssignmentTask> _tasks = new();
    public IReadOnlyCollection<AssignmentTask> Tasks => _tasks.AsReadOnly();
    
    private readonly List<AssignmentSubmission> _submissions = new();
    public IReadOnlyCollection<AssignmentSubmission> Submissions => _submissions.AsReadOnly();

    // Private constructor for EF Core
    private Assignment() 
    {
        Title = string.Empty;
        Content = string.Empty;
    }

    // Factory method - Create cohort assignment
    public static Assignment Create(
        string title,
        string content,
        Guid cohortId,
        Guid createdByUserId,
        DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Assignment title cannot be empty");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Assignment content cannot be empty");

        return new Assignment
        {
            Title = title,
            Content = content,
            CohortId = cohortId,
            CreatedByUserId = createdByUserId,
            IsTemplate = false,
            DueDate = dueDate
        };
    }

    // Factory method - Create template
    public static Assignment CreateTemplate(
        string title,
        string content,
        Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Assignment title cannot be empty");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Assignment content cannot be empty");

        return new Assignment
        {
            Title = title,
            Content = content,
            CohortId = null,
            CreatedByUserId = createdByUserId,
            IsTemplate = true,
            DueDate = null
        };
    }

    // Clone template into cohort assignment
    public Assignment CloneToCohort(Guid cohortId, DateTime? dueDate = null)
    {
        if (!IsTemplate)
            throw new DomainException("Can only clone templates");

        var clonedAssignment = Create(Title, Content, cohortId, CreatedByUserId, dueDate);
        
        // Clone tasks
        foreach (var task in _tasks.OrderBy(t => t.Order))
        {
            clonedAssignment.AddTask(task.Description);
        }

        return clonedAssignment;
    }

    // Update content
    public void UpdateContent(string title, string content)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Assignment title cannot be empty");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Assignment content cannot be empty");

        Title = title;
        Content = content;
        UpdateTimestamp();
    }

    // Update due date
    public void UpdateDueDate(DateTime? dueDate)
    {
        if (IsTemplate)
            throw new DomainException("Templates cannot have due dates");

        DueDate = dueDate;
        UpdateTimestamp();
    }

    // Task management
    public AssignmentTask AddTask(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Task description cannot be empty");

        var order = _tasks.Any() ? _tasks.Max(t => t.Order) + 1 : 1;
        var task = AssignmentTask.Create(Id, description, order);
        _tasks.Add(task);
        UpdateTimestamp();
        
        return task;
    }

    public void RemoveTask(Guid taskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
            throw new DomainException("Task not found");

        _tasks.Remove(task);
        
        // Reorder remaining tasks
        var remainingTasks = _tasks.OrderBy(t => t.Order).ToList();
        for (int i = 0; i < remainingTasks.Count; i++)
        {
            remainingTasks[i].UpdateOrder(i + 1);
        }
        
        UpdateTimestamp();
    }

    public void ReorderTasks(List<Guid> taskIds)
    {
        if (taskIds.Count != _tasks.Count)
            throw new DomainException("Must provide all task IDs for reordering");

        for (int i = 0; i < taskIds.Count; i++)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == taskIds[i]);
            if (task == null)
                throw new DomainException($"Task with ID {taskIds[i]} not found");
            
            task.UpdateOrder(i + 1);
        }
        
        UpdateTimestamp();
    }

    // Get submission by user
    public AssignmentSubmission? GetSubmissionByUser(Guid userId)
    {
        return _submissions.FirstOrDefault(s => s.UserId == userId);
    }

    // Check if user has submitted
    public bool HasUserSubmitted(Guid userId)
    {
        return _submissions.Any(s => s.UserId == userId);
    }
}