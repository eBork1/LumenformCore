using Lumenform.Domain.Exceptions;

namespace Lumenform.Domain.Entities;

public class AssignmentSubmission : Entity
{
    public Guid AssignmentId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }  // Rich text response
    public DateTime SubmittedAt { get; private set; }
    public string? CoordinatorFeedback { get; private set; }
    public DateTime? FeedbackProvidedAt { get; private set; }
    public Guid? FeedbackProvidedBy { get; private set; }  // Which coordinator gave feedback
    
    // Navigation
    public Assignment Assignment { get; private set; } = null!;

    // Private constructor for EF Core
    private AssignmentSubmission()
    {
        Content = string.Empty;
    }

    // Factory method
    public static AssignmentSubmission Create(Guid assignmentId, Guid userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Submission content cannot be empty");

        return new AssignmentSubmission
        {
            AssignmentId = assignmentId,
            UserId = userId,
            Content = content,
            SubmittedAt = DateTime.UtcNow
        };
    }

    // Update submission content
    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Submission content cannot be empty");

        Content = content;
        SubmittedAt = DateTime.UtcNow;  // Reset submission time on edit
        UpdateTimestamp();
    }

    // Coordinator provides feedback
    public void ProvideFeedback(string feedback, Guid coordinatorUserId)
    {
        if (string.IsNullOrWhiteSpace(feedback))
            throw new DomainException("Feedback cannot be empty");

        CoordinatorFeedback = feedback;
        FeedbackProvidedAt = DateTime.UtcNow;
        FeedbackProvidedBy = coordinatorUserId;
        UpdateTimestamp();
    }

    // Clear feedback
    public void ClearFeedback()
    {
        CoordinatorFeedback = null;
        FeedbackProvidedAt = null;
        FeedbackProvidedBy = null;
        UpdateTimestamp();
    }
}