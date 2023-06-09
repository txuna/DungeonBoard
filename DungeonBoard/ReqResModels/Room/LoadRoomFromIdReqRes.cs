using DungeonBoard.Models;
using DungeonBoard.Models.Room;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Room
{
    public class LoadRoomFromIdRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }
        [Required]
        public int RoomId { get; set; }
    }

    public class LoadRoomFromIdResponse
    {
        public ErrorCode Result { get; set; }
        public RedisRoom Room { get; set; }
    }
}
