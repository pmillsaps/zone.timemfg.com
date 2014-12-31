using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.IT.Controllers
{
    public class ITController : Controller
    {
        // GET: IT
        [Themed]
        public ActionResult Index()
        {
            return View();
        }
    }
}