using Lumenform.Application.DTOs.Assignments;
using Lumenform.Domain.Entities;

namespace Lumenform.Application.Mappers;

public class AssignmentMappers
{
    public static AssignmentDto MapToAssignmentDto(Assignment assignment)
    {
        return new AssignmentDto(
            assignment.Id,
            assignment.Title,
            assignment.Content,
            assignment.CohortId,
            assignment.DueDate,
            assignment.IsTemplate,
            assignment.CreatedByUserId,
            assignment.CreatedAt,
            assignment.Tasks.Count,
            assignment.SubmissionRequired
        );
    }
}