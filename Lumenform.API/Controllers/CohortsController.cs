using Lumenform.Application.DTOs.Cohorts;
using Lumenform.Application.Services;
using LumenformCore.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lumenform.API.Controllers;

[Authorize]
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

    [HttpGet("{id}")]
    [Authorize]
    [Authorize(Policy = "Member")]
    public async Task<ActionResult<CohortDetailDtoWithUserInfo>> GetCohort(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var cohort = await _cohortService.GetCohortByIdAsync(id, userId, cancellationToken);

        if (cohort == null)
            return NotFound();

        return Ok(cohort);
    }

    [HttpPost("{id}/members")]
    [Authorize]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<CohortMemberDto>> AddMember(
        Guid id,
        AddMemberDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var member = await _cohortMemberService.AddMemberAsync(id, dto, cancellationToken);
            return Ok(member);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/members/{membershipId}/reactivate")]
    [Authorize]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult<CohortMemberDto>> ReactivateMember(
        Guid id,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        try
        {
            var reactivatedMember = await _cohortMemberService
                .ReactivateMemberAsync(id, membershipId, cancellationToken);
            return Ok(reactivatedMember);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpDelete("{id}/members/{membershipId}")]
    [Authorize]
    [Authorize(Policy = "Coordinator")]
    public async Task<ActionResult> RemoveMember(
        Guid id,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cohortMemberService.WithdrawMemberAsync(id, membershipId, cancellationToken);
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

        return CreatedAtAction(nameof(GetCohort), new { id = cohort.Id }, cohort);
    }

    [HttpGet("{id}/events")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CohortEventDto>>> GetCohortEvents(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var events = await _cohortEventService.GetCohortEventsAsync(id, cancellationToken);
            return Ok(events);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/events")]
    [Authorize]
    public async Task<ActionResult<CohortEventDto>> AddEvent(
        Guid id,
        CreateCohortEventDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var cohortEvent = await _cohortEventService.AddEventAsync(id, dto, cancellationToken);
            return Ok(cohortEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/events/{eventId}")]
    [Authorize]
    public async Task<ActionResult<CohortEventDto>> UpdateEvent(
        Guid id,
        Guid eventId,
        UpdateCohortEventDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var cohortEvent = await _cohortEventService.UpdateEventAsync(id, eventId, dto, cancellationToken);
            return Ok(cohortEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/events/{eventId}/cancel")]
    [Authorize]
    public async Task<ActionResult> CancelEvent(
        Guid id,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cohortEventService.CancelEventAsync(id, eventId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/events/{eventId}/complete")]
    [Authorize]
    public async Task<ActionResult> CompleteEvent(
        Guid id,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _cohortEventService.CompleteEventAsync(id, eventId, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}