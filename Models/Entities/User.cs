namespace RegAuth.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsBlocked {  get; set; }=false;
        public DateTime RegistrationTime { get; set; }=DateTime.Now;
        public DateTime LastLogin { get; set; }

    }
}