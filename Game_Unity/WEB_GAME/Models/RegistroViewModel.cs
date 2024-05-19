namespace WEB_GAME.Models
{
    public class RegistroViewModel
    {
        public int UserId { get; set; }
        public required string NameUser { get; set; }
        public required string Password { get; set; }
        public required string EMail { get; set; }
        public int? Level { get; set; }
        public int? Experience { get; set; }
        public int? Score { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? Departure { get; set; }
        public bool Active { get; set; }

    }
}
