using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Game
{
    public class DiceGameRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public int DiceNumber { get; set; }
    }

    public class DiceGameResponse
    {
        public ErrorCode Result { get; set; } 
    }
}
