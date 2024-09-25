using Microsoft.AspNetCore.Mvc;

namespace YouWeMovie.Controllers;

public class AboutUsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}