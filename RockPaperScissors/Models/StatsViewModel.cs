using RockPaperScissors_Bll.Models;

namespace RockPaperScissors.Models
{
    public class StatsViewModel
    {
        public UserModel User { get; set; } = null!;
        public string? LastGameStatus { get; set; }
    }
}
