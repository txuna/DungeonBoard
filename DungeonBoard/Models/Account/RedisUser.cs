namespace DungeonBoard.Models.Account
{
    public class RedisUser
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string AuthToken { get; set; }
        public UserState State { get; set; }
    }
}
