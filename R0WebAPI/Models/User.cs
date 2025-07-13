namespace WebAPI.Models
{
    // Example user entity
    public class User : ResponseStatus
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public string role { get; set; }

        public string token { get; set; }
        public string refreshToken { get; set; }

        //Optional
        public bool? emailIsConfirmed { get; set; }
        public User? data { get; set; }         
        public SenseBox[]? boxes { get; set; }            
        public string? language { get; set; } = "en_US"; // Default value: "en_US" if not provided
    }
}

