using System;
using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models
{
    public class CreateConsignmentModel
    {
        public int ConId { get; set; }
        public ContactModel From { get; set; }
        public ContactModel To { get; set; }
        public string InstructionsPublic { get; set; }
        public string InstructionsPrivate { get; set; }
        public string ExternalRef1 { get; set; }
        public string ExternalRef2 { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public PickupType? PickupTypeId { get; set; }
        public PickupDetails PickupDetails { get; set; }
        public IEnumerable<CreateConsignmentItemModel> Items { get; set; }
        public IEnumerable<CreateConsignmentServiceModel> Services { get; set; }
        public ConTypeId ConTypeId { get; set; }
    }

    public class CreateConsignmentItemModel
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
        public string Label { get; set; }
    }

    public class CreateConsignmentServiceModel
    {
        public string ServiceCode { get; set; }
        public string ServiceItemCode { get; set; }
    }

    public enum ConTypeId : byte
    {
        Standard = 1,
        ReceiverPays = 2,
        ThirdParty = 3,
        Returns = 4,
        Reseller = 5
    }
}
