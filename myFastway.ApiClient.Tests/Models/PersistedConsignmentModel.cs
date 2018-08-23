using System;
using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models
{
    public class PersistedConsignmentModel
    {
        public int ConId { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string InstructionsPublic { get; set; }
        public string InstructionsPrivate { get; set; }
        public string ExternalRef1 { get; set; }
        public string ExternalRef2 { get; set; }
        public ContactModel From { get; set; }
        public ContactModel To { get; set; }
        public IEnumerable<PersistedConsignmentItemModel> Items { get; set; }
        public IEnumerable<ConsignmentServicesModel> Services { get; set; }
    }

    public class PersistedConsignmentItemModel
    {
        public int ConItemId { get; set; }
        public string Label { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public byte Quantity { get; set; }
        public string Reference { get; set; }
        public string PackageType { get; set; }
        public int? MyItemId { get; set; }
        public string MyItemCode { get; set; }
        public string SatchelSize { get; set; }
        public decimal WeightDead { get; set; }
        public decimal WeightCubic { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public IEnumerable<ConsignmentItemServicesModel> Services { get; set; }
    }

    public class ConsignmentServicesModel
    {
        public short ServiceItemId { get; set; }
        public string ServiceDescription { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }

    public class ConsignmentItemServicesModel
    {
        public short ConItemId { get; set; }
        public short ServiceItemId { get; set; }
        public string ServiceDescription { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
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
