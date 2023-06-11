using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Room
{
    public class CreateRoomRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]  
        public string AuthToken { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int HeadCount { get; set; }
        [Required]
        public int BossId { get; set; }
    }

    public class CreateRoomResponse
    {
        public ErrorCode Result { get; set; }
        public int RoomId { get; set; }
    }
}
