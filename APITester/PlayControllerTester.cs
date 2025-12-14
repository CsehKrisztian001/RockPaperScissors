using Moq;
using RockPaperScissors.Controllers;
using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;
using RockPaperScissors_Bll.Services;
using RockPaperScissors_Dll.Enums;

namespace APITester
{
    public class PlayControllerTester
    {
        [Fact]
        public void Get_InvalidUser_ReturnsErrorMessage()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var playServiceMock = new Mock<IPlayService>();

            userServiceMock
                .Setup(s => s.CheckUser("baduser", "badpass"))
                .Returns((UserModel?)null);

            var controller = new PlayController(
                userServiceMock.Object,
                playServiceMock.Object
            );

            // Act
            var result = controller.Get("baduser", "badpass", "Rock");

            // Assert
            Assert.Null(result.GameStatus);
            Assert.Null(result.UserStats);
            Assert.Equal("Hibás felhasználónév vagy jelszó.", result.Message);
        }

        [Fact]
        public void Get_ValidUser_ReturnsUserStatus()
        {
            // Arrange
            var user = new UserModel
            {
                Id = 1,
                Name = "test",
                Password = "123",
                Won = 0,
                Lost = 0,
                Draw = 0
            };

            var userServiceMock = new Mock<IUserService>();
            var playServiceMock = new Mock<IPlayService>();

            userServiceMock.Setup(s => s.CheckUser("test", "123"))
                .Returns(user);

            var controller = new PlayController(
                userServiceMock.Object,
                playServiceMock.Object
            );

            // Act
            var result = controller.Get("test", "123", "Stats");

            // Assert
            Assert.Null(result.GameStatus);
            Assert.NotNull(result.UserStats);
            Assert.Equal(user.Won, result.UserStats.Won);
            Assert.Equal(user.Lost, result.UserStats.Lost);
            Assert.Equal(user.Draw, result.UserStats.Draw);
            Assert.Equal("Ok.", result.Message);
        }

        [Fact]
        public void Get_ValidUser_InvalidChoice_ReturnsErrorMessage()
        {
            // Arrange
            var user = new UserModel
            {
                Id = 1,
                Name = "test",
                Password = "123",
                Won = 0,
                Lost = 0,
                Draw = 0
            };

            var userServiceMock = new Mock<IUserService>();
            var playServiceMock = new Mock<IPlayService>();

            userServiceMock.Setup(s => s.CheckUser("test", "123"))
                .Returns(user);

            var controller = new PlayController(
                userServiceMock.Object,
                playServiceMock.Object
            );

            // Act
            var result = controller.Get("test", "123", "BadChoice");

            // Assert
            Assert.Null(result.GameStatus);
            Assert.Null(result.UserStats);
            Assert.Equal("Hibás választás! (Rock, Paper, Scissors)", result.Message);
        }

        [Theory]
        [InlineData(GameChoice.Rock, GameResult.Won, GameChoice.Scissors, 1, 0, 0)]
        [InlineData(GameChoice.Paper, GameResult.Lost, GameChoice.Scissors, 0, 1, 0)]
        [InlineData(GameChoice.Scissors, GameResult.Draw, GameChoice.Scissors, 0, 0, 1)]
        [InlineData(GameChoice.Rock, GameResult.Lost, GameChoice.Paper, 0, 1, 0)]
        [InlineData(GameChoice.Paper, GameResult.Draw, GameChoice.Paper, 0, 0, 1)]
        [InlineData(GameChoice.Scissors, GameResult.Won, GameChoice.Paper, 1, 0, 0)]
        [InlineData(GameChoice.Rock, GameResult.Draw, GameChoice.Rock, 0, 0, 1)]
        [InlineData(GameChoice.Paper, GameResult.Won, GameChoice.Rock, 1, 0, 0)]
        [InlineData(GameChoice.Scissors, GameResult.Lost, GameChoice.Rock, 0, 1, 0)]
        public void Get_ValidUser_ReturnsGameResult(GameChoice playerChoice, GameResult gameResult, GameChoice opponentChoice, int won, int lost, int draw)
        {
            // Arrange
            var user = new UserModel
            {
                Id = 1,
                Name = "test",
                Password = "123",
                Won = 0,
                Lost = 0,
                Draw = 0
            };

            var userServiceMock = new Mock<IUserService>();
            var playServiceMock = new Mock<IPlayService>();

            userServiceMock.Setup(s => s.CheckUser("test", "123"))
                .Returns(user);

            userServiceMock.Setup(s => s.GetUser(user.Id))
                .Returns(user);

            playServiceMock.Setup(s => s.Play(playerChoice))
                .Returns(new GameStatusModel
                {
                    GameResult = gameResult,
                    OpponentChoice = opponentChoice
                });

            userServiceMock.Setup(s => s.SetGameResult(user.Id, gameResult))
                .Callback(() =>
                {
                    switch (gameResult)
                    {
                        case GameResult.Won:
                            user.Won = won;
                            break;
                        case GameResult.Lost:
                            user.Lost = lost;
                            break;
                        case GameResult.Draw:
                            user.Draw = draw;
                            break;
                        default:
                            break;
                    }
                });

            var controller = new PlayController(
                userServiceMock.Object,
                playServiceMock.Object
            );

            // Act
            var result = controller.Get("test", "123", playerChoice.ToString());

            // Assert
            Assert.NotNull(result.GameStatus);
            Assert.NotNull(result.UserStats);
            Assert.Equal(gameResult, result.GameStatus.GameResult);
            Assert.Equal(opponentChoice, result.GameStatus.OpponentChoice);
            Assert.Equal(won, result.UserStats.Won);
            Assert.Equal(lost, result.UserStats.Lost);
            Assert.Equal(draw, result.UserStats.Draw);
            Assert.Equal("Ok.", result.Message);
        }
    }
}