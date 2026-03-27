using Lumenform.Domain.Enums;

namespace Lumenform.Application.DTOs.Cohorts;

public record AddMemberDto(
    Guid UserId,
    CohortRole Role,
    ParticipantType? ParticipantType,  // Required if Role is Participant
    Guid? SponsorUserId  // Optional - assign a sponsor
);