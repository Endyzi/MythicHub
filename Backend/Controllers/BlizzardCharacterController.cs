using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/blizzard")]//api route to test if the api is working to fetch certain character
public class BlizzardCharacterController : ControllerBase
{
    private readonly BlizzardCharacterService _characterService;
    private readonly BlizzardAuthService _authService;
    private readonly HttpClient _httpClient;
    

    public BlizzardCharacterController(
        BlizzardCharacterService characterService,
        BlizzardAuthService authService,
        HttpClient httpClient)
    {
        _characterService = characterService;
        _authService = authService;
        _httpClient = httpClient;
    }

    [HttpGet("character/{region}/{realm}/{characterName}")]
    public async Task<IActionResult> GetCharacter(string region, string realm, string characterName)
    {
        var characterData = await _characterService.GetCharacterDataAsync(region, realm, characterName);
        return Ok(characterData);
    }


//[HttpGet("search/{region}/{query}")]
//search for specific character via blizzard api, will use this to populate list in frontend.
/*public async Task<IActionResult> SearchCharacter(string region, string query)
{
    var accessToken = await _authService.GetAccessTokenAsync();
    if (string.IsNullOrEmpty(accessToken))
    {
        return Unauthorized("Failed to authenticate with Blizzard API.");
    }

    var url = $"https://{region}.api.blizzard.com/profile/wow/character/{query}?namespace=profile-{region}&locale=en_GB&access_token={accessToken}";

    var response = await _httpClient.GetAsync(url);
    if (!response.IsSuccessStatusCode)
    {
        return NotFound("Character not found.");
    }

    var content = await response.Content.ReadAsStringAsync();
    return Ok(JsonSerializer.Deserialize<object>(content));
}*/

}


