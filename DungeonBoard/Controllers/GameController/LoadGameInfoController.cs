using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Game;
using DungeonBoard.Models.Room;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    [ApiController]
    public class LoadGameInfoController : Controller
    {
        IMemoryDB _memoryDB; 
        public LoadGameInfoController(IMemoryDB memoryDB)
        {
            _memoryDB = memoryDB;
        }

        [Route("/Game/Info")]
        [HttpPost]
        async public Task<LoadGameInfoResponse> LoadGameInfo(LoadGameInfoRequest loadGameInfoRequest)
        {
            RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];

            Console.WriteLine("RE1");
            
            // GameId == RoomId 
            var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(loadGameInfoRequest.GameId); 
            if(Result != ErrorCode.None)
            {
                return new LoadGameInfoResponse
                {
                    Result = Result
                };
            }

            if(redisRoom.State != RoomState.Playing)
            {
                return new LoadGameInfoResponse
                {
                    Result = ErrorCode.IsNotPlayingThisRoom
                };
            }

            // Load Game Info
            RedisGame? redisGame;
            (Result, redisGame) = await _memoryDB.LoadRedisGame(redisRoom.RoomId);
            if(Result != ErrorCode.None)
            {
                return new LoadGameInfoResponse
                {
                    Result = Result
                };
            }

            Console.WriteLine("RE2");

            return new LoadGameInfoResponse
            {
                Result = ErrorCode.None,
                GameInfo = redisGame
            };
        }
    }
}
