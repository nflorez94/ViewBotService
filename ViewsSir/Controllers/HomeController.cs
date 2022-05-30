using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ViewBot.Services;
using ViewsSir.Models;

namespace ViewBotServices.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IViewBotServices _viewBotServices;

        public HomeController(ILogger<HomeController> logger, IViewBotServices viewBotServices)
        {
            _logger = logger;
            _viewBotServices = viewBotServices;
        }

        public async Task<IActionResult> Index() 
        {
            //var algo=Path.GetDirectoryName(typeof(HomeController).Assembly.Location);
            //await _viewBotServices.PlayVideo("https://www.youtube.com/watch?v=Iu220UjOHZc&t=7s");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}