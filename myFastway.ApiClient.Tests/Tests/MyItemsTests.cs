using myFastway.ApiClient.Tests.Models;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests.Tests {
    public class MyItemsTests : TestBase {
        public const string BASE_ROUTE = "my-items";

        [Fact]
        public async Task CanGetItems() {

            var myItems = await GetCollection<MyItemsModel>(BASE_ROUTE);

            Assert.NotNull(myItems);
            Assert.NotEmpty(myItems);
        }
    }
}
