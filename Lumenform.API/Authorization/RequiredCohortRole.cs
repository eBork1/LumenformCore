namespace LumenformCore.Authorization;

public enum RequiredCohortRole
{
    Member,       // Any member (owner/coordinator/participant/sponsor)
    Coordinator,  // Owner OR Coordinator
    Owner         // Only owner
}