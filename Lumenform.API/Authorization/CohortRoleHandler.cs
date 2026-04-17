using Lumenform.Application.Interfaces;
using LumenformCore.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace LumenformCore.Authorization;

public class CohortRoleHandler : AuthorizationHandler<CohortRoleRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICohortAuthorizationService _authService;

    public CohortRoleHandler(
        IHttpContextAccessor httpContextAccessor,
        ICohortAuthorizationService authService)
    {
        _httpContextAccessor = httpContextAccessor;
        _authService = authService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CohortRoleRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var cohortIdObj = httpContext.Request.RouteValues.GetValueOrDefault("cohortId");
        if (!Guid.TryParse(cohortIdObj?.ToString(), out var cohortId))
        {
            context.Fail();
            return;
        }

        Guid userId;
        try
        {
            userId = AuthHelper.GetCurrentUserId(context.User);
        }
        catch (UnauthorizedAccessException)
        {
            context.Fail();
            return;
        }

        // Check based on required role
        bool hasPermission = requirement.Role switch
        {
            RequiredCohortRole.Owner => 
                await _authService.IsOwner(cohortId, userId),
            
            RequiredCohortRole.Coordinator => 
                await _authService.IsOwner(cohortId, userId) || 
                await _authService.IsCoordinator(cohortId, userId),
            
            RequiredCohortRole.Member => 
                await _authService.IsMember(cohortId, userId),
            
            _ => false
        };

        if (hasPermission)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}