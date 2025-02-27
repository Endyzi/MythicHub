using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;


public class BlizzardCharacterService
{
    private readonly HttpClient _httpClient;
    private readonly BlizzardAuthService _authService;
    private readonly ILogger<BlizzardCharacterService> _logger;

    public BlizzardCharacterService(HttpClient httpClient, BlizzardAuthService authService, ILogger<BlizzardCharacterService> logger)
    {
        _httpClient = httpClient;
        _authService = authService;
        _logger = logger;
    }

    public async Task<string> GetCharacterDataAsync(string region, string realm, string characterName)
    {
        var accessToken = await _authService.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken))
        {
            _logger.LogError("Failed to retrieve OAuth token.");
            return "Error: Unable to authenticate with Blizzard API";
        }


        characterName = characterName.ToLower();
        realm = realm.ToLower();

       
        var url = $"https://{region}.api.blizzard.com/profile/wow/character/{realm}/{characterName}?namespace=profile-eu&locale=en_US&access_token={accessToken}";


        _logger.LogInformation($"Fetching character data from: {url}");


        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Blizzard API request failed: {response.StatusCode}");
            return $"Error: Blizzard API request failed with status {response.StatusCode}";
        }

        return await response.Content.ReadAsStringAsync();
    }
}
