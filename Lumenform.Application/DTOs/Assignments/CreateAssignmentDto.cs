namespace Lumenform.Application.DTOs.Assignments;

public record CreateAssignmentDto(
    string Title,
    string Content,
    Guid CohortId,
    List<string>? Tasks,
    DateTime? DueDate,
    bool SubmissionRequired
);