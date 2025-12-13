using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Dll.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors_Bll.Models
{
    public class ApiGameStatusModel
    {
        public GameStatusModel? GameStatus { get; set; }
        public UserStatsModel? UserStats { get; set; }
        public string? Message { get; set; }
    }
}
