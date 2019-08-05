using myFastway.ApiClient.Tests.Models;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests
{
    public class AddressTests : TestBase {
        public const string BASE_ROUTE = "addresses";

        [Fact]
        public async Task CanGetServicedBy() {

            var request = new ServicedByRequestModel {
                FromRF = "SYD",
                Locality = "Sydney",
                PostalCode = "2000",
                Lat = -33.8641578m,
                Lng = 151.2098659m
            };

            var expected = await LoadModelFromFile<ServicedByResponseModel>("servicedByResponse");
            var actual = await PostSingle<ServicedByResponseModel>($"{BASE_ROUTE}/serviced-by", request);

            Assert.NotNull(actual);
            Assert.Equal(expected.RF, actual.RF);
            Assert.Equal(expected.CF, actual.CF);
            Assert.Equal(expected.Zone, actual.Zone);
            Assert.Equal(expected.SubDepot, actual.SubDepot);

        }

        [Fact]
        public async Task AddressValidation()
        {
            var address = new AddressModel {
                StreetAddress = "73 Katoomba Street",
                Locality = "Katoomba",
                StateOrProvince = "NSW",
                PostalCode = "2780",
                Country = "AU"
            };

            var response = await PostSingle($"{BASE_ROUTE}/validate", address);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
