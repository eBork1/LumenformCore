namespace Lumenform.Application.DTOs.Assignments;

public record AssignmentDto(
    Guid Id,
    string Title,
    string Content,
    Guid? CohortId,
    DateTime? DueDate,
    bool IsTemplate,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    int TaskCount,
    bool SubmissionRequired
);