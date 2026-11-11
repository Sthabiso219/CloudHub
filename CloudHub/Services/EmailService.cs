
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CloudHub.Services
{
    public class EmailService
    {
        // ==========================================
        // UC02 - SEND VERIFICATION EMAIL
        // ==========================================
        public async Task SendVerificationEmailAsync(
            string recipientEmail,
            string firstName,
            string verificationLink)
        {
            string subject =
                "CloudHub - Verify Your Email Address";

            string body =
                "Hello " + firstName + ",\n\n" +
                "Welcome to CloudHub!\n\n" +
                "Thank you for creating your account.\n\n" +
                "Please verify your email address by " +
                "opening the link below:\n\n" +
                verificationLink + "\n\n" +
                "This link expires in 24 hours.\n\n" +
                "If you did not create this account, " +
                "please ignore this email.\n\n" +
                "Kind regards,\n" +
                "CloudHub Team";

            await SendEmailAsync(
                recipientEmail,
                subject,
                body);
        }

        // ==========================================
        // UC05 - SEND PASSWORD RESET EMAIL
        // ==========================================
        public async Task SendPasswordResetEmailAsync(
            string recipientEmail,
            string firstName,
            string resetLink)
        {
            string subject =
                "CloudHub - Reset Your Password";

            string body =
                "Hello " + firstName + ",\n\n" +
                "We received a request to reset your " +
                "CloudHub account password.\n\n" +
                "Please open the following link to " +
                "create a new password:\n\n" +
                resetLink + "\n\n" +
                "This link expires in 30 minutes.\n\n" +
                "If you did not request a password reset, " +
                "you can safely ignore this email.\n\n" +
                "Kind regards,\n" +
                "CloudHub Team";

            await SendEmailAsync(
                recipientEmail,
                subject,
                body);
        }

        // ==========================================
        // SHARED SMTP EMAIL SENDING METHOD
        // ==========================================
        private async Task SendEmailAsync(
            string recipientEmail,
            string subject,
            string body)
        {
            string host = ConfigurationManager
                .AppSettings["SmtpHost"];

            int port = int.Parse(
                ConfigurationManager
                    .AppSettings["SmtpPort"]);

            string username = ConfigurationManager
                .AppSettings["SmtpUsername"];

            string password = ConfigurationManager
                .AppSettings["SmtpPassword"];

            string fromEmail = ConfigurationManager
                .AppSettings["SmtpFromEmail"];

            bool enableSsl = bool.Parse(
                ConfigurationManager
                    .AppSettings["SmtpEnableSsl"]);

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(
                    fromEmail,
                    "CloudHub");

                message.To.Add(recipientEmail);

                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = false;

                using (SmtpClient client =
                    new SmtpClient(host, port))
                {
                    client.EnableSsl = enableSsl;

                    client.UseDefaultCredentials = false;

                    client.Credentials =
                        new NetworkCredential(
                            username,
                            password);

                    client.DeliveryMethod =
                        SmtpDeliveryMethod.Network;

                    await client.SendMailAsync(message);
                }
            }
        }
    }
}
