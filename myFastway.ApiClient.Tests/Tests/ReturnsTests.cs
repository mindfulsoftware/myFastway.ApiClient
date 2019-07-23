using myFastway.ApiClient.Tests.Models;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using System.Collections.Generic;

namespace myFastway.ApiClient.Tests {
    public class ReturnsTests : ConsignmentTestsBase
    {
        [Fact]
        public async Task CanConsignReturns() {
            var consignment = await GetReturnsConsignment();
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
        }

        [Fact]
        public async Task CanConsignReturnsWithOverrides() {
            var consignment = await GetReturnsConsignment();
            var item = consignment.Items.First();
            item.Length = 35;
            item.Width = 35;
            item.Height = 35;
            item.WeightDead = 11;
            var persistedConsignment = await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
            Assert.True(persistedConsignment.ConId > 0);
            var persistedItem = persistedConsignment.Items.First();
            Assert.True(persistedItem.Length == 35);
            Assert.True(persistedItem.Width == 35);
            Assert.True(persistedItem.Height == 35);
            Assert.True(persistedItem.WeightDead == 11);
        }

        private async Task<CreateConsignmentModel> GetReturnsConsignment() {
            var persistedConsignment = await Consign();

            Assert.True(persistedConsignment.ConId > 0);

            var result = new CreateConsignmentModel {
                ConTypeId = ConTypeId.Returns,
                Items = new List<CreateConsignmentItemModel>
                {
                    new CreateConsignmentItemModel { Label = persistedConsignment.Items.First().Label }
                }
            };

            return result;
        }
    }
}
