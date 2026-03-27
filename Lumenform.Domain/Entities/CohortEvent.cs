using Lumenform.Domain.Enums;
using Lumenform.Domain.Exceptions;

namespace Lumenform.Domain.Entities;

public class CohortEvent : Entity
{
    public Guid CohortId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime EventDate { get; private set; }
    public CohortEventType Type { get; private set; }
    public CohortEventStatus Status { get; private set; }
    public bool IsRequired { get; private set; }
    
    // Navigation
    public Cohort Cohort { get; private set; } = null!;

    private CohortEvent()
    {
        Title = string.Empty;
    }

    public static CohortEvent Create(
        Guid cohortId,
        string title,
        DateTime eventDate,
        CohortEventType type,
        string? description = null,
        bool isRequired = false)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Event title cannot be empty");

        return new CohortEvent
        {
            CohortId = cohortId,
            Title = title,
            Description = description,
            EventDate = eventDate,
            Type = type,
            Status = CohortEventStatus.Scheduled,
            IsRequired = isRequired
        };
    }

    public void UpdateDetails(string title, DateTime eventDate, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Event title cannot be empty");

        Title = title;
        EventDate = eventDate;
        Description = description;
        UpdateTimestamp();
    }

    public void Cancel()
    {
        Status = CohortEventStatus.Cancelled;
        UpdateTimestamp();
    }

    public void Complete()
    {
        Status = CohortEventStatus.Completed;
        UpdateTimestamp();
    }

    public void Reschedule(DateTime newDate)
    {
        EventDate = newDate;
        Status = CohortEventStatus.Scheduled;
        UpdateTimestamp();
    }

    public void Postpone()
    {
        Status = CohortEventStatus.Postponed;
        UpdateTimestamp();
    }
}