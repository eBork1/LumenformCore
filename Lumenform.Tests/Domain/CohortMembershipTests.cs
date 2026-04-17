using FluentAssertions;
using Lumenform.Domain.Entities;
using Lumenform.Domain.Enums;
using Lumenform.Domain.Exceptions;

namespace Lumenform.Tests.Domain;

public class CohortMembershipTests
{
    // ------------------------------------------------------------
    // CohortMembership.Create — validation rules
    // ------------------------------------------------------------

    [Fact]
    public void Create_ParticipantWithType_Succeeds()
    {
        var membership = CohortMembership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CohortRole.Participant,
            ParticipantType.Catechumen);

        membership.Role.Should().Be(CohortRole.Participant);
        membership.ParticipantType.Should().Be(ParticipantType.Catechumen);
        membership.Status.Should().Be(MembershipStatus.Active);
    }

    [Fact]
    public void Create_ParticipantWithoutType_ThrowsDomainException()
    {
        // A Participant MUST have a type — no type means bad data
        Action act = () => CohortMembership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CohortRole.Participant,
            participantType: null);

        act.Should().Throw<DomainException>()
            .WithMessage("Participant must have a type (Catechumen or Candidate)");
    }

    [Fact]
    public void Create_CoordinatorWithParticipantType_ThrowsDomainException()
    {
        // Non-participants cannot have a participant type
        Action act = () => CohortMembership.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            CohortRole.Coordinator,
            ParticipantType.Catechumen); // wrong — coordinators don't have participant types

        act.Should().Throw<DomainException>()
            .WithMessage("Only participants can have a participant type");
    }

    [Fact]
    public void Create_Coordinator_HasNoParticipantType()
    {
        var membership = CohortMembership.Create(Guid.NewGuid(), Guid.NewGuid(), CohortRole.Coordinator);

        // .BeNull() checks that the value is null
        membership.ParticipantType.Should().BeNull();
    }

    // ------------------------------------------------------------
    // AssignSponsor
    // ------------------------------------------------------------

    [Fact]
    public void AssignSponsor_ToParticipant_SetsSponsorUserId()
    {
        var membership = CohortMembership.Create(
            Guid.NewGuid(), Guid.NewGuid(), CohortRole.Participant, ParticipantType.Catechumen);
        var sponsorId = Guid.NewGuid();

        membership.AssignSponsor(sponsorId);

        membership.SponsorUserId.Should().Be(sponsorId);
    }

    [Fact]
    public void AssignSponsor_ToCoordinator_ThrowsDomainException()
    {
        var membership = CohortMembership.Create(Guid.NewGuid(), Guid.NewGuid(), CohortRole.Coordinator);

        Action act = () => membership.AssignSponsor(Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Only participants can have sponsors");
    }

    // ------------------------------------------------------------
    // Status transitions: Withdraw → Reactivate → MarkAsCompleted
    // ------------------------------------------------------------

    [Fact]
    public void Withdraw_ChangesStatusToWithdrawn()
    {
        var membership = CohortMembership.Create(Guid.NewGuid(), Guid.NewGuid(), CohortRole.Coordinator);

        membership.Withdraw();

        membership.Status.Should().Be(MembershipStatus.Withdrawn);
    }

    [Fact]
    public void Reactivate_AfterWithdraw_ChangesStatusToActive()
    {
        var membership = CohortMembership.Create(Guid.NewGuid(), Guid.NewGuid(), CohortRole.Coordinator);
        membership.Withdraw();

        membership.Reactivate();

        membership.Status.Should().Be(MembershipStatus.Active);
    }

    [Fact]
    public void MarkAsCompleted_SetsCompletedDateAndStatus()
    {
        var membership = CohortMembership.Create(Guid.NewGuid(), Guid.NewGuid(), CohortRole.Coordinator);

        membership.MarkAsCompleted();

        membership.Status.Should().Be(MembershipStatus.Completed);
        // .NotBeNull() checks that a nullable value has a value
        membership.CompletedDate.Should().NotBeNull();
    }

    [Fact]
    public void Reactivate_AfterCompleted_ClearsCompletedDate()
    {
        var membership = CohortMembership.Create(Guid.NewGuid(), Guid.NewGuid(), CohortRole.Coordinator);
        membership.MarkAsCompleted();

        membership.Reactivate();

        membership.Status.Should().Be(MembershipStatus.Active);
        membership.CompletedDate.Should().BeNull();
    }

    // ------------------------------------------------------------
    // ChangeRole
    // ------------------------------------------------------------

    [Fact]
    public void ChangeRole_FromParticipantToCoordinator_ClearsParticipantType()
    {
        var membership = CohortMembership.Create(
            Guid.NewGuid(), Guid.NewGuid(), CohortRole.Participant, ParticipantType.Elect);

        membership.ChangeRole(CohortRole.Coordinator);

        membership.Role.Should().Be(CohortRole.Coordinator);
        membership.ParticipantType.Should().BeNull(); // type is cleared when no longer a participant
    }
}