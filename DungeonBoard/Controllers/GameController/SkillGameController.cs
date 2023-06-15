using DungeonBoard.Models.Account;
using DungeonBoard.Models.Game;
using DungeonBoard.Models;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    [ApiController]
    public class SkillGameController : Controller
    {
        IMemoryDB _memoryDB; 
        public SkillGameController(IMemoryDB memoryDB)
        {
            _memoryDB = memoryDB;
        }

        [Route("/Game/Skill")]
        [HttpPost]
        async public Task<SkillGameResponse> GameSkill(SkillGameRequest skillGameRequest)
        {
            // 해당 플레이어가 게임 중이고 해당 방이 게임상태라면 
            RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
            if (redisUser.State != UserState.Playing)
            {
                return new SkillGameResponse
                {
                    Result = ErrorCode.PlayerIsNotPlaying
                };
            }

            var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(skillGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new SkillGameResponse
                {
                    Result = Result
                };
            }

            if (redisRoom.State != RoomState.Playing)
            {
                return new SkillGameResponse
                {
                    Result = ErrorCode.IsNotPlayingThisRoom
                };
            }

            RedisGame? redisGame;
            (Result, redisGame) = await _memoryDB.LoadRedisGame(skillGameRequest.GameId);
            if (Result != ErrorCode.None)
            {
                return new SkillGameResponse
                {
                    Result = Result
                };
            }

            if (redisGame.WhoIsTurn.UserId != skillGameRequest.UserId)
            {
                return new SkillGameResponse
                {
                    Result = ErrorCode.IsNotDiceTurn
                };
            }

            Result = await UpdateRedisGame(redisGame, skillGameRequest.UserId);

            return new SkillGameResponse
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
