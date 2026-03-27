using Lumenform.Application.DTOs.Assignments;
using Lumenform.Application.Services;
using LumenformCore.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumenform.Controllers;

[Authorize]
public class AssignmentsController : BaseApiController
{
    private readonly AssignmentService _assignmentService;

    public AssignmentsController(AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignment(Guid id, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentService.GetAssignmentByIdAsync(id, cancellationToken);
        
        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }

    [HttpGet("cohort/{cohortId}")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetCohortAssignments(Guid cohortId, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentService.GetCohortAssignmentsAsync(cohortId, cancellationToken);
        return Ok(assignments);
    }

    [HttpGet("templates")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetMyTemplates(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var templates = await _assignmentService.GetMyTemplatesAsync(userId, cancellationToken);
        return Ok(templates);
    }

    [HttpPost]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment(CreateAssignmentDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var assignment = await _assignmentService.CreateAssignmentAsync(dto, userId, cancellationToken);
        
        return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
    }
}