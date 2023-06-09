using DungeonBoard.ReqResModels.Room;
using DungeonBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.RoomController
{
    public class DeleteRoomController : Controller
    {
        IMemoryDB _memoryDB;
        public DeleteRoomController(IMemoryDB memoryDB)
        {
            _memoryDB = memoryDB;
        }

        [Route("/Room/Delete")]
        [HttpPost]
        async public Task<DeleteRoomResponse> DeleteRoom(DeleteRoomRequest deleteRoomRequest)
        {
            return new DeleteRoomResponse
            {

            };
        }
    }
}
