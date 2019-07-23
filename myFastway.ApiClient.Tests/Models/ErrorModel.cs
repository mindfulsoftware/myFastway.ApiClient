using System.Collections.Generic;

namespace myFastway.ApiClient.Tests.Models {
    public class ErrorModel {
        public string Code { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> SuggestedValues  { get; set; }

        public override string ToString() {
            return $"ErrorModel (Code: {Code}, Message: {Message})";
        }
    }
}
