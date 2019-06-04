using myFastway.ApiClient.Tests.Models;
using System;
using System.Collections.Generic;
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
            var consignment = GetStandardConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            Assert.True(quote.Total > 0);
        }

        [Fact]
        public async Task CanQuoteReseller()
        {
            var consignment = GetResellerConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            Assert.True(quote?.Total > 0);
        }

        [Fact]
        public async Task CanQuoteReceiverPays()
        {
            var consignment = GetReceiverPaysConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            Assert.True(quote.Total > 0);
        }

        [Fact]
        public async Task CanConsign()
        {
            var persistedConsignment = await Consign();
        }

        [Fact]
        public async Task CanConsignReseller()
        {
            var consignment = GetResellerConsignment();
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result?.ConId > 0);
        }

        [Fact]
        public async Task CanConsignResellerWithExistingContacts()
        {
            var consignment = GetResellerConsignment();
            var fromPersistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.From);
            consignment.From = new ContactModel { ContactId = fromPersistedContact.ContactId };
            var toPersistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.To);
            consignment.To= new ContactModel { ContactId = toPersistedContact.ContactId };
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task CanConsignReceiverPays()
        {
            var consignment = GetReceiverPaysConsignment();
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result?.ConId > 0);
        }

        [Fact]
        public async Task CanConsignReceiverPaysWithExistingContact()
        {
            var consignment = GetReceiverPaysConsignment();
            var persistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.From);
            consignment.From = new ContactModel { ContactId = persistedContact.ContactId };
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task CanConsignWithServices()
        {
            var consignment = GetStandardConsignment();
            var services = await GetCollection<ServiceModel>("consignment-services");
            var hasAvailableServices = services.Any();
            if (hasAvailableServices)
            {
                var firstService = services.First();
                var firstItem = firstService.Items.First();
                consignment.Services = new List<CreateConsignmentServiceModel>
                {
                    new CreateConsignmentServiceModel
                    {
                        ServiceCode = firstService.Code,
                        ServiceItemCode = firstItem.Code
                    }
                };
            }
            await Consign(consignment);
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
            var consignment = GetStandardConsignment();
            var persistedContact = await PostSingle<ContactModel>(ContactTests.BASE_ROUTE, consignment.To);
            consignment.To = new ContactModel { ContactId = persistedContact.ContactId };
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task QuoteMatchesConsignment()
        {
            var consignment = GetStandardConsignment();
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

        [Fact]
        public async Task GetPending()
        {
            var persistedConsignment = await Consign();

            var pending = await GetCollection<ConsignmentSearchItem>($"{BASE_ROUTE}/pending?pageNumber=0&pageSize=10");
            Assert.NotEmpty(pending);
        }

        [Fact]
        public async Task CanConsignUsingMyItemId()
        {
            // save my item
            var newMyItem = new MyItemsModel
            {
                Code = "BB",
                Height = 10,
                Length = 20,
                Name = "Big Box",
                WeightDead = 5,
                Width = 30
            };
            var persistedMyItem = await PostSingle<MyItemsModel>("my-items", newMyItem);
            Assert.True(persistedMyItem.MyItemId > 0);

            // consign with my item
            var consignment = GetStandardConsignment();
            consignment.Items = new[]
            {
                new CreateConsignmentItemModel
                {
                    Quantity = 1,
                    MyItemId = persistedMyItem.MyItemId
                },
            };
            await Consign(consignment);
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

        private async Task<PersistedConsignmentModel> Consign(CreateConsignmentModel consignment = null)
        {
            consignment = consignment ?? GetStandardConsignment();
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result.ConId > 0);
            return result;
        }

        private CreateConsignmentModel GetReceiverPaysConsignment()
        {
            var result = GetResellerConsignment();
            result.ConType = ConType.ReceiverPays;
            result.To = null;
            return result;
        }

        private CreateConsignmentModel GetResellerConsignment()
        {
            var result = GetStandardConsignment();
            result.ConType = ConType.Reseller;
            result.From = new ContactModel
            {
                ContactName = "Sarah Sender",
                BusinessName = "Sarahs' Stuff",
                Email = "sarah@sarahs-stuff.com.au",
                PhoneNumber = "0400 000 111",
                Address = new AddressModel
                {
                    StreetAddress = "333 Collins St",
                    Locality = "Melbourne",
                    PostalCode = "3000",
                    StateOrProvince = "VIC",
                    Country = "AU"
                },
            };
            return result;
        }

        private CreateConsignmentModel GetStandardConsignment()
        {
            var result = new CreateConsignmentModel
            {
                ConType = ConType.Standard,
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
