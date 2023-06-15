using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Game
{
    public class LevelupGameRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public int GameId { get; set; }

        
    }

    public class LevelupGameResponse
    {
        public ErrorCode Result { get; set; }
    }
}
