using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MSDev.Work.Tasks;

namespace WebAdmin.Controllers
{
    public class HomeController : BaseController
    {
        private IConfigurationRoot _configuration;
        public HomeController(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewBag.TaskEnv = MSDTask.GetTaskEnv();
            ViewBag.HostEnv = _configuration.GetConnectionString("DefaultConnection");
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }


        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
