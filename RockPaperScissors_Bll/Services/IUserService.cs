using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;
using RockPaperScissors_Dll.Enums;

namespace RockPaperScissors_Bll.Services
{
    public interface IUserService
    {
        UserModel? GetUser(int id);
        UserModel? CheckUser(string username, string password);
        void SetGameResult(int userId, GameResult gameResult);
    }
}
