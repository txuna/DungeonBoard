using DungeonBoard.Models;
using DungeonBoard.Models.Master;
using DungeonBoard.Models.Player;
using DungeonBoard.Models.Room;
using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class LoadRoomFromIdController : Controller
{
    IMemoryDB _memoryDB;
    IPlayerDB _playerDB;
    IMasterDB _masterDB;
    public LoadRoomFromIdController(IMemoryDB memoryDB, IPlayerDB playerDB, IMasterDB masterDB)
    {
        _memoryDB = memoryDB;
        _playerDB = playerDB;
        _masterDB = masterDB;
    }

    /*
     *  클라이언트는 해당 API를 지속적으로 폴링하여 방의 상태를 업데이트한다. (인원 추가 확인)
     *  클라이언트는 해당 정보를 기반으로 플레이어 표시 및 각종 업데이트 
     *  Room정보와 Player정보 
     */
    [Route("/Room/Load")]
    [HttpPost]
    async public Task<LoadRoomFromIdResponse> LoadRoomInfoFromId(LoadRoomFromIdRequest loadRoomFromIdRequest)
    {
        var (Result, Room) = await _memoryDB.LoadRoomFromId(loadRoomFromIdRequest.RoomId);

        if(Result != ErrorCode.None)
        {
            return new LoadRoomFromIdResponse
            {
                Result = Result,
                Room = null,
                Player = null
            };
        }

        int index = 0;
        Player[]? Players = new Player[Room.Users.Length];
        foreach(var userId in Room.Users)
        {
            var (Error, p) = await LoadPlayer(userId);
            if(Error != ErrorCode.None)
            {
                return new LoadRoomFromIdResponse
                {
                    Result = Error,
                    Room = null,
                    Player = null
                };
            }

            Players[index] = new Player
            {
                UserId = p.UserId,
                ClassId = p.ClassId
            };
            index++;
        }

        return new LoadRoomFromIdResponse
        {
            Result = Result,
            Room = Room,
            Player = Players
        };
    }

    async Task<(ErrorCode, Player?)> LoadPlayer(int userId)
    {
        return await _playerDB.LoadPlayerFromId(userId);
    }
}
