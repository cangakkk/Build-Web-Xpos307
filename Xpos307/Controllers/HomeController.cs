using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xpos307.DataModels;
using Xpos307.Models;

namespace Xpos307.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Coba()
        {
            List<TblCoba> tot = new List<TblCoba>
            {
                new TblCoba{Id=1,Name="abdul",Description="aaaa"},
                new TblCoba{Id=2,Name="ab",Description="aaaa"},
                new TblCoba{Id=3,Name="dul",Description="aaaa"},
            };

            ViewBag.TblCoba = tot;
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
