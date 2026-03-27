using Lumenform.Domain.Enums;

namespace Lumenform.Application.DTOs.Cohorts;

public record CreateCohortEventDto(
    string Title,
    DateTime EventDate,
    CohortEventType Type,
    string? Description,
    bool IsRequired
);