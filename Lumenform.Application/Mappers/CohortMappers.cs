using Lumenform.Application.DTOs.Assignments;
using Lumenform.Application.DTOs.Cohorts;
using Lumenform.Application.Interfaces;
using Lumenform.Domain.Entities;


namespace Lumenform.Application.Mappers;

public static class CohortMappers
{
    public static CohortDto MapToCohortDto(Cohort cohort)
    {
        return new CohortDto(
            cohort.Id,
            cohort.Name,
            cohort.StartDate,
            cohort.EndDate,
            cohort.ParishName,
            cohort.IsActive,
            cohort.CreatedByUserId,
            cohort.CreatedAt,
            cohort.Memberships.Count
        );
    }
    
    public static CohortDetailDto MapToCohortDetailDto(Cohort cohort)
    {
        return new CohortDetailDto(
            cohort.Id,
            cohort.Name,
            cohort.StartDate,
            cohort.EndDate,
            cohort.ParishName,
            cohort.IsActive,
            cohort.CreatedByUserId,
            cohort.CreatedAt,
            cohort.Memberships.Select(MapToCohortMemberDto).ToList(),  // ← Full list
            cohort.Assignments.Select(AssignmentMappers.MapToAssignmentDto).ToList()     // ← Full list
        );
    }
    
    public static CohortDetailDtoWithUserInfo MapToCohortDetailDtoWithUserInfo(Cohort cohort, Dictionary<Guid, SupabaseUserInfo> users)
    {
        var membersWithUserInfo = cohort.Memberships.Select(member =>
        {
            var user = users.GetValueOrDefault(member.UserId);
            return MapToCohortMemberDtoWithUserInfo(
                member, 
                user?.FirstName ?? "Unkown", 
                user?.LastName ?? "User"
                );
        }).ToList();
        
        return new CohortDetailDtoWithUserInfo(
            cohort.Id,
            cohort.Name,
            cohort.StartDate,
            cohort.EndDate,
            cohort.ParishName,
            cohort.IsActive,
            cohort.CreatedByUserId,
            cohort.CreatedAt,
            membersWithUserInfo,
            cohort.Assignments.Select(AssignmentMappers.MapToAssignmentDto).ToList()
        );
    }
    
    public static CohortMemberDto MapToCohortMemberDto(CohortMembership membership)
    {
        return new CohortMemberDto(
            membership.Id,
            membership.UserId,
            membership.Role,
            membership.Status,
            membership.ParticipantType,
            membership.SponsorUserId,
            membership.JoinedDate,
            membership.CompletedDate
        );
    }

    public static CohortMemberDtoWithUserInfo MapToCohortMemberDtoWithUserInfo(
        CohortMembership membership,
        string firstName,
        string lastName)
    {
        return new CohortMemberDtoWithUserInfo(
            membership.Id,
            membership.UserId,
            firstName,
            lastName,
            membership.Role,
            membership.Status,
            membership.ParticipantType,
            membership.SponsorUserId,
            membership.JoinedDate,
            membership.CompletedDate
        );
    }
    
    public static CohortEventDto MapToCohortEventDto(CohortEvent cohortEvent)
    {
        return new CohortEventDto(
            cohortEvent.Id,
            cohortEvent.CohortId,
            cohortEvent.Title,
            cohortEvent.Description,
            cohortEvent.EventDate,
            cohortEvent.Type,
            cohortEvent.Status,
            cohortEvent.IsRequired,
            cohortEvent.CreatedAt
        );
    }
}