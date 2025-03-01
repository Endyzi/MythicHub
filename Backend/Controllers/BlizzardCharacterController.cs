using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Models;
using System.Reflection.Metadata.Ecma335;


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
        
         if (string.IsNullOrEmpty(region) || string.IsNullOrEmpty(realm) || string.IsNullOrEmpty(characterName))
    {
        return BadRequest("Region, Realm, and Character Name are required.");
    }
        
        var characterJson = await _characterService.GetCharacterDataAsync(region, realm, characterName);

        if (string.IsNullOrEmpty(characterJson))
        {
            return NotFound("Character not found");
        }
        //log json to see if the format is correct.
        Console.WriteLine("Blizzard API response:");
        Console.WriteLine(characterJson); //writes json in terminal

        try
        {
            // Deserialisera JSON from Blizzard API
            var characterData = JsonSerializer.Deserialize<CharacterApiResponse>(characterJson);

            if (characterData == null)
            {
                return StatusCode(500, "Failed to parse character data");
            }

            
            var profile = new CharacterProfile
            {
                Name = characterData.Name ?? "Unknown",
                Realm = characterData.Realm?.Name ?? "Unknown",
                Faction = characterData.Faction?.Name ?? "Unknown",
                Race = characterData.Race?.Name ?? "Unknown",
                CharacterClass = characterData.CharacterClass?.Name ?? "Unknown",
                Specialization = characterData.ActiveSpec?.Name ?? "Unknown",
                Level = characterData.Level,
                AverageItemLevel = characterData.AverageItemLevel,
                EquippedItemLevel = characterData.EquippedItemLevel,
                Title = characterData.ActiveTitle?.Name ?? "",
                CharacterImage = characterData.Media?.Href ?? ""
            };

            return Ok(profile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing character data: {ex.Message}");
        }
    }



    //will fetch top 20 mythic + players with the current highest score
    [HttpGet("top20")]/*This function is incomplete and needs fixing.*/
    public async Task<IActionResult> GetTop20Players([FromQuery] string requestedRegion)
    {
        var accessToken = await _authService.GetAccessTokenAsync();

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("Failed to authenticate with Blizzard API");
        }
        
        var regions = new List<string> { "eu", "us", "cn", "kr", "tw" };
        var allPlayers = new List<MythicPlusEntry>();

        if (!string.IsNullOrEmpty(requestedRegion) && !regions.Contains(requestedRegion))
        {
            return BadRequest("Invalid region specified.");
        }

        var selectedRegions = string.IsNullOrEmpty(requestedRegion) ? regions : new List<string> { requestedRegion };

        foreach (var region in selectedRegions)
        {
            var connectedRealmsUrl = $"https://{region}.api.blizzard.com/data/wow/connected-realm/index?namespace=dynamic-{region}&locale=en_US&access_token={accessToken}";

            var realmsResponse = await _httpClient.GetAsync(connectedRealmsUrl);
            if (!realmsResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch realms for region {region}: {realmsResponse.StatusCode}");
                continue;
            }

            var realmsContent = await realmsResponse.Content.ReadAsStringAsync();
            var connectedRealms = JsonSerializer.Deserialize<ConnectedRealmIndex>(realmsContent);

            if (connectedRealms?.Realms == null) continue;

            foreach (var realm in connectedRealms.Realms)
            {
                var leaderboardIndexUrl = $"https://{region}.api.blizzard.com/data/wow/connected-realm/{realm.Id}/mythic-leaderboard/index?namespace=dynamic-{region}&locale=en_US&access_token={accessToken}";

                var leaderboardResponse = await _httpClient.GetAsync(leaderboardIndexUrl);
                if (!leaderboardResponse.IsSuccessStatusCode) continue;

                var leaderboardContent = await leaderboardResponse.Content.ReadAsStringAsync();
                var leaderboardIndex = JsonSerializer.Deserialize<MythicLeaderboardIndex>(leaderboardContent);

                if (leaderboardIndex?.CurrentLeaderboards == null) continue;

                foreach (var leaderboard in leaderboardIndex.CurrentLeaderboards)
                {
                    var dungeonLeaderboardUrl = leaderboard.Key.Href + $"?access_token={accessToken}";

                    var dungeonResponse = await _httpClient.GetAsync(dungeonLeaderboardUrl);
                    if (!dungeonResponse.IsSuccessStatusCode) continue;

                    var dungeonContent = await dungeonResponse.Content.ReadAsStringAsync();
                    var dungeonLeaderboard = JsonSerializer.Deserialize<MythicPlusLeaderboard>(dungeonContent);

                    if (dungeonLeaderboard?.Entries != null)
                    {
                        allPlayers.AddRange(dungeonLeaderboard.Entries);
                    }
                }
            }
        }

        if (!allPlayers.Any())
        {
            return NotFound("No Mythic+ Leaderboard data found.");
        }

        var topPlayers = allPlayers
            .OrderByDescending(p => p.MythicScore)
            .Take(20)
            .Select(p => new
            {
                Name = p.Character.Name,
                Realm = p.Character.Realm.Slug,
                MythicScore = p.MythicScore
            });

        return Ok(topPlayers);
    }


}


