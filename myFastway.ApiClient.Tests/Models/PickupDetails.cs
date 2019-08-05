using System;

namespace myFastway.ApiClient.Tests.Models
{
    public class PickupDetails
    {
        public DateTime PreferredPickupDate { get; set; }
        public PickupCycle PreferredPickupCycleId { get; set; }
    }

    public class UpdatePickupDetailsRequest
    {
        public DateTime PreferredPickupDate { get; set; }
        public PickupCycle PreferredPickupCycleId { get; set; }
        public string FromInstructionsPublic { get; set; }
    }

    public enum PickupType
    {
        Required = 1,
        Future = 2,
        NotRequired = 3
    }

    public enum PickupCycle
    {
        AM = 1,
        PM = 2,
        NextCycle = 3
    }
}