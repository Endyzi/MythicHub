namespace Backend.Models
{

    //will probably add more values later on, achievements maybe or something else.
    public class CharacterProfile
    {
        public string Name { get; set; }
        public string Realm { get; set; }
        public string Faction { get; set; }
        public string Race { get; set; }
         public int RaceId { get; set; } 
        public string CharacterClass { get; set; }
        public string Specialization { get; set; }
        public int Level { get; set; }
        public int AverageItemLevel { get; set; }
        public int EquippedItemLevel { get; set; }
        public string Title { get; set; }
        public string CharacterImage { get; set; }
        public string Gender { get; set; }
    }
}
