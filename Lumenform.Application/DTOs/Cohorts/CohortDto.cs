namespace Lumenform.Application.DTOs.Cohorts;

public record CohortDto(
    Guid Id,
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    string? ParishName,
    bool IsActive,
    Guid CreatedByUserId,
    DateTime CreatedAt,
    int MemberCount
);
