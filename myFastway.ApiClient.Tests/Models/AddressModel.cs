namespace myFastway.ApiClient.Tests.Models
{
    public class AddressModel
    {
        public int AddressId { get; set; }
        public string StreetAddress { get; set; }
        public string AdditionalDetails { get; set; }
        public string Locality { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
    }
}
