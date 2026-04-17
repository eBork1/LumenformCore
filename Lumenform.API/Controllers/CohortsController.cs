using Lumenform.Application.DTOs.Cohorts;
using Lumenform.Application.Services;
using LumenformCore.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumenform.API.Controllers;

public class CohortsController : BaseApiController
{
    private readonly CohortService _cohortService;
    private readonly CohortMemberService _cohortMemberService;
    private readonly CohortEventService _cohortEventService;

    public CohortsController(
        CohortService cohortService,
        CohortMemberService cohortMemberService,
        CohortEventService cohortEventService)
    {
        _cohortService = cohortService;
        _cohortMemberService = cohortMemberService;
        _cohortEventService = cohortEventService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CohortDto>>> GetCohorts(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var cohorts = await _cohortService.GetMyCohortsAsync(userId, cancellationToken);
        return Ok(cohorts);
    }

    [HttpGet("{cohortId}")]
    [Authorize(Policy = "Member")]
    public async Task<ActionResult<CohortDetailDtoWithUserInfo>> GetCohort(Guid cohortId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var cohort = await _cohortService.GetCohortByIdAsync(cohortId, userId, cancellationToken);

        if (cohort == null)
            return NotFound();

        return Ok(cohort);
    }

    [HttpPost("{cohortId}/members")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<CohortMemberDto>> AddMember(
        Guid cohortId,
        AddMemberDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var member = await _cohortMemberService.AddMemberAsync(cohortId, dto, cancellationToken);
            return Ok(member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{cohortId}/members/{membershipId}/reactivate")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<CohortMemberDto>> ReactivateMember(
        Guid cohortId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        try
        {
            var reactivatedMember = await _cohortMemberService
                .ReactivateMemberAsync(cohortId, membershipId, cancellationToken);
            return Ok(reactivatedMember);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{cohortId}/members/{membershipId}")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult> RemoveMember(
        Guid cohortId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cohortMemberService.WithdrawMemberAsync(cohortId, membershipId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CohortDto>> CreateCohort(CreateCohortDto dto, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var cohort = await _cohortService.CreateCohortAsync(dto, userId, cancellationToken);

        return CreatedAtAction(nameof(GetCohort), new { cohortId = cohort.Id }, cohort);
    }

    [HttpGet("{cohortId}/events")]
    [Authorize(Policy = "Member")]
    public async Task<ActionResult<IEnumerable<CohortEventDto>>> GetCohortEvents(
        Guid cohortId,
        CancellationToken cancellationToken)
    {
        try
        {
            var events = await _cohortEventService.GetCohortEventsAsync(cohortId, cancellationToken);
            return Ok(events);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{cohortId}/events")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<CohortEventDto>> AddEvent(
        Guid cohortId,
        CreateCohortEventDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var cohortEvent = await _cohortEventService.AddEventAsync(cohortId, dto, cancellationToken);
            return Ok(cohortEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{cohortId}/events/{eventId}")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<CohortEventDto>> UpdateEvent(
        Guid cohortId,
        Guid eventId,
        UpdateCohortEventDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var cohortEvent = await _cohortEventService.UpdateEventAsync(cohortId, eventId, dto, cancellationToken);
            return Ok(cohortEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{cohortId}/events/{eventId}/cancel")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult> CancelEvent(
        Guid cohortId,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cohortEventService.CancelEventAsync(cohortId, eventId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{cohortId}/events/{eventId}/complete")]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult> CompleteEvent(
        Guid cohortId,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cohortEventService.CompleteEventAsync(cohortId, eventId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}