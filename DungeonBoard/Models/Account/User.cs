namespace DungeonBoard.Models.Account
{
    public class User
    {
        public int Userid { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
