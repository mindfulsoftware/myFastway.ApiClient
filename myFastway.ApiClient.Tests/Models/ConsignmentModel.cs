using System;
using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models
{
    public class ConsignmentModel
    {
        public int ConId { get; set; }
        public int ToContactId { get; set; }
        public ContactModel To { get; set; }
        public string InstructionsPublic { get; set; }
        public string InstructionsPrivate { get; set; }
        public string ExternalRef1 { get; set; }
        public string ExternalRef2 { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
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
        public decimal WeightDead { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
    }

    public class ConsignmentListItem
    {
        public int ConId { get; set; }
        public string ToBusinessName { get; set; }
        public string ToContactName { get; set; }
        public string ToStreetAddress { get; set; }
        public string ToAdditionalDetails { get; set; }
        public string ToLocality { get; set; }
        public string ToStateOrProvince { get; set; }
        public string ToPostalCode { get; set; }
        public decimal Total { get; set; }
        public decimal TotalInvoiced { get; set; }
        public DateTime CreatedOn { get; set; }
        public byte ItemCount { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public string ExternalRef1 { get; set; }
        public string ExternalRef2 { get; set; }
    }
}
