using DungeonBoard.Models;
using DungeonBoard.Models.Account;
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

            return new LoadGameInfoResponse
            {
                Result = ErrorCode.None
            };
        }
    }
}
