
using DungeonBoard.Models.Master;
using Microsoft.AspNetCore.Http.Features;

namespace DungeonBoard.Models.Game
{
    public class GamePlayer
    {
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Mp { get; set; }
        public int MaxMp { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int Magic { get; set; }
        public int Level { get; set; }
        public int PositionCard { get; set; }
    }

    public class WhoIsTurn
    {
        public int Index { get; set; }
        public int UserId { get; set; }
    }

    public class RedisGame
    {
        public int GameId { get; set; }
        public MasterBossInfo? BossInfo { get; set; }
        public GamePlayer[]? Players { get; set; }
        public WhoIsTurn? WhoIsTurn { get; set; }
        public int Round { get; set; }
    }
}
