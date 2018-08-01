using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests
{
    public class AuthenticationTests : TestBase
    {
        [Fact]
        public async Task CanGetClientCredentialToken1() {

            var token = await GetClientCredentialDiscovery();

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task CanGetClientCredentialToken2() {

            var token = await GetClientCredentialHttpClient();

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

    }
}
