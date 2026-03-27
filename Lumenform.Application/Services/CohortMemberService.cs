using Lumenform.Application.DTOs.Cohorts;
using Lumenform.Application.Mappers;
using Lumenform.Application.Repositories;
using Lumenform.Domain.Enums;

namespace Lumenform.Application.Services;

public class CohortMemberService
{
    private readonly ICohortRepository _cohortRepository;

    public CohortMemberService(ICohortRepository cohortRepository)
    {
        _cohortRepository = cohortRepository;
    }
    
    public async Task<CohortMemberDto> AddMemberAsync(
        Guid cohortId, 
        AddMemberDto dto, 
        CancellationToken cancellationToken = default)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
    
        if (cohort == null)
            throw new Exception("Cohort not found");
        
        var existingMember = cohort.Memberships
            .SingleOrDefault(m => m.UserId == dto.UserId);

        if (existingMember != null)
        {
            if (existingMember.Status == MembershipStatus.Active)
                throw new InvalidOperationException("User is already an active member");

            if (existingMember.Status == MembershipStatus.Completed)
                throw new InvalidOperationException("User has already completed this cohort");
            
            existingMember.Reactivate();
            existingMember.ChangeRole(dto.Role);
            if (dto.Role == CohortRole.Participant)
            {
                existingMember.ChangeParticipantType(dto.ParticipantType);
                if (dto.SponsorUserId.HasValue)
                {
                    existingMember.AssignSponsor(dto.SponsorUserId.Value);
                }
            }
            
            await _cohortRepository.UpdateAsync(cohort, cancellationToken);
            return CohortMappers.MapToCohortMemberDto(existingMember);
        }
        
        var membership = cohort.AddMember(
            dto.UserId,
            dto.Role,
            dto.ParticipantType,
            dto.SponsorUserId
        );
    
        // Save changes
        await _cohortRepository.UpdateAsync(cohort, cancellationToken);
        return CohortMappers.MapToCohortMemberDto(membership);
    }

    public async Task<CohortMemberDto> ReactivateMemberAsync(
        Guid cohortId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
        
        if (cohort == null)
            throw new Exception("Cohort not found");
        
        var updatedMembership = cohort.ReactivateMember(membershipId);
        await _cohortRepository.UpdateAsync(cohort, cancellationToken);
        return CohortMappers.MapToCohortMemberDto(updatedMembership);
    }

    public async Task WithdrawMemberAsync(
        Guid cohortId,
        Guid membershipId,
        CancellationToken cancellationToken)
    {
        var cohort = await _cohortRepository.GetByIdAsync(cohortId, cancellationToken);
    
        if (cohort == null)
            throw new Exception("Cohort not found");
        
        cohort.WithdrawMember(membershipId);
        await _cohortRepository.UpdateAsync(cohort, cancellationToken);
    }
}