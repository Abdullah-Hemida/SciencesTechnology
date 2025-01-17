using Microsoft.AspNetCore.Mvc;

namespace SciencesTechnology.Areas.Website.Controllers
{
    [Area("Website")]
    public class TrainersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
