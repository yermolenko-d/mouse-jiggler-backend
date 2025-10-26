using Microsoft.AspNetCore.Mvc;
using MouseJigglerBackend.Core.DTOs;
using MouseJigglerBackend.Core.Interfaces;

namespace MouseJigglerBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IActivationKeyService _activationKeyService;

    public HomeController(IActivationKeyService activationKeyService) {
        _activationKeyService = activationKeyService;
    }

    [HttpGet]
    public IActionResult Get() {
        return Ok(new { message = "Virtual Mouse Jiggler API is running", version = "2.0.1" });
    }

    [HttpPost("check-activation-key")]
    public async Task<IActionResult> CheckActivationKey([FromBody] ActivationKeyValidationRequest request) {
        try {
            var result = await _activationKeyService.ValidateActivationKeyAsync(request);
            return Ok(result);
        } catch (Exception ex) {
            return StatusCode(500, new ActivationKeyValidationResponse {
                Valid = false,
                Error = $"Internal server error: {ex.Message}"
            });
        }
    }
}
