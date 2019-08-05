using myFastway.ApiClient.Tests.Models;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests {
    public class ReceiverPaysTests : ConsignmentTestsBase 
    {
        [Fact]
        public async Task CanQuoteReceiverPays() {
            var consignment = GetReceiverPaysConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            Assert.True(quote.Total > 0);
        }

        [Fact]
        public async Task CanConsignReceiverPays() {
            var consignment = GetReceiverPaysConsignment();
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result?.ConId > 0);
        }

        [Fact]
        public async Task CanConsignReceiverPaysWithExistingContact() {
            var consignment = GetReceiverPaysConsignment();
            var persistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.From);
            consignment.From = new ContactModel { ContactId = persistedContact.ContactId };
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        CreateConsignmentModel GetReceiverPaysConsignment() {
            var result = GetStandardConsignment();
            result.ConTypeId = ConTypeId.ReceiverPays;
            result.From = GetContact();
            result.To = null;

            return result;
        }
    }
}
