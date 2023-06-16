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

        // 해당 방의 상태 확인 PLAYING인지 READY인지 확인
        var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(enterRoomRequest.RoomId);
        if(Result != ErrorCode.None)
        {
            return new EnterRoomResponse
            {
                Result = Result
            };
        }

        if(redisRoom.State == RoomState.Playing)
        {
            return new EnterRoomResponse
            {
                Result = ErrorCode.AlreadyPlayRoom
            };
        }

        Result = await ChangeUserState(redisUser, UserState.Playing);
        if(Result != ErrorCode.None)
        {
            return new EnterRoomResponse
            {
                Result = Result
            };
        }

        Result = await EnterRoomInMemory(enterRoomRequest.UserId, enterRoomRequest.RoomId);

        if(Result != ErrorCode.None)
        {
            var Error = await ChangeUserState(redisUser, UserState.Playing);
            if(Error != ErrorCode.None)
            {
                return new EnterRoomResponse
                {
                    Result = Error
                };
            }
            return new EnterRoomResponse
            {
                Result = Result
            };
        }

        return new EnterRoomResponse
        {
            Result = Result,
            RoomId = enterRoomRequest.RoomId
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
