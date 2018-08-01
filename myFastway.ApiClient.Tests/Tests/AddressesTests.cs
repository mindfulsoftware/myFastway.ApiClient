using myFastway.ApiClient.Tests.Models;
using Xunit;

namespace myFastway.ApiClient.Tests
{
    public class AddressesTests : TestBase
    {
        public const string BASE_ROUTE = "current-addresses";


        [Fact]
        public async void CanGetAddresses() {

            var addresses = await GetCollection<ContactModel>(BASE_ROUTE);

            Assert.NotNull(addresses);
            Assert.NotEmpty(addresses);
        }

        [Fact]
        public async void CanGetAddressById() {

            const string ID = "7";

            var address = await GetSingle<ContactModel>($"{BASE_ROUTE}/{ID}");

            Assert.NotNull(address);
        }

    }
}
