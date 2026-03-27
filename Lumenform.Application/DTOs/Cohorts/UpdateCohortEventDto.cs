namespace Lumenform.Application.DTOs.Cohorts;

public record UpdateCohortEventDto(
    string Title,
    DateTime EventDate,
    string? Description
);