using RockPaperScissors_Bll.Enums;
using RockPaperScissors_Bll.Models;
using RockPaperScissors_Dll;
using RockPaperScissors_Dll.Entities;
using RockPaperScissors_Dll.Enums;

namespace RockPaperScissors_Bll.Services
{
    public class UserService : IUserService
    {
        public DbContext _dbContext { get; }

        public UserService(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static readonly Func<UserEntity, UserModel> UserSelector = u => new UserModel
        {
            Id = u.Id,
            Name = u.Name,
            Password = u.Password,
            Won = u.Won,
            Lost = u.Lost,
            Draw = u.Draw,
        };

        public UserModel? GetUser(int id)
        {
            return _dbContext.Users.Where(user => user.Id == id).Select(UserSelector).SingleOrDefault();
        }

        public UserModel? CheckUser(string username, string password)
        {
            var entity = _dbContext.GetByNameAndPassword(username, password);
            if (entity != null)
                return UserSelector(entity);

            if (_dbContext.GetByName(username) != null)
                return null;

            try
            {
                var created = _dbContext.AddUserIfNameFree(username, password);
                return UserSelector(created);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public void SetGameResult(int userId, GameResult gameResult)
        {
            _dbContext.SetUserGameResult(userId, gameResult);
        }
    }
}
