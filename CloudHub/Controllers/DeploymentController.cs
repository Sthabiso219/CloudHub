using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudHub.Models.ViewModels;

namespace CloudHub.Controllers
{
    [Authorize]
    public class DeploymentController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new DeploymentPageViewModel
            {
                DeploymentMethod = "GitHub"
            };

            return View(model);
        }
    }
}