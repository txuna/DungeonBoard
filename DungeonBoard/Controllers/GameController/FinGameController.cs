using DungeonBoard.ReqResModels.Game;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    [ApiController]
    public class FinGameController : Controller
    {
        [Route("/Game/Fin")]
        [HttpPost]
        async public Task<FinGameResponse> FinGame(FinGameRequest finGameRequest)
        {
            // 방이 게임중인지 확인 및 방 로드 

            // 게임이 끝난상태가 맞는지 확인

            // 끝난게 맞다면 유저 상태에서 Lobby로 변경 

            return new FinGameResponse
            {
                Result = Models.ErrorCode.None
            };
        }
    }
}
