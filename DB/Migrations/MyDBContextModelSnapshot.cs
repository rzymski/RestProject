﻿// <auto-generated />
using System;
using DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DB.Migrations
{
    [DbContext(typeof(MyDBContext))]
    partial class MyDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DB.Entities.BaseEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("BaseEntity");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("DB.Entities.Flight", b =>
                {
                    b.HasBaseType("DB.Entities.BaseEntity");

                    b.Property<DateTime>("arrivalTime")
                        .HasColumnType("datetime2");

                    b.Property<long>("capacity")
                        .HasColumnType("bigint");

                    b.Property<string>("departureAirport")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("departureTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("destinationAirport")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("flightCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("Flight", (string)null);
                });

            modelBuilder.Entity("DB.Entities.FlightReservation", b =>
                {
                    b.HasBaseType("DB.Entities.BaseEntity");

                    b.Property<int?>("FlightId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<long>("numberOfReservedSeats")
                        .HasColumnType("bigint");

                    b.HasIndex("FlightId");

                    b.HasIndex("UserId");

                    b.ToTable("FlightReservation", (string)null);
                });

            modelBuilder.Entity("DB.Entities.User", b =>
                {
                    b.HasBaseType("DB.Entities.BaseEntity");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("login")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("DB.Entities.Flight", b =>
                {
                    b.HasOne("DB.Entities.BaseEntity", null)
                        .WithOne()
                        .HasForeignKey("DB.Entities.Flight", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DB.Entities.FlightReservation", b =>
                {
                    b.HasOne("DB.Entities.Flight", "Flight")
                        .WithMany("FlightReservations")
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DB.Entities.BaseEntity", null)
                        .WithOne()
                        .HasForeignKey("DB.Entities.FlightReservation", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DB.Entities.User", "User")
                        .WithMany("FlightReservations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Flight");

                    b.Navigation("User");
                });

            modelBuilder.Entity("DB.Entities.User", b =>
                {
                    b.HasOne("DB.Entities.BaseEntity", null)
                        .WithOne()
                        .HasForeignKey("DB.Entities.User", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DB.Entities.Flight", b =>
                {
                    b.Navigation("FlightReservations");
                });

            modelBuilder.Entity("DB.Entities.User", b =>
                {
                    b.Navigation("FlightReservations");
                });
#pragma warning restore 612, 618
        }
    }
}
