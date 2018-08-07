using myFastway.ApiClient.Tests.Models;
using Xunit;

namespace myFastway.ApiClient.Tests.Tests
{
    public class AddressTests : TestBase {
        public const string BASE_ROUTE = "addresses";

        [Fact]
        public async void CanGetServicedBy() {

            var request = new ServicedByRequestModel {
                FromRF = "SYD",
                Locality = "Sydney",
                PostalCode = "2000",
                Lat = -33.8641578m,
                Lng = 151.2098659m
            };

            var servicedBy = await PostSingle<ServicedByResponseModel>($"{BASE_ROUTE}/serviced-by", request);

            Assert.NotNull(servicedBy);
            Assert.Equal("SYD", servicedBy.RF);
            Assert.Equal("201", servicedBy.CF);
            Assert.Equal("SYD", servicedBy.Zone);
            Assert.Equal(string.Empty, servicedBy.SubDepot);

        }
    }
}
