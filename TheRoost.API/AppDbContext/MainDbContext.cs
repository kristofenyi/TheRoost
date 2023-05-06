using Microsoft.EntityFrameworkCore;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;

namespace TheRoost.API.AppDbContext
{
    public class MainDbContext : DbContext
    {
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ResetUserPassword> ResetUserPasswords { get; set; }
        
        public MainDbContext(DbContextOptions options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotel>()
                .HasBaseType<Accommodation>();

            modelBuilder.Entity<Hostel>()
                .HasBaseType<Accommodation>();

            modelBuilder.Entity<Apartment>()
                .HasBaseType<Accommodation>();

            modelBuilder.Entity<Guesthouse>()
                .HasBaseType<Accommodation>();

            modelBuilder.Entity<Review>()
                .HasOne(b => b.User)
                .WithMany(a => a.Reviews)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reservation>()
                .HasOne(b => b.Room)
                .WithMany(a => a.Reservations)                
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Reservation>()
                .Property(p => p.IsCancelled)
                .HasDefaultValue(false);

            modelBuilder.Entity<Reservation>()
                .HasOne(b => b.User)
                .WithMany(a => a.Reservations)
                .OnDelete(DeleteBehavior.NoAction);

            //// Seed accommodations
            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    ID = 1,
                    Phone = "555-1234",
                    Name = "Hotel 1",
                    Address = "123 Main St.",
                    City = "NY",
                    Region = "N/A",
                    Country = "USA",
                    Description = "A nice hotel in a great location.",
                    AccommodationManagerID = 2,
                    Logo = new Uri("https://example.com/logo.png"),
                    NumberOfFloors = 8,
                    TimeZoneName = "US Eastern Standard Time",
                    StarRating = 4,
                    UserRating = 4.5
                }
            );

            modelBuilder.Entity<Hotel>().HasData(
                new Hotel
                {
                    ID = 2,
                    Phone = "555-456-956",
                    Name = "Hotel 2",
                    Address = "124 Baker St.",
                    City = "NY",
                    Region = "N/A",
                    Country = "USA",
                    Description = "A kinda nice hotel in an OK location.",
                    AccommodationManagerID = 2,
                    Logo = new Uri("https://example.com/logo2.png"),
                    NumberOfFloors = 10,
                    TimeZoneName = "US Eastern Standard Time",
                    StarRating = 3
                }
            );

            modelBuilder.Entity<Apartment>().HasData(
               new Apartment
               {
                   ID = 3,
                   Phone = "666-845-698",
                   Name = "Luxury Penthouse",
                   Address = "456 Park Ave.",
                   City = "NY",
                   Region = "N/A",
                   Country = "USA",
                   Description = "Live on top of the world",
                   AccommodationManagerID = 3,
                   Logo = new Uri("https://example.com/logo3.png"),
                   FloorNumber = 3,
                   TimeZoneName = "US Eastern Standard Time"
               }
            );

            modelBuilder.Entity<Guesthouse>().HasData(
               new Guesthouse
               {
                   ID = 4,
                   Phone = "555-431-487",
                   Name = "Eco Lodge Guesthouse",
                   Address = "Street x1",
                   City = "Canggu",
                   Region = "Bali",
                   Country = "Indonesia",
                   Description = "On the island of Bali, a secluded guesthouse",
                   AccommodationManagerID = 2,
                   Logo = new Uri("https://example.com/logo4.png"),
                   TimeZoneName = "Singapore Standard Time",
                   UserRating = 2,
                   StarRating = 2
               }
            );

            modelBuilder.Entity<Hostel>().HasData(
               new Hostel
               {
                   ID = 5,
                   Phone = "724-082-788",
                   Name = "Darex Luxury Offices",
                   Address = "Václavské nám. 837",
                   City = "Praha",
                   Region = "Praha",
                   Country = "Czech republic",
                   Description = "Beautiful central location and luxurious offices you can sleep in!",
                   AccommodationManagerID = 2,
                   Logo = new Uri("https://example.com/logo5.png"),
                   TimeZoneName = "Central European Standard Time",
                   UserRating = 2,
                   StarRating = 5
               }
            );

            ////Seed Room
            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    ID = 1,
                    RoomNumber = 101,
                    Description = "Delux room",
                    Price = 110.0,
                    MaxCapacity = 5,
                    AccommodationID = 1,
                    RoomTypeID = 2
                });

            modelBuilder.Entity<Room>().HasData(
            new Room
                {
                    ID = 2,
                    RoomNumber = 102,
                    Description = "A great room",
                    Price = 110.0,
                    MaxCapacity = 4,
                    AccommodationID = 1,
                    RoomTypeID = 2
                });

            modelBuilder.Entity<Room>().HasData(
            new Room
                {
                    ID = 3,
                    RoomNumber = 103,
                    Description = "A cute room",
                    Price = 25.0,
                    MaxCapacity = 3,
                    AccommodationID = 1,
                    RoomTypeID = 1
                });

            modelBuilder.Entity<Room>().HasData(
            new Room
                {
                    ID = 4,
                    RoomNumber = 104,
                    Description = "Pretty room",
                    Price = 420.0,
                    MaxCapacity = 4,
                    AccommodationID = 1,
                    RoomTypeID = 2
                });

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    ID = 5,
                    RoomNumber = 105,
                    Description = "Important and mysterious room",
                    Price = 69.0,
                    MaxCapacity = 3,
                    AccommodationID = 1,
                    RoomTypeID = 3
                });

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    ID = 6,
                    RoomNumber = 3,
                    Description = "An interesting room",
                    Price = 420.0,
                    MaxCapacity = 6,
                    AccommodationID = 2,
                    RoomTypeID = 3
                });

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    ID = 7,
                    RoomNumber = 7,
                    Description = "A blue room",
                    Price = 10.0,
                    MaxCapacity = 2,
                    AccommodationID = 3,
                    RoomTypeID = 1
                });

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    ID = 8,
                    RoomNumber = 91,
                    Description = "A room with a bird painting",
                    Price = 66.0,
                    MaxCapacity = 4,
                    AccommodationID = 4,
                    RoomTypeID = 2
                });

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    ID = 9,
                    RoomNumber = 10,
                    Description = "A daunting room",
                    Price = 45.0,
                    MaxCapacity = 4,
                    AccommodationID = 5,
                    RoomTypeID = 1
                });

            modelBuilder.Entity<Room>().HasData(
                new Room
                {
                    ID = 10,
                    RoomNumber = 11,
                    Description = "A room with Aleš inside of it",
                    Price = 5.0,
                    MaxCapacity = 10,
                    AccommodationID = 5,
                    RoomTypeID = 2
                });

            //// Seed reservations
            //Past reservations
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000001"),
                    UserID = 4,
                    AccommodationID = 1,
                    RoomID = 5,
                    CheckInDate = new DateTime(2022, 12, 25),
                    CheckOutDate = new DateTime(2022, 12, 30),
                    NumberOfGuests = 2,
                    IsCancelled = false
                });

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000002"),
                    UserID = 4,
                    AccommodationID = 1,
                    RoomID = 5,
                    CheckInDate = new DateTime(2023, 1, 1),
                    CheckOutDate = new DateTime(2023, 1, 4),
                    NumberOfGuests = 3,
                    IsCancelled = false
                });

            //Future reservation
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000003"),
                    UserID = 4,
                    AccommodationID = 4,
                    RoomID = 10,
                    CheckInDate = new DateTime(2023, 5, 13),
                    CheckOutDate = new DateTime(2023, 5, 20),
                    NumberOfGuests = 8,
                    IsCancelled = false
                });

            //Cancelled reservation
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000004"),
                    UserID = 4,
                    AccommodationID = 3,
                    RoomID = 7,
                    CheckInDate = new DateTime(2024, 11, 17),
                    CheckOutDate = new DateTime(2024, 11, 18),
                    NumberOfGuests = 2,
                    IsCancelled = true
                });

            //Current active reservation (user staying it the accommodation at this moment)
            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000005"),
                    UserID = 4,
                    AccommodationID = 2,
                    RoomID = 10,
                    CheckInDate = new DateTime(2024, 4, 25),
                    CheckOutDate = DateTime.Now.Date.AddDays(5),
                    NumberOfGuests = 7,
                    IsCancelled = false
                });

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000006"),
                    UserID = 4,
                    AccommodationID = 5,
                    RoomID = 10,
                    CheckInDate = new DateTime(2024, 4, 25),
                    CheckOutDate = new DateTime(2024, 5, 8),
                    NumberOfGuests = 7,
                    IsCancelled = false
                });

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000007"),
                    UserID = 5,
                    AccommodationID = 5,
                    RoomID = 6,
                    CheckInDate = new DateTime(2024, 3, 12),
                    CheckOutDate = new DateTime(2024, 3, 14),
                    NumberOfGuests = 5,
                    IsCancelled = true
                });

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000008"),
                    UserID = 5,
                    AccommodationID = 4,
                    RoomID = 8,
                    CheckInDate = new DateTime(2022, 9, 14),
                    CheckOutDate = new DateTime(2022, 9, 18),
                    NumberOfGuests = 4,
                    IsCancelled = false
                });

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000009"),
                    UserID = 4,
                    AccommodationID = 4,
                    RoomID = 8,
                    CheckInDate = new DateTime(2022, 7, 13),
                    CheckOutDate = new DateTime(2022, 7, 23),
                    NumberOfGuests = 4,
                    IsCancelled = false
                });

            modelBuilder.Entity<Reservation>().HasData(
                new Reservation
                {
                    ID = new Guid("152821e6-8a86-418b-d281-08daf46d9e21"),
                    UserID = 4,
                    AccommodationID = 2,
                    RoomID = 5,
                    CheckInDate = new DateTime(2024, 7, 13),
                    CheckOutDate = new DateTime(2024, 7, 23),
                    NumberOfGuests = 4,
                    IsCancelled = false
                });

            ////Seed Review
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000010"),
                    UserID = 4,
                    AccommodationID = 1,
                    Rating = 4,
                    Text = "Great hotel",
                    DateCreated = DateTime.Now
                });

            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000011"),
                    UserID = 4,
                    AccommodationID = 5,
                    Rating = 2,
                    Text = "It was a disaster :(",
                    DateCreated = DateTime.Now
                });

            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000012"),
                    UserID = 5,
                    AccommodationID = 1,
                    Rating = 5,
                    Text = "Sweet!",
                    DateCreated = DateTime.Now
                });

            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    ID = new Guid("00000000-0000-0000-0000-000000000013"),
                    UserID = 5,
                    AccommodationID = 4,
                    Rating = 2,
                    Text = "Ugh!",
                    DateCreated = DateTime.Now
                });

            modelBuilder.Entity<Role>().HasData(
              new Role { ID = 1, Title = "Admin" },
              new Role { ID = 2, Title = "User" },
              new Role { ID = 3, Title = "Hotel Manager" }
            );

            // Seed the RoomTypes table with initial data
            modelBuilder.Entity<RoomType>().HasData(
                new RoomType { ID = 1, Name = "1 full bed" },
                new RoomType { ID = 2, Name = "2 twin beds" },
                new RoomType { ID = 3, Name = "1 single bed and 1 sofa bed" }
            );

            // Seed the Users table with initial data
            // Password for all users is: "Password1"
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    ID = 1,
                    Name = "John Doe",
                    Email = "john.doe@test.com",
                    Password = "9Btfac3pxiGLXZSJcBbw80+NmmDv8qnt1cHvL0VBanw=",
                    Salt = 12345,
                    RoleID = 1 // Admin
                },
                new User
                {
                    ID = 2,
                    Name = "Jane Doe",
                    Email = "jane.doe@test.com",
                    Password = "9Btfac3pxiGLXZSJcBbw80+NmmDv8qnt1cHvL0VBanw=",
                    Salt = 12345,
                    RoleID = 3 // Accommodation Manager
                },
                new User
                {
                    ID = 3,
                    Name = "Elisabeth Doe",
                    Email = "eli.doe@test.com",
                    Password = "9Btfac3pxiGLXZSJcBbw80+NmmDv8qnt1cHvL0VBanw=",
                    Salt = 12345,
                    RoleID = 3 // Accommodation Manager #2
                },
                new User
                {
                    ID = 4,
                    Name = "Kim Dotcom",
                    Email = "kimble@test.com",
                    Password = "9Btfac3pxiGLXZSJcBbw80+NmmDv8qnt1cHvL0VBanw=",
                    Salt = 12345,
                    RoleID = 2 // User
                },
                new User
                {
                    ID = 5,
                    Name = "John Reviewer",
                    Email = "john@test.com",
                    Password = "9Btfac3pxiGLXZSJcBbw80+NmmDv8qnt1cHvL0VBanw=",
                    Salt = 12345,
                    RoleID = 2 // User #2
                });
        }
    }
}
