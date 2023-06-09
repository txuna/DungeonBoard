﻿using DungeonBoard.Models;
using DungeonBoard.Models.Account;
using DungeonBoard.Models.Master;
using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class CreateRoomController : Controller
{
    IMemoryDB _memoryDB;
    IMasterDB _masterDB;
    public CreateRoomController(IMemoryDB memoryDB, IMasterDB masterDB)
    {
        _memoryDB = memoryDB;
        _masterDB = masterDB;
    }

    [Route("/Room/Create")]
    [HttpPost]
    async public Task<CreateRoomResponse> CreateRoom(CreateRoomRequest createRoomRequest)
    {
        //해당 유저가 이미 플레이중인지 확인한다. 
        RedisUser redisUser = (RedisUser)HttpContext.Items["Redis-User"];
        if(redisUser.State == UserState.Playing)
        {
            return new CreateRoomResponse
            {
                Result = ErrorCode.AlreadyInRoom
            };
        }

        if (VerifyBossId(createRoomRequest.BossId) == false)
        {
            return new CreateRoomResponse
            {
                Result = ErrorCode.InvalidBossId
            };
        }

        if (VerifyHeadCount(createRoomRequest.HeadCount) == false)
        {
            return new CreateRoomResponse
            {
                Result = ErrorCode.InvalidHeadCount
            };
        }

        int roomId;
        var Result = await ChangeUserState(redisUser, UserState.Playing);
        if (Result != ErrorCode.None)
        {
            return new CreateRoomResponse
            {
                Result = Result,
            };
        }

        (Result, roomId) = await CreateRoomInMemory(createRoomRequest);
        if(Result != ErrorCode.None)
        {
            var Error = await ChangeUserState(redisUser, UserState.Lobby);
            if(Error != ErrorCode.None)
            {
                return new CreateRoomResponse
                {
                    Result = Error,
                };
            }
            return new CreateRoomResponse
            {
                Result = Result,
            };
        }

        return new CreateRoomResponse
        {
            Result = Result, 
            RoomId = roomId
        };
    }

    async Task<ErrorCode> ChangeUserState(RedisUser redisUser, UserState state)
    {
        redisUser.State = state;
        return await _memoryDB.StoreRedisUser(redisUser.UserId, redisUser);
    }

    async Task<(ErrorCode, int)> CreateRoomInMemory(CreateRoomRequest createRoomRequest)
    {
        var (Result, roomId) = await _memoryDB.CreateRoom(createRoomRequest.Title,
                                                          createRoomRequest.UserId,
                                                          createRoomRequest.HeadCount,
                                                          createRoomRequest.BossId);

        return (Result, roomId);
    }

    bool VerifyBossId(int bossId)
    {
        MasterBossInfo? masterBossInfo = _masterDB.LoadMasterBossInfo(bossId);
        if(masterBossInfo == null)
        {
            return false;
        }
        return true;
    }

    bool VerifyHeadCount(int headCount)
    {
        if(headCount < 1 || headCount > 4)
        {
            return false;
        }
        return true;
    }

}
