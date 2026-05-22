using System.Security.Claims;
using Application.DTOs.Leave;
using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/leave")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(LeaveRequestResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LeaveRequestResponseDto>> Create(
        [FromBody] CreateLeaveRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!TryGetCallerContext(out var userId, out var tenantId))
        {
            return Unauthorized();
        }

        try
        {
            var response = await _leaveService.CreateLeaveRequestAsync(
                request,
                userId,
                tenantId,
                cancellationToken);

            return CreatedAtAction(nameof(GetMyRequests), new { }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpGet("my-requests")]
    [ProducesResponseType(typeof(IReadOnlyList<LeaveRequestResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestResponseDto>>> GetMyRequests(
        CancellationToken cancellationToken)
    {
        if (!TryGetCallerContext(out var userId, out var tenantId))
        {
            return Unauthorized();
        }

        try
        {
            var requests = await _leaveService.GetMyRequestsAsync(
                userId,
                tenantId,
                cancellationToken);

            return Ok(requests);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("{leaveRequestId:guid}/approve")]
    [Authorize(Roles = "Manager,HR")]
    [ProducesResponseType(typeof(LeaveRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeaveRequestResponseDto>> Approve(
        Guid leaveRequestId,
        [FromBody] LeaveActionDto? request,
        CancellationToken cancellationToken)
    {
        return await ProcessLeaveActionAsync(
            leaveRequestId,
            request,
            _leaveService.ApproveLeaveRequestAsync,
            cancellationToken);
    }

    [HttpPost("{leaveRequestId:guid}/reject")]
    [Authorize(Roles = "Manager,HR")]
    [ProducesResponseType(typeof(LeaveRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeaveRequestResponseDto>> Reject(
        Guid leaveRequestId,
        [FromBody] LeaveActionDto? request,
        CancellationToken cancellationToken)
    {
        return await ProcessLeaveActionAsync(
            leaveRequestId,
            request,
            _leaveService.RejectLeaveRequestAsync,
            cancellationToken);
    }

    private async Task<ActionResult<LeaveRequestResponseDto>> ProcessLeaveActionAsync(
        Guid leaveRequestId,
        LeaveActionDto? request,
        Func<Guid, Guid, Guid, string, LeaveActionDto?, CancellationToken, Task<LeaveRequestResponseDto>> action,
        CancellationToken cancellationToken)
    {
        if (!TryGetCallerContext(out var userId, out var tenantId, out var role))
        {
            return Unauthorized();
        }

        try
        {
            var response = await action(
                leaveRequestId,
                userId,
                tenantId,
                role,
                request,
                cancellationToken);

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    private bool TryGetCallerContext(out Guid userId, out Guid tenantId)
    {
        return TryGetCallerContext(out userId, out tenantId, out _);
    }

    private bool TryGetCallerContext(out Guid userId, out Guid tenantId, out string role)
    {
        userId = Guid.Empty;
        tenantId = Guid.Empty;
        role = string.Empty;

        var userIdValue = User.FindFirstValue(AuthService.UserIdClaimType);
        var tenantIdValue = User.FindFirstValue(AuthService.TenantIdClaimType);
        role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        return Guid.TryParse(userIdValue, out userId)
            && Guid.TryParse(tenantIdValue, out tenantId)
            && !string.IsNullOrWhiteSpace(role);
    }
}
