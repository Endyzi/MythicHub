using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/blizzard")]
public class BlizzardAuthController : ControllerBase
{
    private readonly BlizzardAuthService _authService;

    public BlizzardAuthController(BlizzardAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("token")]
    public async Task<IActionResult> GetToken()
    {
        var token = await _authService.GetAccessTokenAsync();
        return Ok(new { access_token = token });
    }
}
