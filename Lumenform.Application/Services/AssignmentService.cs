using Lumenform.Application.DTOs.Assignments;
using Lumenform.Application.Mappers;
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

    public async Task<AssignmentDto> CreateAssignmentAsync(
        CreateAssignmentDto dto, 
        Guid cohortId,
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        var assignment = Assignment.Create(
            dto.Title,
            dto.Content,
            cohortId,
            userId,
            dto.SubmissionRequired,
            dto.DueDate
        );
        
        // Add tasks to assignment for each task in list from dto.
        foreach (var task in dto.Tasks ?? [])
        {
            assignment.AddTask(task);
        }

        await _assignmentRepository.AddAsync(assignment, cancellationToken);

        return AssignmentMappers.MapToAssignmentDto(assignment);
    }

    public async Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id, cancellationToken);
        return assignment == null ? null : AssignmentMappers.MapToAssignmentDto(assignment);
    }

    public async Task<IEnumerable<AssignmentDto>> GetCohortAssignmentsAsync(Guid cohortId, CancellationToken cancellationToken = default)
    {
        var assignments = await _assignmentRepository.GetByCohortAsync(cohortId, cancellationToken);
        return assignments.Select(AssignmentMappers.MapToAssignmentDto);
    }

    public async Task<IEnumerable<AssignmentDto>> GetMyTemplatesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var templates = await _assignmentRepository.GetTemplatesByCreatorAsync(userId, cancellationToken);
        return templates.Select(AssignmentMappers.MapToAssignmentDto);
    }
}