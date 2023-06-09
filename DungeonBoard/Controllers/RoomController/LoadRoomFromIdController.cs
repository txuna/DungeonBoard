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

    [Route("/Room/Load")]
    [HttpPost]
    async public Task<LoadRoomFromIdResponse> LoadRoomFromId(LoadRoomFromIdRequest loadRoomFromIdRequest)
    {
        var (Result, Room) = await _memoryDB.LoadRoomFromId(loadRoomFromIdRequest.RoomId);

        return new LoadRoomFromIdResponse
        {
            Result = Result,
            Room = Room
        };
    }
}
