using DungeonBoard.Models;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Game
{
    public class SkillGameRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public int SkillId { get; set; }

    }

    public class SkillGameResponse
    {
        public ErrorCode Result { get; set; }
    }
}
