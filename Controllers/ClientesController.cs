using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Final.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
