namespace WEB_GAME.Models
{
    public class CropViewModel
    {
        public int CropId { get; set; }

        public int? BatchId { get; set; }

        public int? UserId { get; set; }

        public string? TypeCrop { get; set; }

        public string? GreetingStatus { get; set; }

        public string? StageGrowth { get; set; }

        public int? ClimaticConditionID { get; set; }

        public int? SoilQualityID { get; set; }

        public bool Active { get; set; }
    }
}
