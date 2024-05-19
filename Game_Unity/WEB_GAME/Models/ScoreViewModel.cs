using System.ComponentModel.DataAnnotations;

namespace WEB_GAME.Models
{
    public class ScoreViewModel
    {
        public int ScoreId { get; set; }
        public int? UserId { get; set; }
        public int? Score { get; set; }
        public DateTime? DateRegistrer { get; set; }
        public int? Game { get; set; }

        public bool Active { get; set; }

        public string? TraslateName { get; set; } = null;
    }
}
