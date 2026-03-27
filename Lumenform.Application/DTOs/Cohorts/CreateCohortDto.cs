namespace Lumenform.Application.DTOs.Cohorts;

public record CreateCohortDto(
    string Name,
    DateTime? StartDate,
    DateTime? EndDate,
    string? ParishName
);