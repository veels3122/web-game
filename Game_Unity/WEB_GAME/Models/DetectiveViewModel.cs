namespace WEB_GAME.Models
{
    public class DetectiveViewModel
    {
        public int DetectiveId { get; set; }
        public string? NameDetective { get; set; }
        public string? Skills { get; set; }
        public string? Description { get; set; }
        public DateTime? DateCreation { get; set; }
        public bool Active { get; set; }
    }
}
