using myFastway.ApiClient.Tests.Models.Internationals;
using System.Threading.Tasks;
using Xunit;


namespace myFastway.ApiClient.Tests.Tests.Internationals 
{
    public class ShipmentsTests : TestBase
    {
        public const string BASE_ROUTE = "internationals/shipments";

        [Fact]
        public async Task CanReturnAllShipments()
        {
            var shipments = await GetCollection<ShipmentListModel>(BASE_ROUTE);

            Assert.NotNull(shipments);
            Assert.NotEmpty(shipments);
        }

        [Fact]
        public async Task CanReturnShipmentsForGivenDateRange()
        {
            const string FROM = "20200420";
            const string TO   = "20200425";

            var shipments = await GetCollection<ShipmentListModel>($"{BASE_ROUTE}/from={FROM}&to={TO}");

            Assert.NotNull(shipments);
            Assert.NotEmpty(shipments);
        }

        [Fact]
        public async Task CanReturnShipmentById()
        {
            const int ID = 601;

            var shipment = await GetSingle<ShipmentModel>($"{BASE_ROUTE}/{ID}");

            Assert.NotNull(shipment);
        }

        [Fact]
        public async Task CanReturnAllFilesForGivenShippent()
        {
            const int ID = 601;

            var files = await GetCollection<ShipmentFileListModel>($"{BASE_ROUTE}/{ID}/Files");

            Assert.NotNull(files);
            Assert.NotEmpty(files);
        }

        [Fact]
        public async Task CanDownloadFileForGivenShipment()
        {
            const int ID = 601;
            const byte FILE_ID = 1;

            var file = await GetSingle<ShipmentFileModel>($"{BASE_ROUTE}/{ID}/Files/{FILE_ID}");

            Assert.NotNull(file);

        }

        [Fact]
        public async Task CanGetQuoteForShipment()
        {
            var request = new ShipmentQuoteRequestModel
            {
                To = new ContactModel
                {
                    BusinessName = "test",
                    ContactName = "test",
                    PhoneNumber = "9999 9999",
                    Email = "test@test",
                    Address = new AddressModel
                    {
                        AddressLine1 = "1 test st",
                        City = "Rotorua",
                        PostalCode = "3201",
                        Country = "NZ"
                    }
                },
                ProductType = "PPX",
                NumberOfPieces = 1,
                ActualWeight = 1,
                DescriptionOfGoods = "test",
                CustomsValueAmount = 10
            };


            var quote = await PutSingle<ShipmentQuoteResponseModel>($"{BASE_ROUTE}/quote", request);

            Assert.NotNull(quote);
            Assert.True(quote.Total > 0);
        }

        [Fact]
        public async Task CanCreateShipment()
        {

        }

        [Fact]
        public async Task CanDeleteShipment()
        {

        }
    }
}
