namespace WEB_GAME.Models
{
    public class BatchViewModel
    {
        public int BatchId { get; set; }

        public string? Location { get; set; }

        public int? Size { get; set; }

        public string? SoilType { get; set; }

        public string? Description { get; set; }

        public bool Active { get; set; }
    }
}
