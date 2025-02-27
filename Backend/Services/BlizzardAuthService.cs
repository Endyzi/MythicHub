using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class BlizzardAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlizzardAuthService> _logger;
    private string _accessToken = string.Empty;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public BlizzardAuthService(HttpClient httpClient, IConfiguration configuration, ILogger<BlizzardAuthService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _accessToken; // reuse token if valid
        }

        var clientId = _configuration["Blizzard:ClientId"];
        var clientSecret = _configuration["Blizzard:ClientSecret"];
        var authUrl = "https://oauth.battle.net/token";

        var requestBody = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
        var request = new HttpRequestMessage(HttpMethod.Post, authUrl)
        {
            Headers = { { "Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"))}" } },
            Content = requestBody
        };

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to get Blizzard OAuth token. Status code: {response.StatusCode}");
            return string.Empty;
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var json = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        _accessToken = json["access_token"].ToString();
        _tokenExpiry = DateTime.UtcNow.AddSeconds(double.Parse(json["expires_in"].ToString()));

        return _accessToken;
    }
}
