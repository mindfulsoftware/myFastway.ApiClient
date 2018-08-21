using System;

namespace myFastway.ApiClient.Tests.Models
{
    public class AccountBalanceAvailableModel {
        public decimal Available { get; set; }
    }

    public class AccountBalancePendingModel {
        public decimal Pending { get; set; }
    }

    public class AccountModel
    {
        public long AccountId { get; set; }
        public int ConId { get; set; }
        public string Label { get; set; }
        public DateTime TransDate { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal Tax { get; set; }
        public decimal Balance { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
    }
}
