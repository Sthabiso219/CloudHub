
using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudHub.Models;
using CloudHub.Models.ViewModels;

namespace CloudHub.Controllers
{
    [Authorize]
    public class CustomerDashboardController : Controller
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

            var model = new CustomerDashboardViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                EmailAddress = customer.EmailAddress,

                // These modules are not yet connected.
                TotalProjects = null,
                TotalDeployments = null,
                StorageUsedGB = null,
                StorageLimitGB = null,
                CurrentPlan = null
            };

            return View(model);
        }


        [HttpGet]
        public async Task<ActionResult> Profile()
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

            if (customer == null || !customer.IsActive)
            {
                return new HttpStatusCodeResult(403);
            }

            var model = new CustomerProfileViewModel
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                EmailAddress = customer.EmailAddress,
                PhoneNumber = customer.PhoneNumber,
                IsEmailVerified = customer.IsEmailVerified,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt
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
