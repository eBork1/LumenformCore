using Lumenform.Application.DTOs.Assignments;

namespace Lumenform.Application.DTOs.Cohorts;

public record CohortDetailDto(
    Guid Id,
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    string? ParishName,
    bool IsActive,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    List<CohortMemberDto> Members,
    List<AssignmentDto> Assignments
);

public record CohortDetailDtoWithUserInfo(
    Guid Id,
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    string? ParishName,
    bool IsActive,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    List<CohortMemberDtoWithUserInfo> Members,
    List<AssignmentDto> Assignments
);