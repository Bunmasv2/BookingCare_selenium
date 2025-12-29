namespace server.DTO
{
    public class SpecialtyDTO
    {
        public int SpecialtyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? SpecialtyImage { get; set; } // dạng base64
    }

}