using DungeonBoard.Models;
using DungeonBoard.Models.Game;
using System.ComponentModel.DataAnnotations;

namespace DungeonBoard.ReqResModels.Game
{
    public class LoadGameInfoRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string AuthToken { get; set; }

        [Required]
        public int GameId { get; set; }
    }

    public class LoadGameInfoResponse
    {
        public ErrorCode Result { get; set; }
        public RedisGame? GameInfo { get; set; }
    }

}
