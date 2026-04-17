using Lumenform.Application.DTOs.Assignments;
using Lumenform.Application.Services;
using LumenformCore.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumenform.API.Controllers;

[Route("api/cohorts/{cohortId}/assignments")]
public class AssignmentsController : BaseApiController
{
    private readonly AssignmentService _assignmentService;

    public AssignmentsController(AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Member")] 
    public async Task<ActionResult<AssignmentDto>> GetAssignment(Guid id, CancellationToken cancellationToken)
    {
        var assignment = await _assignmentService.GetAssignmentByIdAsync(id, cancellationToken);
        
        if (assignment == null)
            return NotFound();

        return Ok(assignment);
    }

    [HttpGet]
    [Authorize(Policy = "Member")] 
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetCohortAssignments(Guid cohortId, CancellationToken cancellationToken)
    {
        var assignments = await _assignmentService.GetCohortAssignmentsAsync(cohortId, cancellationToken);
        return Ok(assignments);
    }

    [HttpPost]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment(Guid cohortId, CreateAssignmentDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var assignment = await _assignmentService.CreateAssignmentAsync(dto, cohortId, userId, cancellationToken);
        
        return CreatedAtAction(nameof(GetAssignment), new {cohortId, id = assignment.Id }, assignment);
    }
}