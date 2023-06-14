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

            // Position Update - 업데이트시 최대값 29이상이 될시 0으로 

            return new DiceGameResponse
            {
                Result = ErrorCode.None
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
            return await _memoryDB.StoreRedisGame(redisGame);
        }

    }
}
