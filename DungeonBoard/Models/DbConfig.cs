namespace DungeonBoard.Models
{
    public class DbConfig
    {
        public string GameDB { get; set; }
        public string RedisDB { get; set; }
        public string RedisUserKey { get; set; }
        public string RedisRoomKey { get; set; }
        public string RedisUniqueRoomKey { get; set; }
    }
}
