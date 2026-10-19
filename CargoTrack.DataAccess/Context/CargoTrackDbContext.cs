using CargoTrack.Entity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Context
{
    public class CargoTrackDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public CargoTrackDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ---------- Cargo ilişkileri ----------
            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.Sender)
                .WithMany(m => m.SentCargos)
                .HasForeignKey(c => c.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.Receiver)
                .WithMany(m => m.ReceivedCargos)
                .HasForeignKey(c => c.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.OriginBranch)
                .WithMany(m => m.OriginCargos)
                .HasForeignKey(c => c.OriginBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.DestinationBranch)
                .WithMany(m => m.DestinationCargos)
                .HasForeignKey(c => c.DestinationBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.CurrentBranch)
                .WithMany()
                .HasForeignKey(c => c.CurrentBranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.CurrentTransferCenter)
                .WithMany()
                .HasForeignKey(c => c.CurrentTransferCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cargo>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            // Cargo - Delivery (bire-bir)
            modelBuilder.Entity<Cargo>()
                .HasOne(c => c.Delivery)
                .WithOne(d => d.Cargo)
                .HasForeignKey<Delivery>(d => d.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- CargoMovement ilişkileri ----------
            modelBuilder.Entity<CargoMovement>()
                .HasOne(m => m.Cargo)
                .WithMany(c => c.CargoMovements)
                .HasForeignKey(m => m.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CargoMovement>()
                .HasOne(m => m.Branch)
                .WithMany()
                .HasForeignKey(m => m.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CargoMovement>()
                .HasOne(m => m.TransferCenter)
                .WithMany(t => t.CargoMovements)
                .HasForeignKey(m => m.TransferCenterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CargoMovement>()
                .HasOne(m => m.Employee)
                .WithMany(e => e.CargoMovements)
                .HasForeignKey(m => m.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Delivery ilişkileri ----------
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Employee)
                .WithMany(e => e.Deliveries)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- DeliveryException ilişkileri ----------
            modelBuilder.Entity<DeliveryException>()
                .HasOne(x => x.Cargo)
                .WithMany(c => c.DeliveryExceptions)
                .HasForeignKey(x => x.CargoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DeliveryException>()
                .HasOne(x => x.Employee)
                .WithMany(e => e.DeliveryExceptions)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Employee ilişkileri ----------
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.AppUser)
                .WithMany()
                .HasForeignKey(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- AppUser - Branch (Manager/Employee) ----------
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Branch)
                .WithMany()
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- TransferCenter - City ----------
            modelBuilder.Entity<TransferCenter>()
                .HasOne(t => t.City)
                .WithMany()
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- CargoPrice ----------
            modelBuilder.Entity<CargoPrice>()
                .Property(p => p.BasePrice).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CargoPrice>()
                .Property(p => p.PricePerKg).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CargoPrice>()
                .Property(p => p.PricePerDesi).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CargoPrice>()
                .Property(p => p.DifferentCityFee).HasColumnType("decimal(18,2)");

            // ---------- Soft Delete Global Query Filters ----------
            modelBuilder.Entity<Cargo>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<CargoMovement>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Branch>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<City>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<TransferCenter>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Delivery>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<DeliveryException>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<CargoPrice>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<About>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Address>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<ContactInfo>().HasQueryFilter(x => !x.IsDeleted);
            // AuditLog soft delete uygulanmaz (kalıcı denetim kaydı)

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<About> Abouts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CargoMovement> CargoMovements { get; set; }
        public DbSet<TransferCenter> TransferCenters { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryException> DeliveryExceptions { get; set; }
        public DbSet<CargoPrice> CargoPrices { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
    }
}
