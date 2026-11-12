
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudHub.Models;
using CloudHub.Models.ViewModels;

namespace CloudHub.Controllers
{
    [Authorize]
    public class DeploymentController : Controller
    {
        private readonly CloudHubDbContext db =
            new CloudHubDbContext();

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string email = User.Identity.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(
                    "Login", "CustomerAccount");
            }

            email = email.Trim().ToLowerInvariant();

            var customer = await db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.EmailAddress == email);

            if (customer == null ||
                !customer.IsActive ||
                !customer.IsEmailVerified)
            {
                return new HttpStatusCodeResult(403);
            }

            var projectRecords = await db.Projects
                .AsNoTracking()
                .Where(p => p.CustomerId == customer.CustomerId)
                .OrderBy(p => p.ProjectName)
                .Select(p => new
                {
                    p.ProjectId,
                    p.ProjectName
                })
                .ToListAsync();

            var projectOptions = projectRecords
                .Select(p => new SelectListItem
                {
                    Value = p.ProjectId.ToString(),
                    Text = p.ProjectName
                })
                .ToList();

            var model = new DeploymentPageViewModel
            {
                DeploymentMethod = "GitHub",
                Projects = projectOptions
            };

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
