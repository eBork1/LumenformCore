using Lumenform.Application.DTOs.Assignments;
using Lumenform.Application.DTOs.Cohorts;
using Lumenform.Application.Interfaces;
using Lumenform.Application.Repositories;
using Lumenform.Domain.Entities;
using Lumenform.Domain.Enums;
using Lumenform.Application.Mappers;

namespace Lumenform.Application.Services;

public class CohortService
{
    private readonly ICohortRepository _cohortRepository;
    private readonly ISupabaseUserService _supabaseService;

    public CohortService(ICohortRepository cohortRepository,  ISupabaseUserService supabaseService)
    {
        _cohortRepository = cohortRepository;
        _supabaseService = supabaseService;
    }

    public async Task<CohortDto> CreateCohortAsync(CreateCohortDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        var cohort = Cohort.Create(
            dto.Name,
            userId,
            dto.ParishName,
            dto.StartDate,
            dto.EndDate
        );
        
        // Add cohort owner as member.
        cohort.AddMember(userId, CohortRole.Coordinator, null, null);

        await _cohortRepository.AddAsync(cohort, cancellationToken);

        return CohortMappers.MapToCohortDto(cohort);
    }

    public async Task<CohortDetailDtoWithUserInfo?> GetCohortByIdAsync(
        Guid cohortId, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        // Get Cohort
        var cohort = await _cohortRepository.GetByIdForUserAsync(cohortId, userId, cancellationToken);
        if (cohort == null) return null;

        // Get Supabase users from cohort members
        var userIds = cohort.Memberships.Select(m => m.UserId).ToList();
        var users = await _supabaseService.GetUsersByIdsAsync(userIds);
        
        return CohortMappers.MapToCohortDetailDtoWithUserInfo(cohort, users);
    }

    public async Task<IEnumerable<CohortDto>> GetMyCohortsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cohorts = await _cohortRepository.GetByCreatorAsync(userId, cancellationToken);
        return cohorts.Select(CohortMappers.MapToCohortDto);
    }
    
    
}