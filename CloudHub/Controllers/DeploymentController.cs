
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
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
       
        private const int MaxUploadSize =
            500 * 1024 * 1024;

        private const int MaxArchiveEntries = 2000;

        private const long MaxEntrySize =
            100L * 1024 * 1024;

        private const long MaxUncompressedSize =
            200L * 1024 * 1024;

        // ==========================================
        // GET LOGGED-IN CUSTOMER
        // ==========================================

        private async Task<Customer> GetCustomerAsync()
        {
            if (User == null ||
                User.Identity == null ||
                !User.Identity.IsAuthenticated)
            {
                return null;
            }

            string email = User.Identity.Name;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            email = email.Trim().ToLowerInvariant();

            return await db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.EmailAddress == email &&
                    c.IsActive &&
                    c.IsEmailVerified);
        }

        // ==========================================
        // HOSTED APPLICATIONS
        // GET: /Deployment/Index
        // ==========================================

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var customer = await GetCustomerAsync();

            if (customer == null)
                return new HttpStatusCodeResult(403);

            var deployments = await db.Deployments
                .AsNoTracking()
                .Where(d =>
                    d.Project.CustomerId ==
                        customer.CustomerId &&
                    d.Status == "Successful")
                .OrderByDescending(d => d.CreatedAt)
                .ThenByDescending(d => d.DeploymentId)
                .Select(d => new HostedApplicationViewModel
                {
                    DeploymentId = d.DeploymentId,
                    ProjectId = d.ProjectId,
                    ProjectName = d.Project.ProjectName,
                    Framework = d.Project.Framework,
                    DeploymentMethod = d.DeploymentMethod,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                    CompletedAt = d.CompletedAt
                })
                .ToListAsync();

            var applications = deployments
                .GroupBy(d => d.ProjectId)
                .Select(g => g.First())
                .ToList();

            var model = new HostedApplicationsPageViewModel
            {
                Applications = applications
            };

            return View(model);
        }

        // ==========================================
        // CREATE DEPLOYMENT PAGE
        // GET: /Deployment/Create
        // ==========================================

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var customer = await GetCustomerAsync();

            if (customer == null)
                return new HttpStatusCodeResult(403);

            var projects = await db.Projects
                .AsNoTracking()
                .Where(p =>
                    p.CustomerId == customer.CustomerId)
                .OrderBy(p => p.ProjectName)
                .Select(p => new
                {
                    p.ProjectId,
                    p.ProjectName
                })
                .ToListAsync();

            var model = new DeploymentPageViewModel
            {
                DeploymentMethod = "GitHub",

                Projects = projects
                    .Select(p => new SelectListItem
                    {
                        Value = p.ProjectId.ToString(),
                        Text = p.ProjectName
                    })
                    .ToList()
            };

            return View(model);
        }

        // ==========================================
        // DEPLOYMENT DETAILS
        // GET: /Deployment/Details/5
        // ==========================================

        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var customer = await GetCustomerAsync();

            if (customer == null)
                return new HttpStatusCodeResult(403);

            var deployment = await db.Deployments
                .AsNoTracking()
                .Where(d =>
                    d.DeploymentId == id &&
                    d.Project.CustomerId ==
                        customer.CustomerId &&
                    d.Status == "Successful")
                .Select(d => new HostedApplicationViewModel
                {
                    DeploymentId = d.DeploymentId,
                    ProjectId = d.ProjectId,
                    ProjectName = d.Project.ProjectName,
                    Framework = d.Project.Framework,
                    DeploymentMethod = d.DeploymentMethod,
                    Status = d.Status,
                    CreatedAt = d.CreatedAt,
                    CompletedAt = d.CompletedAt
                })
                .FirstOrDefaultAsync();

            if (deployment == null)
                return HttpNotFound();

            return View(deployment);
        }

        // ==========================================
        // UPLOAD ZIP
        // POST: /Deployment/UploadZip
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadZip(
            ZipDeploymentViewModel model)
        {
            var customer = await GetCustomerAsync();

            if (customer == null)
                return new HttpStatusCodeResult(403);

            if (!ModelState.IsValid ||
                !model.ProjectId.HasValue ||
                model.ZipFile == null)
            {
                TempData["DeploymentError"] =
                    "Please select a project and ZIP file.";

                return RedirectToAction("Create");
            }

            bool ownsProject = await db.Projects.AnyAsync(p =>
            p.ProjectId == model.ProjectId.Value &&
            p.CustomerId == customer.CustomerId &&
           !p.IsArchived);     

            if (!ownsProject)
                return HttpNotFound();

            var file = model.ZipFile;

            if (file.ContentLength <= 0 ||
                file.ContentLength > MaxUploadSize)
            {
                TempData["DeploymentError"] =
                    "ZIP file must be smaller than 500 MB.";

                return RedirectToAction("Create");
            }

            string extension =
                Path.GetExtension(file.FileName);

            if (!string.Equals(
                extension,
                ".zip",
                StringComparison.OrdinalIgnoreCase))
            {
                TempData["DeploymentError"] =
                    "Only ZIP files are allowed.";

                return RedirectToAction("Create");
            }

            // ======================================
            // STORAGE CONFIGURATION
            // ======================================

            string storageRoot =
                ConfigurationManager.AppSettings[
                    "DeploymentStorageRoot"];

            if (string.IsNullOrWhiteSpace(storageRoot) ||
                !Path.IsPathRooted(storageRoot))
            {
                TempData["DeploymentError"] =
                    "Deployment storage is not configured.";

                return RedirectToAction("Create");
            }

            string folder = Path.Combine(
                storageRoot,
                "packages");

            string fileName =
                Guid.NewGuid().ToString("N") + ".zip";

            string fullPath = Path.Combine(
                folder,
                fileName);

            bool databaseSaved = false;

            try
            {
                // Create protected storage directory.
                Directory.CreateDirectory(folder);

                // Save uploaded ZIP.
                file.SaveAs(fullPath);

                // Validate archive before recording request.
                ValidateZipArchive(fullPath);

                // ==================================
                // SAVE DEPLOYMENT TO SQL SERVER
                // ==================================

                var deployment = new Deployment
                {
                    ProjectId = model.ProjectId.Value,
                    DeploymentMethod = "ZIP",
                    PackagePath = fullPath,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                db.Deployments.Add(deployment);

                await db.SaveChangesAsync();

                databaseSaved = true;

                TempData["DeploymentSuccess"] =
                    "ZIP uploaded successfully. " +
                    "Your deployment is pending processing.";

                return RedirectToAction("Create");
            }
            catch (InvalidDataException)
            {
                TempData["DeploymentError"] =
                    "The ZIP package is invalid or unsafe.";
            }
            catch (IOException)
            {
                TempData["DeploymentError"] =
                    "Unable to save the ZIP file. " +
                    "Please try again.";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["DeploymentError"] =
                    "CloudHub does not have permission " +
                    "to access deployment storage.";
            }
            catch (Exception)
            {
                TempData["DeploymentError"] =
                    "An unexpected error occurred " +
                    "while uploading the ZIP file.";
            }
            finally
            {
                // Remove rejected files.
                // Keep accepted packages for the worker.
                if (!databaseSaved)
                {
                    try
                    {
                        if (System.IO.File.Exists(fullPath))
                            System.IO.File.Delete(fullPath);
                    }
                    catch (IOException)
                    {
                        // Storage cleanup should be logged.
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Storage cleanup should be logged.
                    }
                }
            }

            return RedirectToAction("Create");
        }

        // ==========================================
        // VALIDATE ZIP ARCHIVE
        // ==========================================

        private static void ValidateZipArchive(
            string fullPath)
        {
            using (var stream = new FileStream(
                fullPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read))
            using (var archive = new ZipArchive(
                stream,
                ZipArchiveMode.Read))
            {
                if (archive.Entries.Count == 0)
                {
                    throw new InvalidDataException(
                        "ZIP archive is empty.");
                }

                if (archive.Entries.Count >

                    MaxArchiveEntries)
                {
                    throw new InvalidDataException(
                        "ZIP contains too many files.");
                }

                long totalSize = 0;

                foreach (var entry in archive.Entries)
                {
                    string name = entry.FullName
                        .Replace('\\', '/');

                    if (string.IsNullOrWhiteSpace(name) ||
                        name.StartsWith("/") ||
                        name.Contains(":") ||
                        name.Split('/').Any(
                            part => part == ".."))
                    {
                        throw new InvalidDataException(
                            "Unsafe file path in ZIP.");
                    }

                    if (entry.Length > MaxEntrySize)
                    {
                        throw new InvalidDataException(
                            "ZIP contains an oversized file.");
                    }

                    totalSize = checked(
                        totalSize + entry.Length);

                    if (totalSize > MaxUncompressedSize)
                    {
                        throw new InvalidDataException(
                            "ZIP exceeds the size limit.");
                    }

                    // Do not extract here.
                    // Extraction belongs in an isolated worker.
                }
            }
        }

        // ==========================================
        // CLEAN UP DATABASE CONTEXT
        // ==========================================

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
