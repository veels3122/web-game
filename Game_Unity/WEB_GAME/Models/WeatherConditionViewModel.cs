using System.ComponentModel.DataAnnotations;

namespace WEB_GAME.Models
{
    public class WeatherConditionViewModel
    {
        public int WeatherConditionId { get; set; }

        [Display(Name = "Tipo de Clima")]
        public string TypeWeather { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }
    }
}
