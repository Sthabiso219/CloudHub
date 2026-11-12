
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

        // GET: CustomerDashboard/Profile
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

            if (customer == null ||
                !customer.IsActive ||
                !customer.IsEmailVerified)
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

                AccountType = customer.AccountType,
                OrganisationName = customer.OrganisationName,
                PrimaryUse = customer.PrimaryUse,
                PreferredTechnology = customer.PreferredTechnology,
                PreferredDeploymentMethod = customer.PreferredDeploymentMethod,

                IsEmailVerified = customer.IsEmailVerified,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt
            };

            return View(model);
        }

        // POST: CustomerDashboard/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Profile(
            [Bind(Include =
                "FirstName,LastName,PhoneNumber," +
                "OrganisationName,PrimaryUse," +
                "PreferredTechnology,PreferredDeploymentMethod")]
            CustomerProfileViewModel model)
        {
            string email = User.Identity.Name;

            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction(
                    "Login", "CustomerAccount");
            }

            email = email.Trim().ToLowerInvariant();

            // Retrieve the actual logged-in customer.
            // Never trust a customer ID submitted by the browser.
            var customer = await db.Customers
                .FirstOrDefaultAsync(c =>
                    c.EmailAddress == email);

            if (customer == null ||
                !customer.IsActive ||
                !customer.IsEmailVerified)
            {
                return new HttpStatusCodeResult(403);
            }

            // Restore read-only information from the database.
            // This is also needed when returning an invalid form.
            model.CustomerId = customer.CustomerId;
            model.EmailAddress = customer.EmailAddress;
            model.AccountType = customer.AccountType;
            model.IsEmailVerified = customer.IsEmailVerified;
            model.IsActive = customer.IsActive;
            model.CreatedAt = customer.CreatedAt;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update editable personal information.
            customer.FirstName = model.FirstName.Trim();
            customer.LastName = model.LastName.Trim();
            customer.PhoneNumber = model.PhoneNumber.Trim();

            // Update organisation information only for business accounts.
            customer.OrganisationName =
                customer.AccountType == "Business"
                    ? model.OrganisationName?.Trim()
                    : customer.OrganisationName;

            // Update hosting preferences.
            customer.PrimaryUse = model.PrimaryUse;
            customer.PreferredTechnology = model.PreferredTechnology;
            customer.PreferredDeploymentMethod =
                model.PreferredDeploymentMethod;

            await db.SaveChangesAsync();

            TempData["ProfileSuccess"] =
                "Your profile has been updated successfully.";

            return RedirectToAction("Profile");
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
