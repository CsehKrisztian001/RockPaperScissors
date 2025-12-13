using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RockPaperScissors.Models;
using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;
using RockPaperScissors_Bll.Services;
using System.Diagnostics;

namespace RockPaperScissors.Controllers
{
    public class HomeController : Controller
    {
        private IUserService _userService;
        private IPlayService _playService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IUserService userService, IPlayService playService)
        {
            _logger = logger;
            _userService = userService;
            _playService = playService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            UserModel? user = _userService.CheckUser(model.UserName, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Hibás felhasználónév vagy jelszó.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            return RedirectToAction("Stats");
        }

        public IActionResult Stats()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var lastGameStatus = HttpContext.Session.GetString("LastGameStatus");

            if (userId == null)
                return RedirectToAction("Index");

            var user = _userService.GetUser(userId.Value);
            if (user == null)
            {
                HttpContext.Session.Remove("UserId");
                return RedirectToAction("Index");
            }

            StatsViewModel stats = new StatsViewModel() { User = user, LastGameStatus = lastGameStatus };

            return View(stats);
        }

        [HttpPost]
        public IActionResult Play(string choice)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Index");

            GameChoice playerChoice = (GameChoice)Enum.Parse(typeof(GameChoice), choice, true);

            GameStatusModel gameStatus = _playService.Play(playerChoice);

            HttpContext.Session.SetString("LastGameStatus", $"{playerChoice} - {gameStatus.OpponentChoice} / {gameStatus.GameResult}");

            _userService.SetGameResult((int)userId, gameStatus.GameResult);

            return RedirectToAction("Stats");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
