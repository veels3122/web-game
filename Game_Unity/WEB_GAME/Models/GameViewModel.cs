namespace WEB_GAME.Models
{
    public class GameViewModel
    {
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string? DateGame { get; set; }
        public DateTime? StarDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? Duration { get; set; }
        public int? ScenarioId { get; set; }
        public int? Score { get; set; }
        public bool Active { get; set; }

        public string? TransleteNameUser { get; set; } = null;
        public string? TrasleteNameScenarie { get; set; } = null;
    }
}
