
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CloudHub.Services
{
    public class EmailService
    {
        public async Task SendVerificationEmailAsync(
            string recipientEmail,
            string firstName,
            string verificationLink)
        {
            string host = ConfigurationManager
                .AppSettings["SmtpHost"];

            int port = int.Parse(ConfigurationManager
                .AppSettings["SmtpPort"]);

            string username = ConfigurationManager
                .AppSettings["SmtpUsername"];

            string password = ConfigurationManager
                .AppSettings["SmtpPassword"];

            string fromEmail = ConfigurationManager
                .AppSettings["SmtpFromEmail"];

            bool enableSsl = bool.Parse(ConfigurationManager
                .AppSettings["SmtpEnableSsl"]);

            using (MailMessage message = new MailMessage())
            {
                message.From = new MailAddress(
                    fromEmail, "CloudHub");

                message.To.Add(recipientEmail);

                message.Subject =
                    "CloudHub - Verify Your Email Address";

                message.Body =
                    "Hello " + firstName + ",\n\n" +
                    "Welcome to CloudHub!\n\n" +
                    "Thank you for creating an account.\n\n" +
                    "To verify your email address, " +
                    "please click the link below:\n\n" +
                    verificationLink + "\n\n" +
                    "This verification link will expire " +
                    "after 24 hours.\n\n" +
                    "If you did not register with CloudHub, " +
                    "please ignore this email.\n\n" +
                    "Kind regards,\n" +
                    "CloudHub Team";

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
