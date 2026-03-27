using Lumenform.Domain.Enums;
using Lumenform.Domain.Exceptions;

namespace Lumenform.Domain.Entities;
public class Cohort : Entity
{
    public string Name { get; private set; }
    public string? ParishName { get; private set; }  // Optional, free text
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    
    // Navigation properties
    private readonly List<CohortMembership> _memberships = new();
    public IReadOnlyCollection<CohortMembership> Memberships => _memberships.AsReadOnly();
    
    private readonly List<Assignment> _assignments = new();
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();
    
    private readonly List<CohortEvent> _events = new();
    public IReadOnlyCollection<CohortEvent> Events => _events.AsReadOnly();

    // Private constructor for EF Core
    private Cohort() { }

    // Factory method for creating new cohorts
    public static Cohort Create(
        string name, 
        Guid createdByUserId,
        string parishName, 
        DateTime? startDate = null, 
        DateTime? endDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CohortException("Cohort name cannot be empty");

        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new CohortException("Start date cannot be after end date");

        return new Cohort
        {
            Name = name,
            CreatedByUserId = createdByUserId,
            ParishName =  parishName,
            StartDate = startDate,
            EndDate = endDate,
            IsActive = true
        };
    }

    // Business logic methods
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new CohortException("Cohort name cannot be empty");

        Name = name;
        UpdateTimestamp();
    }
    
    public void UpdateParishName(string? parishName)
    {
        ParishName = parishName;
        UpdateTimestamp();
    }

    public void UpdateDates(DateTime? startDate, DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new CohortException("Start date cannot be after end date");

        StartDate = startDate;
        EndDate = endDate;
        UpdateTimestamp();
    }

    public void Archive()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void Reactivate()
    {
        IsActive = true;
        UpdateTimestamp();
    }
    
    public CohortEvent AddEvent(
        string title,
        DateTime eventDate,
        CohortEventType type,
        string? description = null,
        bool isRequired = false)
    {
        var cohortEvent = CohortEvent.Create(Id, title, eventDate, type, description, isRequired);
        _events.Add(cohortEvent);
        UpdateTimestamp();
        return cohortEvent;
    }

    // Membership management
    public CohortMembership AddMember(
        Guid userId, 
        CohortRole role, 
        ParticipantType? participantType = null,
        Guid? sponsorUserId = null)
    {
        if (_memberships.Any(m => m.UserId == userId))
            throw new CohortException("User is already a member of this cohort");

        var membership = CohortMembership.Create(Id, userId, role, participantType, sponsorUserId);
        _memberships.Add(membership);
        UpdateTimestamp();
        
        return membership;
    }

    public void WithdrawMember(Guid membershipId)
    {
        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership == null)
            throw new CohortException("User is not a member of this cohort");

        membership.Withdraw();
        UpdateTimestamp();
    }

    public CohortMembership ReactivateMember(Guid membershipId)
    {
        var membership = _memberships.FirstOrDefault(m => m.Id == membershipId);
        if (membership == null)
            throw new CohortException("User is not a member of this cohort");
    
        membership.Reactivate();
        UpdateTimestamp();
        return membership;
    }

    public CohortMembership? GetMembership(Guid userId)
    {
        return _memberships.FirstOrDefault(m => m.UserId == userId);
    }

    public IEnumerable<CohortMembership> GetMembersByRole(CohortRole role)
    {
        return _memberships.Where(m => m.Role == role && m.Status == MembershipStatus.Active);
    }
}