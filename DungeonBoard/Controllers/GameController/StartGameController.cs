using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.ReqResModels.Game;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController;

public class StartGameController : Controller
{
    IMemoryDB _memoryDB;
    public StartGameController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;
    }

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

        return new StartGameResponse
        {
            Result = ErrorCode.None
        };
    }
}
