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
            modelBuilder.Entity<FlightReservation>().HasIndex(fr => new { fr.FlightId, fr.UserId }).IsUnique();

            modelBuilder.Entity<FlightReservation>().HasOne(fr => fr.Flight).WithMany(f => f.FlightReservations).HasForeignKey(fr => fr.FlightId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FlightReservation>().HasOne(fr => fr.User).WithMany(u => u.FlightReservations).HasForeignKey(fr => fr.UserId).OnDelete(DeleteBehavior.Cascade); 
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            if (ChangeTracker.Entries<FlightReservation>().Any(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                ValidateReservations();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void ValidateReservations()
        {
            var flightReservations = ChangeTracker.Entries<FlightReservation>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).Select(e => e.Entity).ToList();
            foreach (var reservation in flightReservations)
            {
                var totalReservedSeats = FlightReservations.Where(fr => fr.FlightId == reservation.FlightId && fr.Id != reservation.Id).Sum(fr => fr.NumberOfReservedSeats) + reservation.NumberOfReservedSeats;
                var flight = Flights.Single(f => f.Id == reservation.FlightId);
                if (totalReservedSeats > flight.Capacity)
                    throw new InvalidOperationException($"Can't reserve {reservation.NumberOfReservedSeats} seats for flight with ID {reservation.FlightId}. It's only {flight.Capacity - (totalReservedSeats-reservation.NumberOfReservedSeats)} free seats from all {flight.Capacity} seats.");
            }
        }
    }
}
