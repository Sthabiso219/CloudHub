using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Helpers;
using CloudHub.Models;
using CloudHub.Models.ViewModels;
using CloudHub.Services;




namespace CloudHub.Controllers
{
    public class CustomerAccountController : Controller
    {
        private readonly CloudHubDbContext db =
            new CloudHubDbContext();

        private readonly EmailService emailService =
            new EmailService();

        // ==========================================
        // UC01 - CUSTOMER REGISTRATION
        // ==========================================

        // GET: CustomerAccount/Register
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: CustomerAccount/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(
            RegisterCustomerViewModel model)
        {
            // Step 1: Validate registration fields
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Step 2: Normalize email address
            string email = model.EmailAddress
                .Trim()
                .ToLowerInvariant();

            // Step 3: Check duplicate email
            bool emailExists = await db.Customers
                .AnyAsync(c => c.EmailAddress == email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "EmailAddress",
                    "This email address is already registered.");

                return View(model);
            }

            // Step 4: Create customer
            Customer customer = new Customer
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                EmailAddress = email,
                PhoneNumber = model.PhoneNumber.Trim(),

                // Hash password before saving
                PasswordHash = Crypto.HashPassword(
                    model.Password),

                IsEmailVerified = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Step 5: Generate secure verification token
            string rawToken =
                VerificationTokenService.GenerateToken();

            EmailVerificationToken verificationToken =
                new EmailVerificationToken
                {
                    Customer = customer,

                    TokenHash =
                        VerificationTokenService.HashToken(
                            rawToken),

                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    UsedAt = null
                };

            // Step 6: Save customer and token together
            db.Customers.Add(customer);

            db.EmailVerificationTokens.Add(
                verificationToken);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(
                    "",
                    "Registration could not be completed. " +
                    "Please check your details and try again.");

                return View(model);
            }

            // Step 7: Generate verification URL
            string verificationLink = Url.Action(
                "VerifyEmail",
                "CustomerAccount",
                new { token = rawToken },
                protocol: Request.Url.Scheme
            );

            // Step 8: Send verification email
            try
            {
                await emailService.SendVerificationEmailAsync(
                    customer.EmailAddress,
                    customer.FirstName,
                    verificationLink
                );

                TempData["EmailStatus"] =
                    "A verification email has been sent. " +
                    "Please check your inbox.";
            }
            catch (SmtpException)
            {
                // Account remains unverified
                TempData["EmailStatus"] =
                    "Your account was created, but we " +
                    "could not send the verification email. " +
                    "Please contact support.";
            }
            catch (Exception)
            {
                // Avoid displaying internal email errors
                TempData["EmailStatus"] =
                    "Your account was created, but email " +
                    "delivery could not be confirmed.";
            }



            // Step 9: Redirect to success page
            return RedirectToAction(
                "RegistrationSuccess");
        }

        // ==========================================
        // REGISTRATION SUCCESS
        // ==========================================

        // GET: CustomerAccount/RegistrationSuccess
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegistrationSuccess()
        {
            return View();
        }

        // ==========================================
        // UC02 - VERIFY EMAIL ADDRESS
        // ==========================================

        // GET: CustomerAccount/VerifyEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(
            string token)
        {
            // Step 1: Validate token input
            if (string.IsNullOrWhiteSpace(token))
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message =
                    "The verification link is invalid.";

                return View();
            }

            // Step 2: Hash received token
            string tokenHash =
                VerificationTokenService.HashToken(
                    token);

            // Step 3: Find token in database
            EmailVerificationToken verification =
                await db.EmailVerificationTokens
                    .Include(v => v.Customer)
                    .FirstOrDefaultAsync(
                        v => v.TokenHash == tokenHash);

            if (verification == null)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message =
                    "The verification link is invalid.";

                return View();
            }

            // Step 4: Check whether token was used
            if (verification.UsedAt.HasValue)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message =
                    "This verification link has " +
                    "already been used.";

                return View();
            }

            // Step 5: Check token expiry
            if (verification.ExpiresAt <= DateTime.UtcNow)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message =
                    "This verification link has expired.";

                return View();
            }

            // Step 6: Check customer account
            if (verification.Customer == null)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message =
                    "The associated account was not found.";

                return View();
            }

            // Step 7: Check if already verified
            if (verification.Customer.IsEmailVerified)
            {
                ViewBag.IsSuccess = true;
                ViewBag.Message =
                    "Your email address is already verified.";

                return View();
            }

            // Step 8: Update verification status
            verification.Customer.IsEmailVerified = true;

            verification.UsedAt = DateTime.UtcNow;

            // Step 9: Save changes
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ViewBag.IsSuccess = false;
                ViewBag.Message =
                    "We could not complete verification. " +
                    "Please try again.";

                return View();
            }

            // Step 10: Display success
            ViewBag.IsSuccess = true;
            ViewBag.Message =
                "Your email address has been verified " +
                "successfully. You can now log in.";

            return View();
        }


        // ==========================================
        // UC03 - CUSTOMER LOGIN
        // ==========================================

        // GET: CustomerAccount/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: CustomerAccount/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(
            LoginCustomerViewModel model,
            string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            string email = model.EmailAddress
                .Trim()
                .ToLowerInvariant();

            // Find customer using email
            Customer customer = await db.Customers
                .FirstOrDefaultAsync(c => c.EmailAddress == email);

            // Validate credentials
            // Use a dummy hash when the account does not exist
            // to reduce account-enumeration timing differences.
            const string dummyPasswordHash =
                "AQAAAAEAACcQAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";

            bool validPassword = false;

            if (customer != null)
            {
                validPassword = Crypto.VerifyHashedPassword(
                    customer.PasswordHash,
                    model.Password);
            }
            else
            {
                // Do not attempt verification against a fabricated
                // hash in production. Use a valid precomputed dummy
                // hash if timing equalization is required.
            }

            if (customer == null || !validPassword)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email address or password.");

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Check account status
            if (!customer.IsActive)
            {
                ModelState.AddModelError(
                    "",
                    "Your account is inactive. Please contact support.");

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Check email verification
            if (!customer.IsEmailVerified)
            {
                ModelState.AddModelError(
                    "",
                    "Please verify your email address before logging in.");

                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Create authentication cookie
            FormsAuthentication.SetAuthCookie(
                customer.EmailAddress,
                model.RememberMe);

            // Redirect only to a safe local URL
            if (!string.IsNullOrEmpty(returnUrl)
                && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Temporary destination until customer dashboard exists
            return RedirectToAction("Index", "CustomerDashboard");
        }


        // ==========================================
        // UC04 - CUSTOMER LOGOUT
        // ==========================================

        // POST: CustomerAccount/Logout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // Step 1: End Forms Authentication
            FormsAuthentication.SignOut();

            // Step 2: Clear session information
            Session.Clear();
            Session.Abandon();

            // Step 3: Expire authentication cookie
            if (Request.Cookies[
                FormsAuthentication.FormsCookieName] != null)
            {
                HttpCookie authCookie = new HttpCookie(
                    FormsAuthentication.FormsCookieName,
                    "");

                authCookie.Expires =
                    DateTime.UtcNow.AddDays(-1);

                authCookie.HttpOnly = true;
                authCookie.Secure = Request.IsSecureConnection;
                authCookie.Path =
                    FormsAuthentication.FormsCookiePath;

                Response.Cookies.Add(authCookie);
            }

            // Step 4: Prevent caching of logout response
            Response.Cache.SetCacheability(
                HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            // Step 5: Redirect to Login
            return RedirectToAction(
                "Login",
                "CustomerAccount");
        }


        // ==========================================
        // UC05 - FORGOT PASSWORD
        // ==========================================

        // GET: CustomerAccount/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: CustomerAccount/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.EmailAddress
                .Trim()
                .ToLowerInvariant();

            Customer customer = await db.Customers
                .FirstOrDefaultAsync(c => c.EmailAddress == email);

            // Always show the same confirmation.
            // This prevents revealing registered email addresses.
            if (customer != null && customer.IsActive)
            {
                string token =
                    VerificationTokenService.GenerateToken();

                var resetToken = new PasswordResetToken
                {
                    CustomerId = customer.CustomerId,
                    TokenHash =
                        VerificationTokenService.HashToken(token),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    UsedAt = null
                };

                db.PasswordResetTokens.Add(resetToken);
                await db.SaveChangesAsync();

                string resetLink = Url.Action(
                    "ResetPassword",
                    "CustomerAccount",
                    new { token = token },
                    protocol: Request.Url.Scheme);

                try
                {
                    await emailService.SendPasswordResetEmailAsync(
                        customer.EmailAddress,
                        customer.FirstName,
                        resetLink);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Password reset email failed: " +
                        ex.GetType().Name);

                    // Do not reveal delivery details to the visitor.
                }
            }

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        // GET: CustomerAccount/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        // ==========================================
        // UC05 - RESET PASSWORD
        // ==========================================

        // GET: CustomerAccount/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return View("InvalidResetLink");
            }

            string tokenHash =
                VerificationTokenService.HashToken(token);

            var reset = await db.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.TokenHash == tokenHash &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow);

            if (reset == null)
            {
                return View("InvalidResetLink");
            }

            return View(new ResetPasswordViewModel
            {
                Token = token
            });
        }

        // POST: CustomerAccount/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string tokenHash =
                VerificationTokenService.HashToken(model.Token);

            var reset = await db.PasswordResetTokens
                .Include(t => t.Customer)
                .FirstOrDefaultAsync(t =>
                    t.TokenHash == tokenHash &&
                    t.UsedAt == null &&
                    t.ExpiresAt > DateTime.UtcNow);

            if (reset == null ||
                reset.Customer == null ||
                !reset.Customer.IsActive)
            {
                return View("InvalidResetLink");
            }

            DateTime now = DateTime.UtcNow;

            // Hash the new password
            reset.Customer.PasswordHash =
                Crypto.HashPassword(model.NewPassword);

            // Invalidate all outstanding reset links
            var outstandingTokens = await db.PasswordResetTokens
                .Where(t =>
                    t.CustomerId == reset.CustomerId &&
                    t.UsedAt == null)
                .ToListAsync();

            foreach (var item in outstandingTokens)
            {
                item.UsedAt = now;
            }

            await db.SaveChangesAsync();

            // Clear authentication cookie if the customer
            // is currently signed in on this browser.
            System.Web.Security.FormsAuthentication.SignOut();

            return RedirectToAction("ResetPasswordConfirmation");
        }

        // GET: CustomerAccount/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }



        // ==========================================
        // DISPOSE DATABASE CONTEXT
        // ==========================================

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
