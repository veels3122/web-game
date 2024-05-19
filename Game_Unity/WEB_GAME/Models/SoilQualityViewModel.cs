using System.ComponentModel.DataAnnotations;

namespace WEB_GAME.Models
{
    public class SoilQualityViewModel
    {
        public int SoilQualityId { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }
    }
}
