namespace server.Models
{
    public class MomoCreatePaymentResponseModel
    {
        public string PayUrl { get; set; }
        public string Deeplink { get; set; }
        public string QrCodeUrl { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
    }

}