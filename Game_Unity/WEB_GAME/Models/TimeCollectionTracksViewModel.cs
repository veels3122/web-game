using System;
using System.ComponentModel.DataAnnotations;

namespace WEB_GAME.Models
{
    public class TimeCollectionTracksViewModel
    {
        public int TimeCollectionTracksId { get; set; }

        [Display(Name = "ID del Detective")]
        [Required(ErrorMessage = "El campo ID del Detective es requerido")]
        public int DetectiveId { get; set; }

        [Display(Name = "ID del Juego")]
        [Required(ErrorMessage = "El campo ID del Juego es requerido")]
        public int GameId { get; set; }

        [Display(Name = "ID de Pistas del Detective")]
        [Required(ErrorMessage = "El campo ID de Pistas del Detective es requerido")]
        public int DetectiveCluesId { get; set; }

        [Display(Name = "Tiempo de las Pistas")]
        public DateTime? TimeClues { get; set; }

        [Display(Name = "Activo")]
        public bool Active { get; set; }
    }
}
