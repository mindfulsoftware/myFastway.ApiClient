using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models
{
    public class ConsignmentModel
    {
        public ContactModel To { get; set; }
        public string InstructionsPublic { get; set; }
        public string InstructionsPrivate { get; set; }
        public string ExternalRef1 { get; set; }
        public string ExternalRef2 { get; set; }
        public IEnumerable<ConsignmentItemModel> Items { get; set; }
    }

    public class ConsignmentItemModel
    {
        public byte Quantity { get; set; }
        public string Reference { get; set; }
        public string PackageType { get; set; }
        public int? MyItemId { get; set; }
        public string MyItemCode { get; set; }
        public string SatchelSize { get; set; }
        public decimal? WeightDead { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
    }
}
