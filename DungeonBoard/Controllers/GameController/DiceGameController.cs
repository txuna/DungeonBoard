using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Game;
using DungeonBoard.Models.Room;
using DungeonBoard.ReqResModels;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    [ApiController]
    public class DiceGameController : Controller
    {
        IMemoryDB _memoryDB;
        public DiceGameController(IMemoryDB memoryDB)
        {
            _memoryDB = memoryDB;
        }

        [Route("/Game/Dice")]
        [HttpPost]
        async public Task<DiceGameResponse> GameDice(DiceGameRequest diceGameRequest)
        {
            // 해당 플레이어가 게임 중이고 해당 방이 게임상태라면 
            RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
            if(redisUser.State != UserState.Playing)
            {
                return new DiceGameResponse
                {
                    Result = ErrorCode.PlayerIsNotPlaying
                };
            }

            if (VerifyDiceNumber(diceGameRequest.DiceNumber) == false)
            {
                return new DiceGameResponse
                {
                    Result = ErrorCode.InvalidDiceNumber
                };
            }

            var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(diceGameRequest.GameId);
            if(Result != ErrorCode.None)
            {
                return new DiceGameResponse
                {
                    Result = Result
                };
            }

            if(redisRoom.State != RoomState.Playing)
            {
                return new DiceGameResponse
                {
                    Result = ErrorCode.IsNotPlayingThisRoom
                };
            }

            RedisGame? redisGame;
            (Result, redisGame) = await _memoryDB.LoadRedisGame(diceGameRequest.GameId); 
            if(Result != ErrorCode.None)
            {
                return new DiceGameResponse
                {
                    Result = Result
                };
            }

            // Position Update 
            Result = await UpdatePlayerPosition(redisGame, diceGameRequest.UserId, diceGameRequest.DiceNumber);

            return new DiceGameResponse
            {
                Result = Result
            };
        }

        bool VerifyDiceNumber(int dice)
        {
            if(dice >= 1 && dice <= 6)
            {
                return true;
            }

            return false;
        }

        async Task<ErrorCode> UpdatePlayerPosition(RedisGame redisGame, int userId, int dice)
        {
            for(int i=0; i<redisGame.Players.Length; i++)
            {
                if (redisGame.Players[i].UserId == userId)
                {
                    if (redisGame.Players[i].PositionCard + dice > 29)
                    {
                        int v = redisGame.Players[i].PositionCard + dice - 30;
                        redisGame.Players[i].PositionCard = v;
                    }
                    else
                    {
                        redisGame.Players[i].PositionCard += dice;
                    }
                    break; 
                }
            }

            return await _memoryDB.StoreRedisGame(redisGame);
        }

    }
}
