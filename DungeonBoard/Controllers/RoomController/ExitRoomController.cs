using DungeonBoard.Models.Account;
using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;
using DungeonBoard.Models;
using DungeonBoard.Models.Room;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class ExitRoomController : Controller
{
    IMemoryDB _memoryDB;
    public ExitRoomController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;   
    }

    [Route("/Room/Exit")]
    [HttpPost]
    async public Task<ExitRoomResponse> ExitRoom(ExitRoomRequest exitRoomRequest)
    {
        RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];

        // 플레이어의 상태 확인 -> PLAYING상태인가
        if(redisUser.State != UserState.Playing)
        {
            return new ExitRoomResponse
            {
                Result = ErrorCode.PlayerIsNotInRoom
            };
        }

        // 방의 상태가 READY상태인가 확인
        var (Result, redisRoom) = await _memoryDB.LoadRoomFromId(exitRoomRequest.RoomId);
        if(Result != ErrorCode.None)
        {
            return new ExitRoomResponse
            {
                Result = Result
            };
        }

        if(redisRoom.State != RoomState.Ready)
        {
            return new ExitRoomResponse
            {
                Result = ErrorCode.AlreadyPlayRoom
            };
        }

        // 해당 플레이어가 해당 방에 속해있는지 확인
        if(redisRoom.Users.Any( e => e == exitRoomRequest.UserId) == false)
        {
            return new ExitRoomResponse
            {
                Result = ErrorCode.PlayerIsNotInRoom
            };
        }

        // 플레이어 상태 변경 및 나간방 레디스에 저장 
        // 만약 HostUserId와 동일하다면 변경
        // 만약 인원이 1명이라면 방삭제 
        redisRoom.Users = redisRoom.Users.Where( e => e != exitRoomRequest.UserId).ToArray();
        
        if(redisRoom.Users.Length == 0)
        {
            // 방 삭제
            Result = await _memoryDB.DeleteRedisRoom(redisRoom.RoomId);
            if(Result != ErrorCode.None)
            {
                return new ExitRoomResponse
                {
                    Result = Result
                };
            }
        }
        else
        {
            if (redisRoom.HostUserId == exitRoomRequest.UserId)
            {
                int randomIndex = new Random().Next(0, redisRoom.Users.Length);
                int randomElement = redisRoom.Users[randomIndex];
                redisRoom.HostUserId = randomElement;
            }

            Result = await _memoryDB.UpdateRedisRoom(redisRoom);
            if (Result != ErrorCode.None)
            {
                return new ExitRoomResponse
                {
                    Result = Result
                };
            }
        }

        Result = await ChangeUserState(redisUser, UserState.Lobby);
        if (Result != ErrorCode.None)
        {
            return new ExitRoomResponse
            {
                Result = Result
            };
        }

        return new ExitRoomResponse
        {
            Result = ErrorCode.None
        };
    }
    async Task<ErrorCode> ChangeUserState(RedisUser redisUser, UserState state)
    {
        redisUser.State = state;
        return await _memoryDB.StoreRedisUser(redisUser.UserId, redisUser);
    }
}
