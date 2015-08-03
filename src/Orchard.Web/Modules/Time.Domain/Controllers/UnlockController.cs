using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Domain.Controllers
{
    [Themed]
    public class UnlockController : Controller
    {
        // GET: Unlock
        public ActionResult Index()
        {
            return View();
        }
    }
}