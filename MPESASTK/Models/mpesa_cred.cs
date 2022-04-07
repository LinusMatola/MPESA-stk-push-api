namespace MPESASTK.Models
{
    public class mpesa_cred
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string Passkey { get; set; }
        public string shortCode { get; set; }
        public string STKPushEndPoint { get; set; }
        public string callback_endpoint { get; set; }
    }
}
