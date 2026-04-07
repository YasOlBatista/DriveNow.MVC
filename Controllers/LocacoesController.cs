using Microsoft.AspNetCore.Mvc;

namespace DriveNow.MVC.Controllers
{
    public class LocacoesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
