using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Game;
using DungeonBoard.Models.Master;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    [ApiController]
    public class LevelupGameController : Controller
    {

        IMemoryDB _memoryDB; 
        IMasterDB _masterDB;
        public LevelupGameController(IMemoryDB memoryDB, IMasterDB masterDB)
        {
            _memoryDB = memoryDB;
            _masterDB = masterDB;
        }

        [Route("/Game/Levelup")]
        [HttpPost]
        async public Task<LevelupGameResponse> GameLevelup(LevelupGameRequest levelupGameRequest)
        {
            // 해당 플레이어가 게임 중이고 해당 방이 게임상태라면 
            RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
            if (redisUser.State != UserState.Playing)
            {
                return new LevelupGameResponse
                {
                    Result = ErrorCode.PlayerIsNotPlaying
                };
            }

            var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(levelupGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new LevelupGameResponse
                {
                    Result = Result
                };
            }

            if (redisRoom.State != RoomState.Playing)
            {
                return new LevelupGameResponse
                {
                    Result = ErrorCode.IsNotPlayingThisRoom
                };
            }

            RedisGame? redisGame;
            (Result, redisGame) = await _memoryDB.LoadRedisGame(levelupGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new LevelupGameResponse
                {
                    Result = Result
                };
            }

            if (redisGame.WhoIsTurn.UserId != levelupGameRequest.UserId)
            {
                return new LevelupGameResponse
                {
                    Result = ErrorCode.IsNotDiceTurn
                };
            }

            Result = await UpdateRedisGame(redisGame, levelupGameRequest.UserId);

            return new LevelupGameResponse
            {
                Result = Result
            };
        }

        async Task<ErrorCode> UpdateRedisGame(RedisGame redisGame, int userId)
        {
            // Backup 덮기
            for (int i = 0; i < redisGame.Players.Length; i++)
            {
                if (redisGame.Players[i].UserId == userId)
                {
                    MasterClassLevelupStat? masterClassLevelupStat = _masterDB.LoadClassLevelupStat(redisGame.Players[i].ClassId);
                    if(masterClassLevelupStat == null)
                    {
                        return ErrorCode.NoneExistClassId;
                    }

                    redisGame.Players[i].MaxHp += masterClassLevelupStat.Hp;
                    redisGame.Players[i].MaxMp += masterClassLevelupStat.Mp;
                    redisGame.Players[i].Attack += masterClassLevelupStat.Attack;
                    redisGame.Players[i].Defence += masterClassLevelupStat.Defence;
                    redisGame.Players[i].Magic += masterClassLevelupStat.Magic;
                    redisGame.Players[i].Level += 1;

                    break;
                }
            }

            // Set WhoIsTurn 
            if (redisGame.WhoIsTurn.Index + 1 >= redisGame.Players.Length)
            {
                redisGame.WhoIsTurn.Index = 0;
            }
            else
            {
                redisGame.WhoIsTurn.Index += 1;
            }

            redisGame.WhoIsTurn.UserId = redisGame.Players[redisGame.WhoIsTurn.Index].UserId;

            return await _memoryDB.StoreRedisGame(redisGame);
        }
    }
}
