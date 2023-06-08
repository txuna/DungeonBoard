using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class LoadRoomsController : Controller
{
    IMemoryDB _memoryDB;
    public LoadRoomsController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;
    }

    [Route("/Room/Load")]
    [HttpPost]
    async public Task<LoadRoomResponse> LoadRooms(LoadRoomRequest loadRoomRequest)
    {
        return new LoadRoomResponse
        {
            Result = Models.ErrorCode.None
        };
    }
}
