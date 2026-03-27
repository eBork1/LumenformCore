using Lumenform.Application.DTOs.Assignments;
using Lumenform.Application.Repositories;
using Lumenform.Domain.Entities;

namespace Lumenform.Application.Services;

public class AssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;

    public AssignmentService(IAssignmentRepository assignmentRepository)
    {
        _assignmentRepository = assignmentRepository;
    }

    public async Task<AssignmentDto> CreateAssignmentAsync(CreateAssignmentDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        var assignment = Assignment.Create(
            dto.Title,
            dto.Content,
            dto.CohortId,
            userId,
            dto.DueDate
        );

        await _assignmentRepository.AddAsync(assignment, cancellationToken);

        return MapToAssignmentDto(assignment);
    }

    public async Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
        return assignment == null ? null : MapToAssignmentDto(assignment);
    }

    public async Task<IEnumerable<AssignmentDto>> GetCohortAssignmentsAsync(Guid cohortId, CancellationToken cancellationToken = default)
    {
        var assignments = await _assignmentRepository.GetByCohortAsync(cohortId, cancellationToken);
        return assignments.Select(MapToAssignmentDto);
    }

    public async Task<IEnumerable<AssignmentDto>> GetMyTemplatesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var templates = await _assignmentRepository.GetTemplatesByCreatorAsync(userId, cancellationToken);
        return templates.Select(MapToAssignmentDto);
    }

    private static AssignmentDto MapToAssignmentDto(Assignment assignment)
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
            assignment.Tasks.Count
        );
    }
}