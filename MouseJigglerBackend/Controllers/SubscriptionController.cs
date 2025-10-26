using Microsoft.AspNetCore.Mvc;
using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IAuthService _authService;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        ISubscriptionService subscriptionService,
        IAuthService authService,
        ILogger<SubscriptionController> logger) {
        _subscriptionService = subscriptionService;
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's subscription information
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User's subscription information</returns>
    [HttpPost("current")]
    [ProducesResponseType(typeof(SubscriptionDto), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 404)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> GetCurrentUserSubscription([FromBody] string token) {
        try {
            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { success = false, message = "Token is required" });
            }
            var user = await _authService.GetUserFromTokenAsync(token);
            if (user == null) {
                return Unauthorized(new { success = false, message = "Invalid or expired token" });
            }
            var subscription = await _subscriptionService.GetUserSubscriptionAsync(user.Id);
            if (subscription == null) {
                return NotFound(new { success = false, message = "No subscription found for this user" });
            }
            return Ok(subscription);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error in get current user subscription endpoint");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Check if current user has active subscription
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Active subscription status</returns>
    [HttpPost("check-active")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<IActionResult> CheckActiveSubscription([FromBody] string token) {
        try {
            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { success = false, message = "Token is required" });
            }
            var user = await _authService.GetUserFromTokenAsync(token);
            if (user == null) {
                return Unauthorized(new { success = false, message = "Invalid or expired token" });
            }
            var hasActiveSubscription = await _subscriptionService.HasActiveSubscriptionAsync(user.Id);
            return Ok(new { hasActiveSubscription });
        } catch (Exception ex) {
            _logger.LogError(ex, "Error in check active subscription endpoint");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }
}
