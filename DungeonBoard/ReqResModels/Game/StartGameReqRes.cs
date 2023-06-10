using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Game
{
    public class StartGameRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public int RoomId { get; set; }
    }

    public class StartGameResponse
    {
        public ErrorCode Result { get; set; }
    }
}
