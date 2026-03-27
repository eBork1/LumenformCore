using Lumenform.Domain.Enums;

namespace Lumenform.Application.DTOs.Cohorts;

public record CohortMemberDto(
    Guid Id,
    Guid UserId,
    CohortRole Role,
    MembershipStatus Status,
    ParticipantType? ParticipantType,
    Guid? SponsorUserId,
    DateTime JoinedDate,
    DateTime? CompletedDate
);

public record CohortMemberDtoWithUserInfo(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    CohortRole Role,
    MembershipStatus Status,
    ParticipantType? ParticipantType,
    Guid? SponsorUserId,
    DateTime JoinedDate,
    DateTime? CompletedDate
);
