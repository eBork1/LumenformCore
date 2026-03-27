using Lumenform.Domain.Enums;
using Lumenform.Domain.Exceptions;

namespace Lumenform.Domain.Entities;

public class CohortMembership : Entity
{
    public Guid CohortId { get; private set; }
    public Guid UserId { get; private set; }  // References Supabase auth.users
    public CohortRole Role { get; private set; }
    public MembershipStatus Status { get; private set; }
    public ParticipantType? ParticipantType { get; private set; }  // Only for Participants
    public Guid? SponsorUserId { get; private set; }  // If Participant, who's their sponsor?
    public DateTime JoinedDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }
    
    // Navigation properties
    public Cohort Cohort { get; private set; } = null!;

    // Private constructor for EF Core
    private CohortMembership() { }

    // Factory method
    public static CohortMembership Create(
        Guid cohortId, 
        Guid userId, 
        CohortRole role, 
        ParticipantType? participantType = null,
        Guid? sponsorUserId = null)
    {
        // Validation
        if (role == CohortRole.Participant && !participantType.HasValue)
            throw new DomainException("Participant must have a type (Catechumen or Candidate)");

        if (role != CohortRole.Participant && participantType.HasValue)
            throw new DomainException("Only participants can have a participant type");

        return new CohortMembership
        {
            CohortId = cohortId,
            UserId = userId,
            Role = role,
            Status = MembershipStatus.Active,
            ParticipantType = participantType,
            SponsorUserId = sponsorUserId,
            JoinedDate = DateTime.UtcNow
        };
    }

    // Business logic methods
    public void AssignSponsor(Guid sponsorUserId)
    {
        if (Role != CohortRole.Participant)
            throw new DomainException("Only participants can have sponsors");

        SponsorUserId = sponsorUserId;
        UpdateTimestamp();
    }

    public void RemoveSponsor()
    {
        SponsorUserId = null;
        UpdateTimestamp();
    }

    public void MarkAsCompleted()
    {
        Status = MembershipStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Withdraw()
    {
        Status = MembershipStatus.Withdrawn;
        UpdateTimestamp();
    }

    public void Reactivate()
    {
        Status = MembershipStatus.Active;
        CompletedDate = null;
        UpdateTimestamp();
    }

    public void ChangeRole(CohortRole newRole)
    {
        Role = newRole;
        if (newRole != CohortRole.Participant)
        {
            ParticipantType = null;
        }
        UpdateTimestamp();
    }
    
    public void ChangeParticipantType(ParticipantType? newParticipantType)
    {
        ParticipantType = newParticipantType;
        UpdateTimestamp();
    }
}