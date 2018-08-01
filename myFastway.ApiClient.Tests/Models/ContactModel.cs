namespace myFastway.ApiClient.Tests.Models
{
    public class ContactModel {
        public int ContactId { get; set; }
        public string Code { get; set; }
        public string BusinessName { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Instructions { get; set; }
        public AddressModel Address { get; set; }
    }
}
