using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class EnterRoomController : Controller
{
    IMemoryDB _memoryDB;
    public EnterRoomController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;
    }

    [Route("/Room/Enter")]
    [HttpPost]
    async public Task<EnterRoomResponse> EnterRoom(EnterRoomRequest enterRoomRequest)
    {
        // 유저 플레이 상태 확인 
        RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
        if (redisUser.State == UserState.Playing)
        {
            return new EnterRoomResponse
            {
                Result = ErrorCode.AlreadyInRoom
            };
        }
        
        ErrorCode Result = await EnterRoomInMemory(enterRoomRequest.UserId, enterRoomRequest.RoomId);

        if(Result != ErrorCode.None)
        {
            return new EnterRoomResponse
            {
                Result = Result
            };
        }

        Result = await ChangeUserState(redisUser, UserState.Playing);

        return new EnterRoomResponse
        {
            Result = Result
        };
    }

    async Task<ErrorCode> ChangeUserState(RedisUser redisUser, UserState state)
    {
        redisUser.State = state;
        return await _memoryDB.StoreRedisUser(redisUser.UserId, redisUser);
    }

    async Task<ErrorCode> EnterRoomInMemory(int roomId, int userId)
    {
        ErrorCode Result = await _memoryDB.EnterRoom(roomId, userId);
        return Result;
    }
}
