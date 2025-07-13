namespace WebAPI.Models
{
    // Example SenseBox entity
    public class SenseBox //: ResponseStatus
    {
        public string name { get; set; }

        public string exposure { get; set; }

        public string model { get; set; }

        public string email { get; set; }

        //Optional
        public string? _id { get; set; }
    }
}

