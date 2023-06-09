namespace DungeonBoard.Models.Room
{
    public class RedisRoom
    {
        public int RoomId { get; set; }
        public string Title { get; set; }
        public int HeadCount { get; set; }
        public int[] Users { get; set; }
        public int HostUserId { get; set; }
        public int BossId { get; set; }
        public RoomState State { get; set; }
    }
}
