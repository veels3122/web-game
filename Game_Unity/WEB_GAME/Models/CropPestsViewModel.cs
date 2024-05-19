using System.ComponentModel.DataAnnotations.Schema;

namespace WEB_GAME.Models
{
    public class CropPestsViewModel
    {
        [ForeignKey("CropId")]
        public int? CropId { get; set; }

        [ForeignKey("PlagueId")]
        public int? PlagueId { get; set; }

        public bool Active { get; set; }
    }
}
