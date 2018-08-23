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
            var persistedConsignment = await Consign();
        }

        [Fact]
        public async Task CanGetById()
        {
            var persistedConsignment = await Consign();
            var loadedConsignment = await GetSingle<PersistedConsignmentModel>($"{BASE_ROUTE}/{persistedConsignment.ConId}");
            Assert.NotNull(loadedConsignment);
            Assert.Equal(persistedConsignment.ConId, loadedConsignment.ConId);
        }

        [Fact]
        public async Task CanConsignWithExistingContact()
        {
            var consignment = GetConsignment();
            var persistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.To);
            consignment.To = null;
            consignment.ToContactId = persistedContact.ContactId;
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task QuoteMatchesConsignment()
        {
            var consignment = GetConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.Equal(quote.Total, persistedConsignment.Total);
        }

        [Fact]
        public async Task GetByDateRangeReturnsMultiple()
        {
            for (var i = 0; i < 3; i++)
            {
                var persistedConsignment = await Consign();
            }
            var dateFormat = DateTime.Now.ToString("yyyy-MM-dd");
            var listItems = await GetCollection<ConsignmentListItem>($"{BASE_ROUTE}?fromDate={dateFormat}&toDate={dateFormat}&pageNumber=0&pageSize=10");
            Assert.True(listItems.Count() >= 3);
        }

        [Fact]
        public async Task GetLabelsForConsignment()
        {
            var persistedConsignment = await Consign();
            await WriteLabelsPDF(persistedConsignment.ConId, "A4");
            await WriteLabelsPDF(persistedConsignment.ConId, "4x6");
        }

        [Fact]
        public async Task GetLabelsForConsignmentLabels()
        {
            var persistedConsignment = await Consign();
            var label = persistedConsignment.Items.First().Label;
            await WriteLabelsPDF(persistedConsignment.ConId, "A4", label);
            await WriteLabelsPDF(persistedConsignment.ConId, "4x6", label);
        }

        [Fact]
        public async Task DeleteAndUndeleteCycle()
        {
            var persistedConsignment = await Consign();

            var deletedReasons = await GetCollection<DeletedReasonModel>($"{BASE_ROUTE}/deleted-reasons");
            Assert.NotEmpty(deletedReasons);

            var deleteResponse = await Delete($"{BASE_ROUTE}/{persistedConsignment.ConId}/reason/{deletedReasons.First().Id}");
            Assert.True(deleteResponse.IsSuccessStatusCode);

            var shouldBeNull = await GetSingle<PersistedConsignmentModel>($"{BASE_ROUTE}/{persistedConsignment.ConId}");
            Assert.Null(shouldBeNull);

            var undeleteResponse = await PutSingle($"{BASE_ROUTE}/{persistedConsignment.ConId}/undelete", null);
            Assert.True(undeleteResponse.IsSuccessStatusCode);

            var undeletedConsignment = await GetSingle<PersistedConsignmentModel>($"{BASE_ROUTE}/{persistedConsignment.ConId}");
            Assert.NotNull(undeletedConsignment);
            Assert.Equal(persistedConsignment.ConId, undeletedConsignment.ConId);
        }

        private async Task WriteLabelsPDF(int conId, string pageSize, string label = null)
        {
            var labelPart = string.IsNullOrWhiteSpace(label) ? string.Empty : $"/{label}";
            var labelsPdf = await GetBytes($"{BASE_ROUTE}/{conId}/labels{labelPart}?pageSize={pageSize}");
            Assert.NotNull(labelsPdf);
            Assert.NotEmpty(labelsPdf);
            var suffix = string.IsNullOrWhiteSpace(label) ? conId.ToString() : label;
            var path = $@"{Path.GetTempPath()}Labels_{pageSize}_{suffix}.pdf";
            await File.WriteAllBytesAsync(path, labelsPdf);
            Debug.WriteLine($"{pageSize} Labels written to {path}");
        }

        private async Task<PersistedConsignmentModel> Consign()
        {
            var consignment = GetConsignment();
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result.ConId > 0);
            return result;
        }

        private CreateConsignmentModel GetConsignment()
        {
            var result = new CreateConsignmentModel
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
                    new CreateConsignmentItemModel
                    {
                        Quantity = 1,
                        PackageType = "P",
                        Reference = "Parcel",
                        WeightDead = 5,
                        Length = 25,
                        Width = 10,
                        Height = 10
                    },
                    new CreateConsignmentItemModel
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
