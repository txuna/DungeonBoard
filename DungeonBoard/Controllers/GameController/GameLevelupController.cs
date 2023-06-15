using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    public class GameLevelupController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
