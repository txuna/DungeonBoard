using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class EnterRoomController : Controller
{
    IMemoryDB _memoryDB;
    public EnterRoomController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;
    }

    [Route("/Room/Enter")]
    [HttpPost]
    async public Task<EnterRoomResponse> EnterRoom(EnterRoomRequest enterRoomRequest)
    {
        return new EnterRoomResponse
        {
            Result = Models.ErrorCode.None
        };
    }
}
