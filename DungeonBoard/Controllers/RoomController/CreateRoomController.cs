using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class CreateRoomController : Controller
{
    IMemoryDB _memoryDB;
    public CreateRoomController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;
    }

    [Route("/Room/Create")]
    [HttpPost]
    async public Task<CreateRoomResponse> CreateRoom(CreateRoomRequest createRoomRequest)
    {
        return new CreateRoomResponse
        {
            Result = Models.ErrorCode.None
        };
    }
}
