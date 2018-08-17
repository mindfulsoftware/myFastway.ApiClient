using myFastway.ApiClient.Tests.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            var persistedConsignment = await PostSingle<ConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task CanConsignWithExistingContact()
        {
            var consignment = GetConsignment();
            var persistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.To);
            Assert.True(persistedContact.ContactId > 0);
            consignment.To = null;
            consignment.ToContactId = persistedContact.ContactId;
            var persistedConsignment = await PostSingle<ConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task QuoteMatchesConsignment()
        {
            var consignment = GetConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            var persistedConsignment = await PostSingle<ConsignmentModel>(BASE_ROUTE, consignment);
            Assert.Equal(quote.Total, persistedConsignment.Total);
        }

        [Fact]
        public async Task GetByDateRangeReturnsMultiple()
        {
            var consignment = GetConsignment();
            for (var i = 0; i < 3; i++)
            {
                var persistedConsignment = await PostSingle<ConsignmentModel>(BASE_ROUTE, consignment);
                Assert.True(persistedConsignment.ConId > 0);
            }
            var dateFormat = DateTime.Now.ToString("yyyy-MM-dd");
            var listItems = await GetCollection<ConsignmentListItem>($"{BASE_ROUTE}?fromDate={dateFormat}&toDate={dateFormat}&pageNumber=0&pageSize=10");
            Assert.True(listItems.Count() >= 3);
        }

        [Fact]
        public async Task GetLabelsForConsignment()
        {
            var consignment = GetConsignment();
            var persistedConsignment = await PostSingle<ConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
            await WriteLabelsPDF(persistedConsignment.ConId, "A4");
            await WriteLabelsPDF(persistedConsignment.ConId, "4x6");
        }

        private async Task WriteLabelsPDF(int conId, string pageSize)
        {
            var labelsPdf = await GetBytes($"{BASE_ROUTE}/{conId}/labels?pageSize={pageSize}");
            Assert.NotNull(labelsPdf);
            Assert.NotEmpty(labelsPdf);
            var path = $@"{Path.GetTempPath()}Labels_{pageSize}_{conId}.pdf";
            await File.WriteAllBytesAsync(path, labelsPdf);
            Debug.WriteLine($"{pageSize} Labels written to {path}");
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
