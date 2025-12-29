namespace server.DTO
{
    public class MedicineDTO
    {
        public class MedicineBasic
        {
            public int? MedicineId { get; set; }

            public string? MedicalName { get; set; }
            public string? Unit { get; set; }

        }
        
        //public class MedicineDetail
        //{
        //    public int? MedicineId { get; set; }

        //    public string? MedicalName { get; set; }

        //    public int? StockQuantity { get; set; }

        //    public DateTime? ExpiredDate { get; set; }

        //    public string? Unit { get; set; }
        //}
    }
}