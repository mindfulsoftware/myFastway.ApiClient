using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models
{
    public class QuoteModel
    {
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public IEnumerable<QuoteLineItemModel> Items { get; set; }
    }

    public class QuoteLineItemModel
    {
        public string ProductType { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
