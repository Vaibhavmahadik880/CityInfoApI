
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointOfInterests { get; set; } = null!;
        public DbSet<UserDetails> UserDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Adding an index on the Name column in the City table
            modelBuilder.Entity<City>()
                .HasIndex(c => c.Name)
                .HasDatabaseName("IX_Cities_Name");

            // Adding an index on the Description column in the City table
            modelBuilder.Entity<City>()
                .HasIndex(c => c.Description)
                .HasDatabaseName("IX_Cities_Description");
            modelBuilder.Entity<City>()
                .HasData(
                    new City("Lucknow")
                    {
                        Id = 1,
                        Description = "It is known for its Mughal architecture and cuisine."
                    },
                    new City("Mumbai")
                    {
                        Id = 2,
                        Description = "The financial capital of India, famous for Bollywood."
                    },
                    new City("Kolkata")
                    {
                        Id = 3,
                        Description = "Known for its cultural heritage and colonial architecture."
                    },
                    new City("Jaipur")
                    {
                        Id = 4,
                        Description = "Famous for its palaces and the pink buildings in the old city."
                    },
                    new City("Chennai")
                    {
                        Id = 5,
                        Description = "Known for its beaches and Dravidian-style temples."
                    }
                );

            modelBuilder.Entity<PointOfInterest>().HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The most visited urban park."
                },
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "A 103-story skyscraper located in midtown Manhattan."
                },
                new PointOfInterest("Red Fort")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "A historic fort and UNESCO World Heritage site."
                },
                new PointOfInterest("India Gate")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "A war memorial and popular tourist spot."
                },
                new PointOfInterest("Lalbagh Botanical Garden")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "A famous botanical garden with a glass house."
                },
                new PointOfInterest("Bangalore Palace")
                {
                    Id = 6,
                    CityId = 3,
                    Description = "A palace known for its Tudor-style architecture."
                },
                new PointOfInterest("Victoria Memorial")
                {
                    Id = 7,
                    CityId = 4,
                    Description = "A large marble building and museum."
                },
                new PointOfInterest("Howrah Bridge")
                {
                    Id = 8,
                    CityId = 4,
                    Description = "A cantilever bridge over the Hooghly River."
                },
                new PointOfInterest("Marina Beach")
                {
                    Id = 9,
                    CityId = 5,
                    Description = "The longest natural urban beach in India."
                },
                new PointOfInterest("Kapaleeshwarar Temple")
                {
                    Id = 10,
                    CityId = 5,
                    Description = "A famous Hindu temple dedicated to Lord Shiva."
                }
            );
            modelBuilder.Entity<UserDetails>().HasData(
            new UserDetails
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "john.doe",
                Password = "Password123" // Remember to hash passwords in real applications
            },
            new UserDetails
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Username = "jane.smith",
                Password = "Password456" // Remember to hash passwords in real applications
            },
            new UserDetails
            {
                Id = 3,
                FirstName = "Robert",
                LastName = "Brown",
                Username = "robert.brown",
                Password = "Password789" // Remember to hash passwords in real applications
            }
            );


            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlite("CityInfoDBConnectionString");
        //    }
        //}
    }
}
