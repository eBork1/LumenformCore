using Lumenform.Application.DTOs.Assignments;
using Lumenform.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LumenformCore.Controllers;

[Authorize]
public class TemplatesController : BaseApiController
{
    private readonly AssignmentService _assignmentService;

    public TemplatesController(AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetMyTemplates(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var templates = await _assignmentService.GetMyTemplatesAsync(userId, cancellationToken);
        return Ok(templates);
    }
}