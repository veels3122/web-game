namespace WEB_GAME.Models
{
    public class MaturationMonitoringViewModel
    {
        public int MaturationMonitoringId { get; set; }
        public int? CropId { get; set; }
        public DateTime? EstimatedRipeningDate { get; set; }
        public DateTime? EstimatedPlanningDate { get; set; }
        public string? MaturationState { get; set; }
        public string? Observations { get; set; }
    }
}
