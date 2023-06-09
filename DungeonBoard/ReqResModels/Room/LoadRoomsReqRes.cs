using DungeonBoard.Models;
using DungeonBoard.Models.Room;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Room
{
    public class LoadRoomsRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }
    }

    public class LoadRoomsResponse
    {
        public ErrorCode Result { get; set; }
        public RedisRoom[]? redisRoom { get; set; }
    }
}
