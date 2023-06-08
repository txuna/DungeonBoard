using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Room
{
    public class EnterRoomRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }
        [Required]
        public int RoomId { get; set; }
    }

    public class EnterRoomResponse
    {
        public ErrorCode Result { get; set; }
    }
}
