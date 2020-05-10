using Microsoft.EntityFrameworkCore;

namespace VirusTracker.Models
{
    public partial class TracingContext : DbContext
    {
        public TracingContext()
        {
        }

        public TracingContext(DbContextOptions<TracingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Business> Business { get; set; }
        public virtual DbSet<Tracker> Tracker { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Data Source=GARYLAPTOP\\SQLEXPRESS01;Initial Catalog=ContactTracing;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Business>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<Tracker>(entity =>
            {
                // entity.HasNoKey();
                entity.Property(d => d.Id).HasDefaultValueSql("(newid())");

                /* entity.HasOne(d => d.BusinessIdfkNavigation)
                     .WithMany()
                     .HasForeignKey(d => d.BusinessIdfk)
                     .HasConstraintName("FK_Tracker_Business");*/
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
