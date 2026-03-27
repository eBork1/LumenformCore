using System.Security.Claims;
using LumenformCore.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LumenformCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        return AuthHelper.GetCurrentUserId(User);
    }

    protected string GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value 
               ?? User.FindFirst("email")?.Value 
               ?? throw new UnauthorizedAccessException("Email not found in token");
    }
}