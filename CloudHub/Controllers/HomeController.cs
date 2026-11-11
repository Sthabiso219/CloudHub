using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace CloudHub.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Features()
        {
            return View();
        }

        public ActionResult Pricing()
        {
            return View();
        }

        public ActionResult Support()
        {
            return View();
        }
    }
}
