using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Final.Controllers
{
    public class VentasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
