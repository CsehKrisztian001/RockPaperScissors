using Newtonsoft.Json;
using RockPaperScissors_Dll.Entities;
using RockPaperScissors_Dll.Enums;

namespace RockPaperScissors_Dll
{
    public class DbContext
    {
        private const string _userFilename = "users.json";
        private static readonly object _lock = new();

        private List<UserEntity> _users;

        public List<UserEntity> Users { get { return _users; } }

        public DbContext()
        {
            Load();
        }

        private void Load()
        {
            lock (_lock)
            {
                if (File.Exists(_userFilename))
                {
                    _users = JsonConvert.DeserializeObject<List<UserEntity>>(File.ReadAllText(_userFilename))
                             ?? new List<UserEntity>();
                }
                else
                {
                    _users = new List<UserEntity>();
                    Save();
                }
            }
        }

        private void Save()
        {
            File.WriteAllText(_userFilename, JsonConvert.SerializeObject(_users, Formatting.Indented));
        }

        public UserEntity? GetByNameAndPassword(string username, string password)
        {
            lock (_lock)
                return _users.SingleOrDefault(u => u.Name == username && u.Password == password);
        }

        public UserEntity? GetByName(string username)
        {
            lock (_lock)
                return _users.SingleOrDefault(u => u.Name == username);
        }

        public UserEntity AddUserIfNameFree(string username, string password)
        {
            lock (_lock)
            {
                if (_users.Any(u => u.Name == username))
                    throw new InvalidOperationException("User already exists.");

                int nextId = _users.Count == 0 ? 1 : _users.Max(u => u.Id) + 1;

                var newUser = new UserEntity
                {
                    Id = nextId,
                    Name = username,
                    Password = password,
                    Won = 0,
                    Lost = 0,
                    Draw = 0
                };

                _users.Add(newUser);

                Save();

                return newUser;
            }
        }

        public void SetUserGameResult(int userId, GameResult gameResult)
        {
            lock (_lock)
            {
                UserEntity user = _users.Single(user => user.Id == userId);

                switch (gameResult)
                {
                    case GameResult.Won:
                        user.Won++;
                        break;
                    case GameResult.Lost:
                        user.Lost++;
                        break;
                    case GameResult.Draw:
                        user.Draw++;
                        break;
                    default:
                        break;
                }

                Save();
            }
        }
    }
}
