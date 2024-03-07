using DI_Service_Lifetime.Models;
using DI_Service_Lifetime.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace DI_Service_Lifetime.Controllers
{
    public class HomeController : Controller
    {
        private readonly IScopedGuidService _scoped1;
        private readonly IScopedGuidService _scoped2;

        private readonly ISingletonGuidService _singleton1;
        private readonly ISingletonGuidService _singleton2;

        private readonly ITransientGuidService _transient1;
        private readonly ITransientGuidService _transient2;

        private readonly ILogger<HomeController> _logger;

        public HomeController(IScopedGuidService _scopedGuid1, 
            IScopedGuidService _scopedGuid2,
            ISingletonGuidService _singletonGuid1,
            ISingletonGuidService _singletonGuid2,
            ITransientGuidService _transientGuid1,
            ITransientGuidService _transientGuid2)
        {
            _scoped1 = _scopedGuid1;
            _scoped2 = _scopedGuid2;
            _singleton1 = _singletonGuid1;
            _singleton2 = _singletonGuid2;
            _transient1 = _transientGuid1;
            _transient2 = _transientGuid2;
        }

        public IActionResult Index()
        {
            StringBuilder messages = new StringBuilder();
            messages.Append($"Transient 1: { _transient1.GetGuid()}\n");
            messages.Append($"Transient 2: {_transient2.GetGuid()}\n\n\n");
            messages.Append($"Scoped 1: {_scoped1.GetGuid()}\n");
            messages.Append($"Scoped 2: {_scoped2.GetGuid()}\n\n\n");
            messages.Append($"Singleton 1: {_singleton1.GetGuid()}\n");
            messages.Append($"Singleton 2: {_singleton2.GetGuid()}\n\n\n");
            return Ok(messages.ToString());
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
