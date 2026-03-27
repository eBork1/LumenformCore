namespace Lumenform.Application.DTOs.Assignments;

public record CreateAssignmentDto(
    string Title,
    string Content,
    Guid CohortId,
    DateTime? DueDate
);