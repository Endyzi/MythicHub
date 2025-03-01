using System.Text.Json.Serialization;

namespace Backend.Models
{
  public class CharacterApiResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("realm")]
    public CharacterRealmInfo Realm { get; set; }

    [JsonPropertyName("faction")]
    public FactionInfo Faction { get; set; }

    [JsonPropertyName("race")]
    public RaceInfo Race { get; set; }

    [JsonPropertyName("character_class")]
    public CharacterClassInfo CharacterClass { get; set; }

    [JsonPropertyName("active_spec")]
    public SpecInfo ActiveSpec { get; set; }

    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("average_item_level")]
    public int AverageItemLevel { get; set; }

    [JsonPropertyName("equipped_item_level")]
    public int EquippedItemLevel { get; set; }

    [JsonPropertyName("active_title")]
    public TitleInfo ActiveTitle { get; set; }

    [JsonPropertyName("media")]
    public MediaInfo Media { get; set; }

     [JsonPropertyName("gender")]
    public GenderInfo Gender { get; set; }
}

public class CharacterRealmInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class FactionInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class RaceInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }
}

public class CharacterClassInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class SpecInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class TitleInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class MediaInfo
{
    [JsonPropertyName("href")]
    public string Href { get; set; }
}

public class GenderInfo
{
    [JsonPropertyName("type")]
    public string Type { get; set; } 

    [JsonPropertyName("name")]
    public string Name { get; set; } 
}

}
