using Microsoft.AspNetCore.Authorization;

namespace LumenformCore.Authorization;

public class CohortRoleRequirement : IAuthorizationRequirement
{
    public RequiredCohortRole Role { get; }

    public CohortRoleRequirement(RequiredCohortRole role)
    {
        Role = role;
    }
}