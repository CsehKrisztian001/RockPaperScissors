using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;
using RockPaperScissors_Bll.Services;
using System.Reflection;

namespace RockPaperScissors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPlayService _playService;

        public PlayController(IUserService userService, IPlayService playService)
        {
            _userService = userService;
            _playService = playService;
        }

        /// <summary>
        /// Kő, papír, olló API játék.
        /// </summary>
        /// <param name="userName">Felhasználó név.</param>
        /// <param name="password">Jelszó.</param>
        /// <param name="choice">Rock, Paper, Scissors, Stats</param>
        /// <returns></returns>
        [HttpGet(Name = "GetPlay")]
        public ApiGameStatusModel Get(string userName, string password, string choice)
        {
            UserModel? user = _userService.CheckUser(userName, password);

            if (user == null)
            {
                return new ApiGameStatusModel() { GameStatus = null, UserStats = null, Message = "Hibás felhasználónév vagy jelszó." };
            }

            if (choice == "Stats")
            {
                return new ApiGameStatusModel() { GameStatus = null, UserStats = new UserStatsModel() { Won = user.Won, Lost = user.Lost, Draw = user.Draw }, Message = "Ok." };
            }

            if (!Enum.TryParse<GameChoice>(choice, true, out var playerChoice))
            {
                return new ApiGameStatusModel() { GameStatus = null, UserStats = null, Message = "Hibás választás! (Rock, Paper, Scissors)" };
            }

            GameStatusModel gameStatus = _playService.Play(playerChoice);

            _userService.SetGameResult(user.Id, gameStatus.GameResult);

            user = _userService.GetUser(user.Id);

            return new ApiGameStatusModel() { GameStatus = gameStatus, UserStats = new UserStatsModel() { Won = user.Won, Lost = user.Lost, Draw = user.Draw }, Message = "Ok." };
        }
    }
}
