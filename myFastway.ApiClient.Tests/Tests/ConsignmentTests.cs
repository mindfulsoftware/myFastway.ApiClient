using myFastway.ApiClient.Tests.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests
{
    public class ConsignmentTests : ConsignmentTestsBase 
    {
        [Fact]
        public async Task CanQuote()
        {
            var consignment = GetStandardConsignment();
            var quote = await PostSingle<QuoteModel>($"{BASE_ROUTE}/quote", consignment);
            Assert.True(quote.Total > 0);
        }

        [Fact]
        public async Task CanConsign()
        {
            var persistedConsignment = await Consign();

            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task CanConsignWithPickupDates()
        {
            var consignment = GetStandardConsignment();
            consignment.PickupTypeId = PickupType.Required;
            consignment.PickupDetails = new PickupDetails
            {
                PreferredPickupDate = DateTime.Today.AddDays(7),
                PreferredPickupCycleId = PickupCycle.AM
            };
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result.ConId > 0);
            Assert.NotNull(result.PickupDetails);
            Assert.Equal(consignment.PickupDetails.PreferredPickupDate, result.PickupDetails.PreferredPickupDate);
            Assert.Equal(consignment.PickupDetails.PreferredPickupCycleId, result.PickupDetails.PreferredPickupCycleId);
        }

        [Fact]
        public async Task CanConsignWithFuturePickupDates()
        {
            var consignment = GetStandardConsignment();
            consignment.PickupTypeId = PickupType.Future;
            consignment.FromInstructionsPublic = "Initial pickup instructions";
            var result = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(result.ConId > 0);
            Assert.Equal(consignment.FromInstructionsPublic, result.FromInstructionsPublic);

            var updateRequest = new UpdatePickupDetailsRequest
            {
                PreferredPickupDate = DateTime.Today.AddDays(7),
                PreferredPickupCycleId = PickupCycle.PM,
                FromInstructionsPublic = "Updated pickup instructions"
            };
            await PostSingle($"{BASE_ROUTE}/{result.ConId}/pickup-details", updateRequest);
            var loadedConsignment = await GetSingle<PersistedConsignmentModel>($"{BASE_ROUTE}/{result.ConId}");

            Assert.NotNull(loadedConsignment);
            Assert.NotNull(loadedConsignment.PickupDetails);
            Assert.Equal(updateRequest.PreferredPickupDate, loadedConsignment.PickupDetails.PreferredPickupDate);
            Assert.Equal(updateRequest.PreferredPickupCycleId, loadedConsignment.PickupDetails.PreferredPickupCycleId);
            Assert.Equal(updateRequest.FromInstructionsPublic, loadedConsignment.FromInstructionsPublic);
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

            var persistedConsignment = await Consign(consignment);

            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task CanGetById()
        {
            var persistedConsignment = await Consign();
            var loadedConsignment = await GetSingle<PersistedConsignmentModel>($"{BASE_ROUTE}/{persistedConsignment.ConId}");

            Assert.True(persistedConsignment.ConId > 0);
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

                Assert.True(persistedConsignment.ConId > 0);
            }
            var dateFormat = DateTime.Now.ToString("yyyy-MM-dd");
            var listItems = await GetCollection<ConsignmentListItem>($"{BASE_ROUTE}?fromDate={dateFormat}&toDate={dateFormat}&pageNumber=0&pageSize=10");

            Assert.True(listItems.Count() >= 3);
        }

        [Fact]
        public async Task GetLabelsForConsignment()
        {
            var persistedConsignment = await Consign();

            Assert.True(persistedConsignment.ConId > 0);

            await WriteLabelsPDF(persistedConsignment.ConId, "A4");
            await WriteLabelsPDF(persistedConsignment.ConId, "4x6");
        }

        [Fact]
        public async Task GetLabelsForConsignmentLabels()
        {
            var persistedConsignment = await Consign();

            Assert.True(persistedConsignment.ConId > 0);

            var label = persistedConsignment.Items.First().Label;
            await WriteLabelsPDF(persistedConsignment.ConId, "A4", label);
            await WriteLabelsPDF(persistedConsignment.ConId, "4x6", label);
        }

        [Fact]
        public async Task DeleteAndUndeleteCycle()
        {
            var persistedConsignment = await Consign();
            Assert.True(persistedConsignment.ConId > 0);

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

            Assert.True(persistedConsignment.ConId > 0);

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

            var persistedConsignment = await Consign(consignment);

            Assert.True(persistedConsignment.ConId > 0);
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

    }
}
