using Microsoft.AspNetCore.Mvc;

namespace DungeonBoard.Controllers.GameController
{
    public class GameSkillController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
