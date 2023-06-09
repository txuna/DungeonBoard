using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Room
{
    public class DeleteRoomRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public int RoomId { get; set; }
    }

    public class DeleteRoomResponse
    {
        public ErrorCode Result { get; set; }
    }
}
