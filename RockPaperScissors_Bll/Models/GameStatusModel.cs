using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Dll.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockPaperScissors_Bll.Models
{
    public class GameStatusModel
    {
        public GameResult GameResult { get; set; }
        public GameChoice OpponentChoice { get; set; }
    }
}
