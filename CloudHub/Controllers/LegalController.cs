using System.Web.Mvc;

namespace CloudHub.Controllers
{
    // Public pages: no login needed, so visitors can read them before registering.
    [AllowAnonymous]
    public class LegalController : Controller
    {
        // GET: Legal/Terms
        [HttpGet]
        public ActionResult Terms()
        {
            return View();
        }

        // GET: Legal/Privacy
        [HttpGet]
        public ActionResult Privacy()
        {
            return View();
        }
    }
}