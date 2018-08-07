
namespace myFastway.ApiClient.Tests.Models
{
    public class ServicedByRequestModel {
        public string FromRF { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }

    }

    public class ServicedByResponseModel {
        public bool Deeded { get; set; }
        public string RF { get; set; }
        public string CF { get; set; }
        public string Zone { get; set; }
        public string SubDepot { get; set; }

    }
}
