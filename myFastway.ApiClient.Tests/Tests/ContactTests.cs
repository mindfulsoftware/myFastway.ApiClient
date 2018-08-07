using myFastway.ApiClient.Tests.Models;
using Xunit;

namespace myFastway.ApiClient.Tests
{
    public class ContactTests : TestBase
    {
        public const string BASE_ROUTE = "contacts";


        [Fact]
        public async void CanGetContacts() {

            var addresses = await GetCollection<ContactModel>(BASE_ROUTE);

            Assert.NotNull(addresses);
            Assert.NotEmpty(addresses);
        }

        [Fact]
        public async void CanGetContactById() {

            const string ID = "7";

            var address = await GetSingle<ContactModel>($"{BASE_ROUTE}/{ID}");

            Assert.NotNull(address);
        }

    }
}
