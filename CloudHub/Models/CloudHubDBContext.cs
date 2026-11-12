using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CloudHub.Models
{
    public class CloudHubDbContext : DbContext
    {
        public CloudHubDbContext()
            : base("CloudHubConnection")
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens{ get; set; }
        public DbSet<PasswordResetToken>PasswordResetTokens{ get; set; }
    }
}
