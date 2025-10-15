using Microsoft.AspNetCore.Mvc;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase {
    private readonly IUserService _userService;

    public TestController(IUserService userService) {
        _userService = userService;
    }

    /// <summary>
    /// Gets test users with emails containing 'test1@example.com' and 'test2@example.com'
    /// </summary>
    /// <returns>List of test users</returns>
    [HttpGet("users")]
    public async Task<IActionResult> GetTestUsers() {
        try {
            var emailFilters = new[] { "test1@example.com", "test2@example.com" };
            var users = await _userService.GetUsersByEmailFilterAsync(emailFilters);
            return Ok(new {
                success = true,
                message = $"Found {users.Count()} test users",
                data = users,
                filters = emailFilters
            });
        }
        catch (Exception ex) {
            return StatusCode(500, new {
                success = false,
                message = "An error occurred while retrieving test users",
                error = ex.Message
            });
        }
    }
}