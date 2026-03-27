using Lumenform.Application.DTOs.Cohorts;
using Lumenform.Application.Mappers;
using Lumenform.Application.Repositories;

namespace Lumenform.Application.Services;

public class CohortEventService
{
    private readonly ICohortRepository _cohortRepository;

    public CohortEventService(ICohortRepository cohortRepository)
    {
        _cohortRepository = cohortRepository;
    }
    
    public async Task<IEnumerable<CohortEventDto>> GetCohortEventsAsync(
        Guid cohortId, 
        CancellationToken cancellationToken = default)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
    
        if (cohort == null)
            throw new Exception("Cohort not found");

        return cohort.Events
            .OrderBy(e => e.EventDate)
            .Select(CohortMappers.MapToCohortEventDto);
    }
    
    public async Task<CohortEventDto> AddEventAsync(
        Guid cohortId,
        CreateCohortEventDto dto,
        CancellationToken cancellationToken = default)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
    
        if (cohort == null)
            throw new Exception("Cohort not found");

        var cohortEvent = cohort.AddEvent(
            dto.Title,
            dto.EventDate,
            dto.Type,
            dto.Description,
            dto.IsRequired
        );

        await _cohortRepository.UpdateAsync(cohort, cancellationToken);

        return CohortMappers.MapToCohortEventDto(cohortEvent);
    }
    
    public async Task<CohortEventDto> UpdateEventAsync(
        Guid cohortId,
        Guid eventId,
        UpdateCohortEventDto dto,
        CancellationToken cancellationToken = default)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
    
        if (cohort == null)
            throw new Exception("Cohort not found");

        var cohortEvent = cohort.Events.FirstOrDefault(e => e.Id == eventId);
        if (cohortEvent == null)
            throw new Exception("Event not found");

        cohortEvent.UpdateDetails(dto.Title, dto.EventDate, dto.Description);
    
        await _cohortRepository.UpdateAsync(cohort, cancellationToken);

        return CohortMappers.MapToCohortEventDto(cohortEvent);
    }

    public async Task CancelEventAsync(
        Guid cohortId,
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
    
        if (cohort == null)
            throw new Exception("Cohort not found");

        var cohortEvent = cohort.Events.FirstOrDefault(e => e.Id == eventId);
        if (cohortEvent == null)
            throw new Exception("Event not found");

        cohortEvent.Cancel();
    
        await _cohortRepository.UpdateAsync(cohort, cancellationToken);
    }
    
    public async Task CompleteEventAsync(
        Guid cohortId,
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
    
        if (cohort == null)
            throw new Exception("Cohort not found");

        var cohortEvent = cohort.Events.FirstOrDefault(e => e.Id == eventId);
        if (cohortEvent == null)
            throw new Exception("Event not found");

        cohortEvent.Complete();
    
        await _cohortRepository.UpdateAsync(cohort, cancellationToken);
    }
}