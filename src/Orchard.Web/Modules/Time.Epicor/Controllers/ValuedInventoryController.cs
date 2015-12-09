using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Time.Epicor.Controllers
{
    [Themed]
    public class ValuedInventoryController : Controller
    {
        // GET: ValuedInventory
        public ActionResult Index()
        {
            return View();
        }
    }
}