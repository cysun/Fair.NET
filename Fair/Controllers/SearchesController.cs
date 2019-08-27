using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Fair.Controllers
{
    public class SearchesController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
