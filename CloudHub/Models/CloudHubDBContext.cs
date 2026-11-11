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
        public DbSet<HostingPlan> HostingPlans { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionHistory> SubscriptionHistories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .HasRequired(s => s.HostingPlan)
                .WithMany(p => p.Subscriptions)
                .HasForeignKey(s => s.HostingPlanId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
