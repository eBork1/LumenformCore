using FluentAssertions;
using Lumenform.Domain.Entities;
using Lumenform.Domain.Exceptions;

namespace Lumenform.Tests.Domain;

public class AssignmentTests
{
    // ------------------------------------------------------------
    // Helpers — small private methods that build common test data
    //           so you don't repeat the same setup in every test.
    // ------------------------------------------------------------

    private static Assignment MakeCohortAssignment() =>
        Assignment.Create("Week 1 Reading", "<p>Read chapter 1</p>", Guid.NewGuid(), Guid.NewGuid(), false);

    private static Assignment MakeTemplate() =>
        Assignment.CreateTemplate("Template A", "<p>Template content</p>", Guid.NewGuid(), false);

    // ------------------------------------------------------------
    // Assignment.Create
    // ------------------------------------------------------------

    [Fact]
    public void Create_WithValidData_ReturnsNonTemplate()
    {
        var cohortId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var assignment = Assignment.Create("Week 1", "<p>Content</p>", cohortId, userId, true);

        assignment.Title.Should().Be("Week 1");
        assignment.CohortId.Should().Be(cohortId);
        assignment.IsTemplate.Should().BeFalse();
        assignment.SubmissionRequired.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "<p>Content</p>")]
    [InlineData("   ", "<p>Content</p>")]
    [InlineData("Week 1", "")]
    [InlineData("Week 1", "   ")]
    public void Create_WithEmptyTitleOrContent_ThrowsDomainException(string title, string content)
    {
        Action act = () => Assignment.Create(title, content, Guid.NewGuid(), Guid.NewGuid(), false);

        act.Should().Throw<DomainException>();
    }

    // ------------------------------------------------------------
    // Assignment.CreateTemplate
    // ------------------------------------------------------------

    [Fact]
    public void CreateTemplate_SetsIsTemplateTrue_AndNullCohortId()
    {
        var template = Assignment.CreateTemplate("Template A", "<p>Content</p>", Guid.NewGuid(), false);

        template.IsTemplate.Should().BeTrue();
        template.CohortId.Should().BeNull();
    }

    // ------------------------------------------------------------
    // Assignment.AddTask / RemoveTask — order management
    // ------------------------------------------------------------

    [Fact]
    public void AddTask_FirstTask_GetsOrderOne()
    {
        var assignment = MakeCohortAssignment();

        var task = assignment.AddTask("Read the intro");

        task.Order.Should().Be(1);
    }

    [Fact]
    public void AddTask_MultipleTasksIncrementOrder()
    {
        var assignment = MakeCohortAssignment();

        assignment.AddTask("Task 1");
        assignment.AddTask("Task 2");
        var third = assignment.AddTask("Task 3");

        third.Order.Should().Be(3);
        assignment.Tasks.Should().HaveCount(3);
    }

    [Fact]
    public void AddTask_EmptyDescription_ThrowsDomainException()
    {
        var assignment = MakeCohortAssignment();

        Action act = () => assignment.AddTask("   ");

        act.Should().Throw<DomainException>()
            .WithMessage("Task description cannot be empty");
    }

    [Fact]
    public void RemoveTask_MiddleTask_ReordersRemainingTasksConsecutively()
    {
        // Arrange: three tasks with orders 1, 2, 3
        var assignment = MakeCohortAssignment();
        assignment.AddTask("Task 1");
        var middle = assignment.AddTask("Task 2");
        assignment.AddTask("Task 3");

        // Act: remove the middle one (order 2)
        assignment.RemoveTask(middle.Id);

        // Assert: the remaining two should now be 1, 2 (no gaps)
        var remaining = assignment.Tasks.OrderBy(t => t.Order).ToList();
        remaining.Should().HaveCount(2);
        remaining[0].Order.Should().Be(1);
        remaining[1].Order.Should().Be(2);
    }

    [Fact]
    public void RemoveTask_NonExistentId_ThrowsDomainException()
    {
        var assignment = MakeCohortAssignment();
        assignment.AddTask("Task 1");

        Action act = () => assignment.RemoveTask(Guid.NewGuid());

        act.Should().Throw<DomainException>()
            .WithMessage("Task not found");
    }

    // ------------------------------------------------------------
    // Assignment.ReorderTasks
    // ------------------------------------------------------------

    [Fact]
    public void ReorderTasks_ValidIds_UpdatesOrder()
    {
        var assignment = MakeCohortAssignment();
        var t1 = assignment.AddTask("Task 1");
        var t2 = assignment.AddTask("Task 2");
        var t3 = assignment.AddTask("Task 3");

        // Reverse the order: 3, 2, 1
        assignment.ReorderTasks([t3.Id, t2.Id, t1.Id]);

        // t3 should now be order 1, t1 should be order 3
        assignment.Tasks.First(t => t.Id == t3.Id).Order.Should().Be(1);
        assignment.Tasks.First(t => t.Id == t1.Id).Order.Should().Be(3);
    }

    [Fact]
    public void ReorderTasks_WrongCount_ThrowsDomainException()
    {
        var assignment = MakeCohortAssignment();
        assignment.AddTask("Task 1");
        assignment.AddTask("Task 2");

        // Providing only one ID when there are two tasks
        Action act = () => assignment.ReorderTasks([Guid.NewGuid()]);

        act.Should().Throw<DomainException>()
            .WithMessage("Must provide all task IDs for reordering");
    }

    // ------------------------------------------------------------
    // Assignment.CloneToCohort
    // ------------------------------------------------------------

    [Fact]
    public void CloneToCohort_FromTemplate_CopiesTitleContentAndTasks()
    {
        var template = MakeTemplate();
        template.AddTask("Step 1");
        template.AddTask("Step 2");
        var cohortId = Guid.NewGuid();

        var clone = template.CloneToCohort(cohortId, submissionRequired: true);

        clone.Title.Should().Be(template.Title);
        clone.Content.Should().Be(template.Content);
        clone.CohortId.Should().Be(cohortId);
        clone.IsTemplate.Should().BeFalse();
        clone.SubmissionRequired.Should().BeTrue();
        // Tasks should be copied, preserving count and order
        clone.Tasks.Should().HaveCount(2);
        clone.Tasks.OrderBy(t => t.Order).First().Description.Should().Be("Step 1");
    }

    [Fact]
    public void CloneToCohort_FromNonTemplate_ThrowsDomainException()
    {
        var assignment = MakeCohortAssignment(); // not a template

        Action act = () => assignment.CloneToCohort(Guid.NewGuid(), false);

        act.Should().Throw<DomainException>()
            .WithMessage("Can only clone templates");
    }

    // ------------------------------------------------------------
    // Assignment.UpdateDueDate
    // ------------------------------------------------------------

    [Fact]
    public void UpdateDueDate_OnTemplate_ThrowsDomainException()
    {
        var template = MakeTemplate();

        Action act = () => template.UpdateDueDate(DateTime.UtcNow);

        act.Should().Throw<DomainException>()
            .WithMessage("Templates cannot have due dates");
    }

    [Fact]
    public void UpdateDueDate_OnCohortAssignment_SetsDate()
    {
        var assignment = MakeCohortAssignment();
        var due = new DateTime(2026, 12, 25);

        assignment.UpdateDueDate(due);

        assignment.DueDate.Should().Be(due);
    }

    // ------------------------------------------------------------
    // Assignment.HasUserSubmitted
    // ------------------------------------------------------------

    [Fact]
    public void HasUserSubmitted_WhenNoSubmissions_ReturnsFalse()
    {
        var assignment = MakeCohortAssignment();

        assignment.HasUserSubmitted(Guid.NewGuid()).Should().BeFalse();
    }
}