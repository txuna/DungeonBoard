using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.AccountController;

[ApiController]
public class LoginController : Controller
{
    [Route("/Login")]
    [HttpGet]
    public IActionResult Index()
    {
        return Content("HEllo");
    }
}
