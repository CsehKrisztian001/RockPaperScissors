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

        [Fact]
        public void Get_ValidUser_Rock_ReturnsGameResult()
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

            userServiceMock.Setup(s => s.GetUser(1))
                .Returns(user);

            playServiceMock.Setup(s => s.Play(GameChoice.Rock))
                .Returns(new GameStatusModel
                {
                    GameResult = GameResult.Won
                });

            userServiceMock.Setup(s => s.SetGameResult(1, GameResult.Won))
                .Callback(() =>
                {
                    user.Won++;
                });

            var controller = new PlayController(
                userServiceMock.Object,
                playServiceMock.Object
            );

            // Act
            var result = controller.Get("test", "123", "Rock");

            // Assert
            Assert.NotNull(result.GameStatus);
            Assert.NotNull(result.UserStats);
            Assert.Equal(GameResult.Won, result.GameStatus.GameResult);
            Assert.Equal(1, result.UserStats.Won);
            Assert.Equal(user.Lost, result.UserStats.Lost);
            Assert.Equal(user.Draw, result.UserStats.Draw);
            Assert.Equal("Ok.", result.Message);
        }
    }
}