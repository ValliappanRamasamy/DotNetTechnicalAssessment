namespace WebAPI.Models
{
    public class UserRegisterRequest
    {
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        //Optional
        public string? language { get; set; } = "en_US"; // Default value: "en_US" if not provided
    }
}

