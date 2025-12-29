namespace server.DTO
 {
     public class ServiceDTO
     {
        // public class Service
        // {
        //     public int ServiceId { get; set; }
        //     public string? ServiceName { get; set; }
        //     public string? Description { get; set; }
        //     public decimal? Price { get; set; }
        // }

        public class ServiceDetail
        {
            public int ServiceId { get; set; }
            public string? ServiceName { get; set; }
            public string? Description { get; set; }
            public float? Price { get; set; }
            public string? ServiceImage { get; set; }
            public string? ServiceIcon { get; set; }
         }
     }
 }