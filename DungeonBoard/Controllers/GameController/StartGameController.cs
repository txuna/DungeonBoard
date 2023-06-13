using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController;

[ApiController]
public class StartGameController : Controller
{
    IMemoryDB _memoryDB;
    public StartGameController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;
    }

    [Route("/Game/Start")]
    [HttpPost]
    async public Task<StartGameResponse> StartGame(StartGameRequest startGameRequest)
    {
        // 방에 참여하고 있는지 확인 
        RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
        if(redisUser.State != UserState.Playing)
        {
            return new StartGameResponse
            {
                Result = ErrorCode.NoneExistInThisRoom
            };
        }

        var (Result, room) = await _memoryDB.LoadRoomFromId(startGameRequest.RoomId);
        if(Result != ErrorCode.None)
        {
            return new StartGameResponse
            {
                Result = Result
            };
        }

        // 해당 방의 호스트인지 확인
        if(room.HostUserId != redisUser.UserId)
        {
            return new StartGameResponse
            {
                Result = ErrorCode.NoneHostThisRoom
            };
        }

        // 해당 방의 상태가 Ready가 맞는지 확인
        if(room.State != RoomState.Ready)
        {
            return new StartGameResponse
            {
                Result = ErrorCode.AlreadyPlayRoom
            };
        }

        // 해당 방의 상태 변경 
        room.State = RoomState.Playing;
        Result = await _memoryDB.UpdateRedisRoom(room);

        return new StartGameResponse
        {
            Result = Result
        };
    }
}
