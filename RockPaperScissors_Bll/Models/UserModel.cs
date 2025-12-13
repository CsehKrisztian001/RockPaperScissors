namespace RockPaperScissors_Bll.Models
{
    public class UserModel
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; } = null!;
        public string Password { get; internal set; } = null!;
        public int Won { get; internal set; }
        public int Lost { get; internal set; }
        public int Draw { get; internal set; }
    }
}
