using DungeonBoard.Models;
using DungeonBoard.ReqResModels.Class;
using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.ClassController;

[ApiController]
public class ClassSelectController : Controller
{
    public ClassSelectController()
    {

    }

    [Route("/Class/Select")]
    [HttpPost]
    async public Task<ClassSelectResponse> SelectClass(ClassSelectRequest classSelectRequest)
    {
        return new ClassSelectResponse
        {
            Result = ErrorCode.None
        };
    }
}
