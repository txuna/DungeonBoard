using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Room
{
    public class LoadRoomRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }
    }

    public class LoadRoomResponse
    {
        public ErrorCode Result { get; set; }
    }
}
