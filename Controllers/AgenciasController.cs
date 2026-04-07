using Microsoft.AspNetCore.Mvc;

namespace DriveNow.MVC.Controllers
{
    public class AgenciasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
