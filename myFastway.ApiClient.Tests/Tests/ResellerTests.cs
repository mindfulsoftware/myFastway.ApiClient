using myFastway.ApiClient.Tests.Models;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests {
    public class ResellerTests : ConsignmentTestsBase
    {
        [Fact]
        public async Task CanQuoteReseller() {
            var consignment = GetResellerConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            Assert.True(quote?.Total > 0);
        }

        [Fact]
        public async Task CanConsignReseller() {
            var consignment = GetResellerConsignment();
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result?.ConId > 0);
        }

        [Fact]
        public async Task CanConsignResellerWithExistingContacts() {
            var consignment = GetResellerConsignment();
            var fromPersistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.From);
            consignment.From = new ContactModel { ContactId = fromPersistedContact.ContactId };
            var toPersistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.To);
            consignment.To = new ContactModel { ContactId = toPersistedContact.ContactId };
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        CreateConsignmentModel GetResellerConsignment() {
            var result = GetStandardConsignment();
            result.ConTypeId = ConTypeId.Reseller;
            result.From = GetContact();
            
            return result;
        }

    }
}
