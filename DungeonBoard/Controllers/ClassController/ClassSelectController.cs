using DungeonBoard.Models;
using DungeonBoard.Models.Player;
using DungeonBoard.ReqResModels.Class;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.ClassController;

[ApiController]
public class ClassSelectController : Controller
{
    IMasterDB _masterDB; 
    IPlayerDB _playerDB;
    public ClassSelectController(IPlayerDB playerDB, IMasterDB masterDB)
    {
        _playerDB = playerDB;
        _masterDB = masterDB;
    }

    [Route("/Class/Select")]
    [HttpPost]
    async public Task<ClassSelectResponse> SelectClass(ClassSelectRequest classSelectRequest)
    {
        if(IsExistClassId(classSelectRequest.ClassId) == false)
        {
            return new ClassSelectResponse
            {
                Result = ErrorCode.NoneExistClassId
            };
        }

        var (Result, player) = await _playerDB.LoadPlayerFromId(classSelectRequest.UserId);
        if(Result != ErrorCode.None)
        {
            return new ClassSelectResponse
            {
                Result = ErrorCode.NoneExistUserId
            };
        }

        // 이미 유저가 클래스를 소유하고 있는지 확인 
        if(HasAlreadyClass(player) == false)
        {
            return new ClassSelectResponse
            {
                Result = ErrorCode.PlayerAlreadyHasClass
            };
        }

        Result = await _playerDB.UpdatePlayerClass(player.UserId, classSelectRequest.ClassId);

        // 요청한 클래스가 있는지 확인
        return new ClassSelectResponse
        {
            Result = Result
        };
    }

    bool IsExistClassId(int classId)
    {
        var masterClassInitStat = _masterDB.LoadClassInitStat(classId);
        if(masterClassInitStat == null)
        {
            return false;
        }
        return true;
    }

    bool HasAlreadyClass(Player player)
    {
        if(player.ClassId != (int)ClassType.NoneClass)
        {
            return false;
        }

        return true;
    }
}
