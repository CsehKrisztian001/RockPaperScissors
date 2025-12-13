using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;
using RockPaperScissors_Dll.Enums;

namespace RockPaperScissors_Bll.Services
{
    public class PlayeService : IPlayService
    {
        private Random rnd = new Random();

        public PlayeService() { }

        private GameChoice GetOpponentChoice()
        {
            return (GameChoice)rnd.Next(0, 3);
        }

        public GameStatusModel Play(GameChoice playerChoice)
        {
            GameChoice opponentChoice = GetOpponentChoice();

            if (playerChoice == opponentChoice) return new GameStatusModel { GameResult = GameResult.Draw, OpponentChoice = opponentChoice };
            else
            {
                switch (playerChoice)
                {
                    case GameChoice.Rock:
                        if (opponentChoice == GameChoice.Paper) return new GameStatusModel { GameResult = GameResult.Lost, OpponentChoice = opponentChoice };
                        else return new GameStatusModel { GameResult = GameResult.Won, OpponentChoice = opponentChoice };
                    case GameChoice.Paper:
                        if (opponentChoice == GameChoice.Scissors) return new GameStatusModel { GameResult = GameResult.Lost, OpponentChoice = opponentChoice };
                        else return new GameStatusModel { GameResult = GameResult.Won, OpponentChoice = opponentChoice };
                    case GameChoice.Scissors:
                        if (opponentChoice == GameChoice.Rock) return new GameStatusModel { GameResult = GameResult.Lost, OpponentChoice = opponentChoice };
                        else return new GameStatusModel { GameResult = GameResult.Won, OpponentChoice = opponentChoice };
                    default:
                        break;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(playerChoice), playerChoice, "Invalid game choice");
        }
    }
}
