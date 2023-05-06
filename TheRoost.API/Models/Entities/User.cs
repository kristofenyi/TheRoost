namespace TheRoost.API.Models.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Salt { get; set; }

        //FK
        public int RoleID { get; set; }
        public Role Role { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Reservation> Reservations { get; set; }

        public User()
        {
        }

        public User(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
