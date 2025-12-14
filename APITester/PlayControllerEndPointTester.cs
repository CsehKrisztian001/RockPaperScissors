using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;
using RockPaperScissors_Bll.Services;
using RockPaperScissors_Dll.Enums;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace APITester
{
    public class ApiFactory : WebApplicationFactory<RockPaperScissors.Program>
    {
        private readonly GameChoice _playerChoice;
        private readonly GameStatusModel _gameStatus;
        private readonly UserModel _user;

        public ApiFactory(GameChoice playerChoice, GameStatusModel gameStatus, UserModel user)
        {
            _playerChoice = playerChoice;
            _gameStatus = gameStatus;
            _user = user;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IUserService>();
                services.RemoveAll<IPlayService>();

                var userServiceMock = new Mock<IUserService>();
                var playServiceMock = new Mock<IPlayService>();

                userServiceMock.Setup(s => s.CheckUser("test", "123"))
                    .Returns(_user);

                userServiceMock.Setup(s => s.GetUser(_user.Id))
                    .Returns(_user);

                playServiceMock.Setup(s => s.Play(_playerChoice))
                .Returns(_gameStatus);

                userServiceMock.Setup(s => s.SetGameResult(_user.Id, _gameStatus.GameResult))
                    .Callback<int, GameResult>((id, result) =>
                    {
                        switch (result)
                        {
                            case GameResult.Won: _user.Won++; break;
                            case GameResult.Lost: _user.Lost++; break;
                            case GameResult.Draw: _user.Draw++; break;
                        }
                    });

                services.AddScoped(_ => userServiceMock.Object);
                services.AddScoped(_ => playServiceMock.Object);
            });
        }
    }

    public class PlayControllerEndPointTester
    {
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
        public async Task Play_Endpoint_ReturnsExpected(GameChoice playerChoice, GameResult gameResult, GameChoice opponentChoice, int won, int lost, int draw)
        {
            var user = new UserModel
            {
                Id = 1,
                Name = "test",
                Password = "123",
                Won = 0,
                Lost = 0,
                Draw = 0
            };

            var factory = new ApiFactory(
                playerChoice, 
                new GameStatusModel { GameResult = gameResult, OpponentChoice = opponentChoice }, 
                user);

            var client = factory.CreateClient();

            var resp = await client.GetAsync($"/api/Play?userName=test&password=123&choice={playerChoice}");
            resp.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new JsonStringEnumConverter());

            var value = await resp.Content.ReadFromJsonAsync<ApiGameStatusModel>(options);

            Assert.NotNull(value);
            Assert.Equal("Ok.", value!.Message);

            Assert.NotNull(value.GameStatus);
            Assert.Equal(gameResult, value.GameStatus!.GameResult);
            Assert.Equal(opponentChoice, value.GameStatus!.OpponentChoice);

            Assert.NotNull(value.UserStats);
            Assert.Equal(won, value.UserStats!.Won);
            Assert.Equal(lost, value.UserStats!.Lost);
            Assert.Equal(draw, value.UserStats!.Draw);
        }
    }
}
