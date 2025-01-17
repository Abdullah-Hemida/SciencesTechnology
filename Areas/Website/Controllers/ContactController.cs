using Microsoft.AspNetCore.Mvc;

namespace SciencesTechnology.Areas.Website.Controllers
{
    [Area("Website")]
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
