using myFastway.ApiClient.Tests.Models;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests.Tests
{
    public class ConsignmentTests : TestBase
    {
        const string BASE_ROUTE = "consignments";

        [Fact]
        public async Task CanQuote()
        {
            var consignment = GetConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            Assert.True(quote.Total > 0);
        }

        [Fact]
        public async Task CanConsign()
        {
            var consignment = GetConsignment();
            var response = await PostSingle<ConsignmentResponse>(BASE_ROUTE, consignment);
            Assert.True(response.ConsignmentId > 0);
        }

        [Fact]
        public async Task CanConsignWithExistingContact()
        {
            var consignment = GetConsignment();
            var persistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.To);
            Assert.True(persistedContact.ContactId > 0);
            consignment.To = null;
            consignment.ToContactId = persistedContact.ContactId;
            var response = await PostSingle<ConsignmentResponse>(BASE_ROUTE, consignment);
            Assert.True(response.ConsignmentId > 0);
        }

        [Fact]
        public async Task QuoteMatchesConsignment()
        {
            var consignment = GetConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            var consignmentResponse = await PostSingle<ConsignmentResponse>(BASE_ROUTE, consignment);
            var persistedConsignment = await GetSingle<ConsignmentModel>($"{BASE_ROUTE}/{consignmentResponse.ConsignmentId}");
            Assert.Equal(quote.Total, persistedConsignment.Total);
        }

        private ConsignmentModel GetConsignment()
        {
            var result = new ConsignmentModel
            {
                To = new ContactModel
                {
                    ContactName = "Tony Receiver",
                    BusinessName = "Tony's Tools",
                    Email = "tony@tonystools.com.au",
                    PhoneNumber = "0400 123 456",
                    Address = new AddressModel
                    {
                        StreetAddress = "73 Katoomba St",
                        Locality = "Katoomba",
                        PostalCode = "2780",
                        StateOrProvince = "NSW",
                        Country = "AU"
                    },
                },
                Items = new[]
                {
                    new ConsignmentItemModel
                    {
                        Quantity = 1,
                        PackageType = "P",
                        Reference = "Parcel",
                        WeightDead = 5,
                        Length = 25,
                        Width = 10,
                        Height = 10
                    },
                    new ConsignmentItemModel
                    {
                        Quantity = 1,
                        PackageType = "S",
                        Reference = "Satchel",
                        SatchelSize = "A4"
                    }
                }
            };
            return result;
        }
    }
}
