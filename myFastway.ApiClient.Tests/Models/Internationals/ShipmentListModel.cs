using System;

namespace myFastway.ApiClient.Tests.Models.Internationals
{
    public class ShipmentListModel {
        public int ShipmentId { get; set; }
        public string HAWB { get; set; }
        public string ToPerson { get; set; }
        public string ToCompanyName { get; set; }
        public string ToAddress { get; set; }
        public string PackageType { get; set; }
        public string DescriptionOfGoods { get; set; }
        public byte NumberOfPieces { get; set; }
        public DateTime CreatedOn { get; set; }
        public int InvoiceNo { get; set; }

    }
}
