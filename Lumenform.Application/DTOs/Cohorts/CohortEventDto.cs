using Lumenform.Domain.Enums;

namespace Lumenform.Application.DTOs.Cohorts;

public record CohortEventDto(
    Guid Id,
    Guid CohortId,
    string Title,
    string? Description,
    DateTime EventDate,
    CohortEventType Type,
    CohortEventStatus Status,
    bool IsRequired,
    DateTime CreatedAt
);