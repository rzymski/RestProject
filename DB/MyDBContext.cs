using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FlightReservation> FlightReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<BaseEntity>();

            //modelBuilder.Entity<BaseEntity>().Property(p => p.Id).UseIdentityColumn(seed: 1, increment: 1);
            modelBuilder.HasSequence<int>("EntitySeq", schema: "shared").StartsAt(1).IncrementsBy(1);

            modelBuilder.Entity<Flight>().ToTable("Flight");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<FlightReservation>().ToTable("FlightReservation");

            modelBuilder.Entity<Flight>().HasKey(f => f.Id);
            modelBuilder.Entity<Flight>().Property(f => f.Id).HasDefaultValueSql("NEXT VALUE FOR shared.EntitySeq");
            modelBuilder.Entity<Flight>().HasIndex(f => f.FlightCode).IsUnique();

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Id).HasDefaultValueSql("NEXT VALUE FOR shared.EntitySeq");
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();

            modelBuilder.Entity<FlightReservation>().HasKey(fr => fr.Id);
            modelBuilder.Entity<FlightReservation>().Property(fr => fr.Id).HasDefaultValueSql("NEXT VALUE FOR shared.EntitySeq");

            modelBuilder.Entity<FlightReservation>().HasOne(fr => fr.Flight).WithMany(f => f.FlightReservations).HasForeignKey(fr => fr.FlightId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FlightReservation>().HasOne(fr => fr.User).WithMany(u => u.FlightReservations).HasForeignKey(fr => fr.UserId).OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
