using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController;

[ApiController]
public class ExitRoomController : Controller
{
    IMemoryDB _memoryDB;
    public ExitRoomController(IMemoryDB memoryDB)
    {
        _memoryDB = memoryDB;   
    }

    [Route("/Room/Exit")]
    [HttpPost]
    async public Task<ExitRoomResponse> ExitRoom(ExitRoomRequest exitRoomRequest)
    {
        // 플레이어의 상태 확인 -> PLAYING상태인가

        // 방의 상태가 READY상태인가 확인

        // 해당 플레이어가 해당 방에 속해있는지 확인

        // 만약 HostUserId와 동일하다면 방삭제

        return new ExitRoomResponse
        {

        };
    }
}
