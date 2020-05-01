using System;
using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models.Internationals {
    public class ShipmentModel {
        public int ShipmentId { get; set; }
        public ContactModel From { get; set; }
        public ContactModel To { get; set; }
        public string Label { get; set; }
        public string HAWB { get; set; }
        public string DescriptionOfGoods { get; set; }
        public string PackageType { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Unit { get; set; }
        public byte NumberOfPieces { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal ActualWeight { get; set; }
        public bool HasAttachments { get; set; }
        public string CustomsValueCurrencyCode { get; set; }
        public decimal? CustomsValueAmount { get; set; }
        public string ExpectedCurrencyCode { get; set; }
        public decimal ExpectedTotal { get; set; }
        public string ActualCurrencyCode { get; set; }
        public decimal ActualTotal { get; set; }
        public int InvoiceNo { get; set; }
        public bool IsDutiable { get; set; }
        public Guid UriPart { get; set; }
        public IEnumerable<TrackingItemModel> TrackingItems { get; set; }
        public IEnumerable<string> TrackingErrors { get; set; }
        public IEnumerable<ShipmentFileListModel> ShipmentFiles { get; set; }
    }

    public class ContactModel {
        public int ContactId { get; set; }
        public string Code { get; set; }
        public string BusinessName { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string Email { get; set; }
        public virtual AddressModel Address { get; set; }
        public byte[] Version { get; set; }

    }

    public class AddressModel {
        public int AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public byte[] Hash { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }


    public class TrackingItemModel {
        public string Description { get; set; }
        public string Location { get; set; }
        public string Comments { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    public abstract class ShipmentFileBase {
        public int ShipmenId { get; set; }
        public byte ShipmentFileId { get; set; }
        public ShipmentFileType FileTypeId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }

    public class ShipmentFileListModel : ShipmentFileBase {
    }

    public class ShipmentFileModel : ShipmentFileBase {
        public byte[] Content { get; set; }
    }


    public enum ShipmentFileType : byte {
        Invoice = 1,
        Label = 2,
        Other = 3
    }



    public class ShipmentQuoteRequestModel {
        public ContactModel To { get; set; }
        public string ProductType { get; set; }
        public byte NumberOfPieces { get; set; }
        public decimal ActualWeight { get; set; }
        public string DescriptionOfGoods { get; set; }
        public decimal CustomsValueAmount { get; set; }
    }

    public class ShipmentQuoteResponseModel {
        public string CurrencyCode { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}
