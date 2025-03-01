using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Models;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Cors;



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
    [EnableCors("AllowFrontend")]
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
                RaceId = characterData.Race?.Id ?? 0,
                CharacterClass = characterData.CharacterClass?.Name ?? "Unknown",
                Specialization = characterData.ActiveSpec?.Name ?? "Unknown",
                Level = characterData.Level,
                AverageItemLevel = characterData.AverageItemLevel,
                EquippedItemLevel = characterData.EquippedItemLevel,
                Title = characterData.ActiveTitle?.Name ?? "",
                CharacterImage = characterData.Media?.Href ?? "",
                Gender = characterData.Gender?.Type ?? "male"
            };

            return Ok(profile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error processing character data: {ex.Message}");
        }
    }


    /*"A resource is blocked by OpaqueResponseBlocking, please check browser console for details." got this error from fetching the image in get character function 
    probably cors error, image url is pointing to another extern url from blizzard, need to bypass it with the function below.
    */
    [HttpGet("character-image")]
    [EnableCors("AllowFrontend")]
    public async Task<IActionResult> GetCharacterImage([FromQuery] string imageUrl)
    {

        Console.WriteLine($"Fetching image from URL: {imageUrl}");

        if (string.IsNullOrEmpty(imageUrl))
        {
            return BadRequest("Image URL is required");
        }

        try
        {
            using var client = new HttpClient();


            var accessToken = await _authService.GetAccessTokenAsync();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch image. Status Code: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, "Failed to fetch image.");
            }


            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Fetched JSON Data" + json);

            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var imageUrlNode = doc.RootElement.GetProperty("assets")[0].GetProperty("value").GetString();

            if (string.IsNullOrEmpty(imageUrlNode))
            {
                Console.WriteLine("No image URL found in JSON data.");
                return NotFound("No image URL found");
            }

            var imageResponse = await client.GetAsync(imageUrlNode);
            if (!imageResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed to fetch image file. Status Code: {imageResponse.StatusCode}");
                return StatusCode((int)imageResponse.StatusCode, "Failed to fetch image file");
            }

            var contentType = imageResponse.Content.Headers.ContentType?.MediaType ?? "image/Jpeg";
            var imageData = await imageResponse.Content.ReadAsByteArrayAsync();

            Console.WriteLine("Image fetched successfully.");
            return File(imageData, contentType);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching image: {ex.Message}");
            return StatusCode(500, $"Error fetching image: {ex.Message}");
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


