using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models
{
    public class ServiceModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ChargeLevel { get; set; }
        public IEnumerable<ServiceItemModel> Items { get; set; }
    }

    public class ServiceItemModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
