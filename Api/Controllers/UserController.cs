using System.Security.Claims;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserProfileResponse> GetProfile()
    {
        if (!TryGetUserClaims(out var userId, out var email, out var role))
        {
            return Unauthorized();
        }

        return Ok(new UserProfileResponse(userId, email, role));
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetAdmin()
    {
        return Ok(new { message = "Admin access granted." });
    }

    [HttpGet("manager")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetManager()
    {
        return Ok(new { message = "Manager access granted." });
    }

    private bool TryGetUserClaims(out string userId, out string email, out string role)
    {
        userId = User.FindFirstValue(AuthService.UserIdClaimType) ?? string.Empty;
        email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        return !string.IsNullOrWhiteSpace(userId)
            && !string.IsNullOrWhiteSpace(email)
            && !string.IsNullOrWhiteSpace(role);
    }

    public sealed record UserProfileResponse(string UserId, string Email, string Role);
}
