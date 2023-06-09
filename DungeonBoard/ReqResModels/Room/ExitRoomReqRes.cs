using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Room
{
    public class ExitRoomRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public int RoomId { get; set; }
    }

    public class ExitRoomResponse
    {
        public ErrorCode Result { get; set; }
    }
}
