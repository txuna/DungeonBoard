using DungeonBoard.Models.Room;
using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class LoadRoomFromIdController : Controller
{
    IMemoryDB _memoryDB;
    public LoadRoomFromIdController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;
    }

    /*
     *  클라이언트는 해당 API를 지속적으로 폴링하여 방의 상태를 업데이트한다. (인원 추가 확인)
     *  클라이언트는 해당 정보를 기반으로 플레이어 표시 및 각종 업데이트 
     */
    [Route("/Room/Load")]
    [HttpPost]
    async public Task<LoadRoomFromIdResponse> LoadRoomInfoFromId(LoadRoomFromIdRequest loadRoomFromIdRequest)
    {
        var (Result, Room) = await _memoryDB.LoadRoomFromId(loadRoomFromIdRequest.RoomId);

        

        return new LoadRoomFromIdResponse
        {
            Result = Result,
            Room = Room
        };
    }
}
