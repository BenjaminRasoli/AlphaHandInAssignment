using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class FileUploadController : Controller
    {
        public IActionResult Upload()
        {
            return View();
        }
    }
}
