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
        if(redisUser.State == Models.UserState.Playing)
        {
            return new CreateRoomResponse
            {
                Result = Models.ErrorCode.AlreadyInRoom
            };
        }
        
        if(VerifyBossId(createRoomRequest.BossId) == false)
        {
            return new CreateRoomResponse
            {
                Result = Models.ErrorCode.InvalidBossId
            };
        }

        if(VerifyHeadCount(createRoomRequest.RoomHeadCount) == false)
        {
            return new CreateRoomResponse
            {
                Result = Models.ErrorCode.InvalidHeadCount
            };
        }

        //방을 생성한다. 

        return new CreateRoomResponse
        {
            Result = Models.ErrorCode.None
        };
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
