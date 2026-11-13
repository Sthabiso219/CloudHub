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
    public class ProjectController : Controller
    {
        private readonly CloudHubDbContext db = new CloudHubDbContext();

        private async Task<Customer> GetCustomerAsync()
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return null;
            email = email.Trim().ToLowerInvariant();
            return await db.Customers.AsNoTracking().FirstOrDefaultAsync(c =>
                c.EmailAddress == email && c.IsActive && c.IsEmailVerified);
        }

        [HttpGet]
        public async Task<ActionResult> Index(string search = null)
        {
            var customer = await GetCustomerAsync();
            if (customer == null) return new HttpStatusCodeResult(403);
            var query = db.Projects.AsNoTracking().Where(p => p.CustomerId == customer.CustomerId && !p.IsArchived);
            search = (search ?? "").Trim();
            if (search.Length > 100) search = search.Substring(0, 100);
            if (search.Length > 0) query = query.Where(p => p.ProjectName.Contains(search));
            var projects = await query.OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProjectListItemViewModel
                {
                    ProjectId = p.ProjectId, ProjectName = p.ProjectName, Framework = p.Framework,
                    Status = p.Status, CreatedAt = p.CreatedAt,
                    DeploymentCount = db.Deployments.Count(d => d.ProjectId == p.ProjectId),
                    LastDeploymentAt = db.Deployments.Where(d => d.ProjectId == p.ProjectId)
                        .OrderByDescending(d => d.CreatedAt).Select(d => (DateTime?)d.CreatedAt).FirstOrDefault(),
                    LatestDeploymentStatus = db.Deployments.Where(d => d.ProjectId == p.ProjectId)
                        .OrderByDescending(d => d.CreatedAt).ThenByDescending(d => d.DeploymentId)
                        .Select(d => d.Status).FirstOrDefault()
                }).ToListAsync();
            return View(new ProjectsPageViewModel { Search = search, Projects = projects });
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            if (await GetCustomerAsync() == null) return new HttpStatusCodeResult(403);
            return View(new ProjectFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProjectFormViewModel model)
        {
            var customer = await GetCustomerAsync();
            if (customer == null) return new HttpStatusCodeResult(403);
            if (!ModelState.IsValid) return View(model);
            var name = model.ProjectName.Trim();
            var framework = model.Framework.Trim();
            if (name.Length == 0 || framework.Length == 0)
            {
                ModelState.AddModelError("", "Enter a project name and framework.");
                return View(model);
            }
            bool duplicate = await db.Projects.AnyAsync(p => p.CustomerId == customer.CustomerId && !p.IsArchived && p.ProjectName == name);
            if (duplicate)
            {
                ModelState.AddModelError("ProjectName", "You already have a project with this name.");
                return View(model);
            }
            var project = new Project
            {
                CustomerId = customer.CustomerId, ProjectName = name, Framework = framework,
                Status = "Created", CreatedAt = DateTime.UtcNow, IsArchived = false
            };
            db.Projects.Add(project);
            await db.SaveChangesAsync();
            TempData["ProjectSuccess"] = "Project created successfully.";
            return RedirectToAction("Details", new { id = project.ProjectId });
        }

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var customer = await GetCustomerAsync();
            if (customer == null) return new HttpStatusCodeResult(403);
            var project = await db.Projects.AsNoTracking().Where(p => p.ProjectId == id && p.CustomerId == customer.CustomerId && !p.IsArchived)
                .Select(p => new ProjectDetailsViewModel { ProjectId = p.ProjectId, ProjectName = p.ProjectName,
                    Framework = p.Framework, Status = p.Status, CreatedAt = p.CreatedAt }).FirstOrDefaultAsync();
            if (project == null) return HttpNotFound();
            project.Deployments = await db.Deployments.AsNoTracking().Where(d => d.ProjectId == id)
                .OrderByDescending(d => d.CreatedAt).ThenByDescending(d => d.DeploymentId)
                .Select(d => new ProjectDeploymentItemViewModel { DeploymentId = d.DeploymentId,
                    DeploymentMethod = d.DeploymentMethod, Status = d.Status, CreatedAt = d.CreatedAt,
                    CompletedAt = d.CompletedAt }).ToListAsync();
            return View(project);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var customer = await GetCustomerAsync();
            if (customer == null) return new HttpStatusCodeResult(403);
            var model = await db.Projects.AsNoTracking().Where(p => p.ProjectId == id && p.CustomerId == customer.CustomerId && !p.IsArchived)
                .Select(p => new ProjectFormViewModel { ProjectId = p.ProjectId, ProjectName = p.ProjectName, Framework = p.Framework }).FirstOrDefaultAsync();
            if (model == null) return HttpNotFound();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProjectFormViewModel model)
        {
            var customer = await GetCustomerAsync();
            if (customer == null) return new HttpStatusCodeResult(403);
            var project = await db.Projects.FirstOrDefaultAsync(p => p.ProjectId == model.ProjectId && p.CustomerId == customer.CustomerId && !p.IsArchived);
            if (project == null) return HttpNotFound();
            if (!ModelState.IsValid) return View(model);
            var name = model.ProjectName.Trim();
            var framework = model.Framework.Trim();
            if (name.Length == 0 || framework.Length == 0)
            {
                ModelState.AddModelError("", "Enter a project name and framework.");
                return View(model);
            }
            bool duplicate = await db.Projects.AnyAsync(p => p.CustomerId == customer.CustomerId && !p.IsArchived && p.ProjectId != project.ProjectId && p.ProjectName == name);
            if (duplicate)
            {
                ModelState.AddModelError("ProjectName", "You already have a project with this name.");
                return View(model);
            }
            project.ProjectName = name;
            project.Framework = framework;
            await db.SaveChangesAsync();
            TempData["ProjectSuccess"] = "Project updated successfully.";
            return RedirectToAction("Details", new { id = project.ProjectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Archive(int id)
        {
            var customer = await GetCustomerAsync();
            if (customer == null) return new HttpStatusCodeResult(403);
            var project = await db.Projects.FirstOrDefaultAsync(p => p.ProjectId == id && p.CustomerId == customer.CustomerId && !p.IsArchived);
            if (project == null) return HttpNotFound();
            // Prevent hiding a project while a deployment is in progress.
            bool busy = await db.Deployments.AnyAsync(d => d.ProjectId == id &&
                (d.Status == "Pending" || d.Status == "Queued" || d.Status == "Building" || d.Status == "Deploying"));
            if (busy)
            {
                TempData["ProjectError"] = "This project has an unfinished deployment and cannot be archived yet.";
                return RedirectToAction("Details", new { id });
            }
            project.IsArchived = true;
            project.ArchivedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            TempData["ProjectSuccess"] = "Project archived. Deployment history was preserved.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
