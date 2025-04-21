using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Vigen_Repository.Models
{
    public partial class vigendbContext : DbContext
    {
        public vigendbContext()
        {
        }

        public vigendbContext(DbContextOptions<vigendbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Notify> Notifies { get; set; } = null!;
        public virtual DbSet<Organization> Organizations { get; set; } = null!;
        public virtual DbSet<OrganizationType> OrganizationTypes { get; set; } = null!;
        public virtual DbSet<Site> Sites { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<ViolenceType> ViolenceTypes { get; set; } = null!;
        public virtual DbSet<ViolenceTypesOrganization> ViolenceTypesOrganizations { get; set; } = null!;
        public virtual DbSet<Poll> Polls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ViolenceTypesOrganization>(x =>
            {
                x.HasKey(y => new { y.OrganizationTypeId, y.IdViolence });
            });

            modelBuilder.Entity<Site>(x =>
            {
                x.HasKey(y => new { y.Id, y.Nit });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
