using System;

namespace WEB_GAME.Models
{
    public class SceneryViewModel
    {
        public int SceneryId { get; set; }
        public string? NameScenery { get; set; }
        public string? DescriptionScenery { get; set; }
        public int? DifficultyLevel { get; set; }
        public string? Location { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Active { get; set; }
    }
}
