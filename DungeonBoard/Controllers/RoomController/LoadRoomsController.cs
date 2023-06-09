using DungeonBoard.Models.Room;
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

    [Route("/Room/LoadAll")]
    [HttpPost]
    async public Task<LoadRoomsResponse> LoadRooms(LoadRoomsRequest loadRoomRequest)
    {
        var (Result, redisRoom) = await _memoryDB.LoadAllRoom();

        return new LoadRoomsResponse
        {
            Result = Result, 
            redisRoom = redisRoom
        };
    }
}
