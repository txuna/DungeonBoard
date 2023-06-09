using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class RoomExitController : Controller
{
    IMemoryDB _memoryDB;
    public RoomExitController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;   
    }

    [Route("/Room/Exit")]
    [HttpPost]
    async public Task<RoomExitResponse> ExitRoom(RoomExitRequest roomExitRequest)
    {
        return new RoomExitResponse
        {

        };
    }
}
