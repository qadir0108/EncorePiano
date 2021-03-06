﻿using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;
using WFP.ICT.Data.Migrations;

namespace WFP.ICT.Data.Entities
{
    public class WFPICTContext : IdentityDbContext<WFPUser>
    {
        public WFPICTContext() : base("WFPICTContext")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = false;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<WFPICTContext, Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // The rest should not be needed - it should be done by conventions
            //modelBuilder.Entity<Order>()
            //    .HasOptional(s => s.Assignment)
            //    .WithRequired(si => si.Order);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<APIRequestLog> APIRequestLogs { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<AspNetClaims> Claims { get; set; }
        public DbSet<AspNetRoleClaims> RoleClaims { get; set; }

        // Order
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PianoType> PianoTypes { get; set; }
        public DbSet<Piano> Pianos { get; set; }
        public DbSet<Leg> Legs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PianoCharges> PianoCharges { get; set; }

        public DbSet<OrderCharges> OrderCharges { get; set; }
        public DbSet<OrderBilling> OrderBillings { get; set; }
        public DbSet<PianoQuote> PianoQuotes { get; set; }

        // Lookups
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<DriverLogin> DriverLogins { get; set; }

        // Proof of Pickup and Delivery
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentRoute> AssignmentRoutes { get; set; }
        public DbSet<TripStatus> TripStatuses { get; set; }
        public DbSet<PianoStatus> PianoStatuses { get; set; }
        public DbSet<Proof> Proofs { get; set; }
        public DbSet<PianoPicture> PianoPictures { get; set; }
        public DbSet<PianoMake> PianoMake { get; set; }
        public DbSet<PianoSize> PianoSize { get; set; }
        public DbSet<PianoFinish> PianoFinish{ get; set; }

        // Accounting
        public DbSet<ClientInvoice> Invoices { get; set; }
        public DbSet<ClientPayment> Payments { get; set; }
        public DbSet<ClientPaymentCard> Cards { get; set; }

        public static WFPICTContext Create()
        {
            return new WFPICTContext();
        }

        #region Overrided
        public override int SaveChanges()
        {
            try
            {
                base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
            return -1;
        }

        private Exception HandleDbUpdateException(DbUpdateException dbu)
        {
            var builder = new StringBuilder("A DbUpdateException was caught while saving changes. ");

            try
            {
                foreach (var result in dbu.Entries)
                {
                    builder.AppendFormat("Type: {0} was part of the problem. ", result.Entity.GetType().Name);
                }
            }
            catch (Exception e)
            {
                builder.Append("Error parsing DbUpdateException: " + e.ToString());
            }

            string message = builder.ToString();
            return new Exception(message, dbu);
        }
        #endregion
    }
}
