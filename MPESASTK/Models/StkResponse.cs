namespace STKPushAPI.Models
{
    public class StkResponse
    {
        public string ExternalRef { get; set; }
        public string MerchantID { get; set; }
        public string CheckoutRequestID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string CustomerMessage { get; set; }
    }
}
