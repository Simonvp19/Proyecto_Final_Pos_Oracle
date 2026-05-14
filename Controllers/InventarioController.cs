using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Final.Controllers
{
    public class InventarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
