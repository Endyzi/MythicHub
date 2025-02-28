namespace Backend.Models
{
    using System.Collections.Generic;

    //Model being used by BlizzardCharacterController
    //incomplete, need fixing.
    public class ConnectedRealmIndex
    {
        public List<ConnectedRealm> Realms { get; set; }
    }

    public class ConnectedRealm
    {
        public int Id { get; set; }
    }

    public class MythicLeaderboardIndex
    {
        public List<CurrentLeaderboard> CurrentLeaderboards { get; set; }
    }

    public class CurrentLeaderboard
    {
        public Key Key { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class Key
    {
        public string Href { get; set; }
    }

    public class MythicPlusLeaderboard
    {
        public List<MythicPlusEntry> Entries { get; set; }
    }

    public class MythicPlusEntry
    {
        public CharacterInfo Character { get; set; }
        public double MythicScore { get; set; }
    }

    public class CharacterInfo
    {
        public string Name { get; set; }
        public RealmInfo Realm { get; set; }
    }

    public class RealmInfo
    {
        public string Slug { get; set; }
    }
}
