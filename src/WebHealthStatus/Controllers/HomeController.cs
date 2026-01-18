using Microsoft.AspNetCore.Mvc;

namespace WebHealthStatus.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/healthchecks-ui");
        }
    }
}
