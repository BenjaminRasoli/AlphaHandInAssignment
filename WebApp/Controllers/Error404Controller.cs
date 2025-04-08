using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class Error404Controller : Controller
{
    public IActionResult Error404()
    {
        return View();
    }
}
