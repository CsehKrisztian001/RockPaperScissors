using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;

namespace RockPaperScissors_Bll.Services
{
    public interface IPlayService
    {
        GameStatusModel Play(GameChoice playerChoice);
    }
}
