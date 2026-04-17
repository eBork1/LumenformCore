using FluentAssertions;
using Lumenform.Domain.Entities;
using Lumenform.Domain.Enums;
using Lumenform.Domain.Exceptions;

namespace Lumenform.Tests.Domain;

// A "test class" groups related tests together. This one covers the Cohort entity.
public class CohortTests
{
    // ------------------------------------------------------------
    // Cohort.Create — valid input
    // ------------------------------------------------------------

    // [Fact] marks a method as a single test. It takes no parameters.
    [Fact]
    public void Create_WithValidData_ReturnsActiveCohort()
    {
        // Each test is split into three sections:
        //
        //   Arrange — set up the data you need
        //   Act     — call the thing you're testing
        //   Assert  — verify the result is what you expected
        //
        // You don't have to write the comments, but it helps while learning.

        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var cohort = Cohort.Create("RCIA 2026", userId, "St. Mary's");

        // Assert
        // FluentAssertions lets you write assertions that read like English.
        // .Should()  is how you start any assertion.
        // .Be()      checks exact equality.
        // .BeTrue()  checks that a bool is true.
        cohort.Name.Should().Be("RCIA 2026");
        cohort.ParishName.Should().Be("St. Mary's");
        cohort.CreatedByUserId.Should().Be(userId);
        cohort.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithStartAndEndDate_SetsDateRange()
    {
        var start = new DateTime(2026, 9, 1);
        var end = new DateTime(2027, 4, 1);

        var cohort = Cohort.Create("RCIA 2026", Guid.NewGuid(), "St. Mary's", start, end);

        cohort.StartDate.Should().Be(start);
        cohort.EndDate.Should().Be(end);
    }

    // ------------------------------------------------------------
    // Cohort.Create — invalid input (testing that exceptions are thrown)
    // ------------------------------------------------------------

    // [Theory] is like [Fact] but lets you run the same test with multiple inputs.
    // [InlineData(...)] provides each set of inputs.
    // The parameters of the method receive each value in order.
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ThrowsCohortException(string? name)
    {
        // Act + Assert combined:
        // FluentAssertions.Invoking() lets you assert that a method throws an exception.
        // .Should().Throw<T>() checks that it throws the right exception type.
        Action act = () => Cohort.Create(name!, Guid.NewGuid(), "St. Mary's");

        act.Should().Throw<CohortException>()
            .WithMessage("Cohort name cannot be empty");
    }

    [Fact]
    public void Create_WithStartDateAfterEndDate_ThrowsCohortException()
    {
        var start = new DateTime(2027, 1, 1);
        var end = new DateTime(2026, 1, 1); // end is before start — invalid

        Action act = () => Cohort.Create("RCIA", Guid.NewGuid(), "St. Mary's", start, end);

        act.Should().Throw<CohortException>()
            .WithMessage("Start date cannot be after end date");
    }

    // ------------------------------------------------------------
    // Cohort.UpdateDates
    // ------------------------------------------------------------

    [Fact]
    public void UpdateDates_WithInvertedRange_ThrowsCohortException()
    {
        var cohort = Cohort.Create("RCIA 2026", Guid.NewGuid(), "St. Mary's");

        Action act = () => cohort.UpdateDates(new DateTime(2027, 1, 1), new DateTime(2026, 1, 1));

        act.Should().Throw<CohortException>();
    }

    // ------------------------------------------------------------
    // Cohort.Archive / Reactivate
    // ------------------------------------------------------------

    [Fact]
    public void Archive_SetsIsActiveToFalse()
    {
        var cohort = Cohort.Create("RCIA 2026", Guid.NewGuid(), "St. Mary's");

        cohort.Archive();

        cohort.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Reactivate_AfterArchive_SetsIsActiveToTrue()
    {
        var cohort = Cohort.Create("RCIA 2026", Guid.NewGuid(), "St. Mary's");
        cohort.Archive();

        cohort.Reactivate();

        cohort.IsActive.Should().BeTrue();
    }

    // ------------------------------------------------------------
    // Cohort.AddMember
    // ------------------------------------------------------------

    [Fact]
    public void AddMember_NewUser_AddsMembership()
    {
        var cohort = Cohort.Create("RCIA 2026", Guid.NewGuid(), "St. Mary's");
        var userId = Guid.NewGuid();

        cohort.AddMember(userId, CohortRole.Coordinator);

        // .HaveCount() checks the number of items in a collection
        cohort.Memberships.Should().HaveCount(1);
        cohort.Memberships.First().UserId.Should().Be(userId);
    }

    [Fact]
    public void AddMember_DuplicateUser_ThrowsCohortException()
    {
        var cohort = Cohort.Create("RCIA 2026", Guid.NewGuid(), "St. Mary's");
        var userId = Guid.NewGuid();
        cohort.AddMember(userId, CohortRole.Coordinator);

        // Adding the same user a second time should throw
        Action act = () => cohort.AddMember(userId, CohortRole.Participant, ParticipantType.Catechumen);

        act.Should().Throw<CohortException>()
            .WithMessage("User is already a member of this cohort");
    }

    // ------------------------------------------------------------
    // Cohort.GetMembersByRole
    // ------------------------------------------------------------

    [Fact]
    public void GetMembersByRole_ReturnsOnlyActiveMatchingRoles()
    {
        var cohort = Cohort.Create("RCIA 2026", Guid.NewGuid(), "St. Mary's");
        var coordinatorId = Guid.NewGuid();
        var participantId = Guid.NewGuid();

        cohort.AddMember(coordinatorId, CohortRole.Coordinator);
        var participantMembership = cohort.AddMember(participantId, CohortRole.Participant, ParticipantType.Catechumen);

        // Withdraw the participant so they're no longer active
        cohort.WithdrawMember(participantMembership.Id);

        var coordinators = cohort.GetMembersByRole(CohortRole.Coordinator);

        coordinators.Should().HaveCount(1);
        coordinators.First().UserId.Should().Be(coordinatorId);
    }
}