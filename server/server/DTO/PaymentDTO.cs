namespace server.DTO
{
    public class PaymentDTO
    {
        public class PaymentInformationModel
        {
            public string PaymentId { get; set; }
            public string Amount { get; set; }
            public string OrderDescription { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public string Success { get; set; }
        }
        public class PaymentCallBack
        {
            public string VnPayResponseCode { get; set; }
            public string PaymentInfo { get; set; }
            public string PaymentId { get; set; }
            public DateTime PaymentDateTime { get; set; }
            public string TransactionCode { get; set; }
            public string TransactionNo { get; set; }
            public string Amount { get; set; }

        }
    }
}