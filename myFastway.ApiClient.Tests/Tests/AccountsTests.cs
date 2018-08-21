using myFastway.ApiClient.Tests.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests.Tests {
    public class AccountsTests : TestBase {
        public const string BASE_ROUTE = "accounts";

        [Fact]
        public async Task CanGetAvailableBalance() {

            var accounts = await GetSingle<AccountBalanceAvailableModel>($"{BASE_ROUTE}/available/balance");

            Assert.NotNull(accounts);
        }

        [Fact]
        public async Task CanGetAvailableTransactions() {

            var accounts = await GetCollection<AccountModel>($"{BASE_ROUTE}/available");

            Assert.NotNull(accounts);
            Assert.NotEmpty(accounts);
        }

        [Fact]
        public async Task CanGetPendingBalance() {

            var accounts = await GetSingle<AccountBalancePendingModel>($"{BASE_ROUTE}/pending/balance");

            Assert.NotNull(accounts);
        }


        [Fact]
        public async Task CanGetPendingTransactions() {

            var accounts = await GetCollection<AccountModel>($"{BASE_ROUTE}/pending");

            Assert.NotNull(accounts);
            Assert.NotEmpty(accounts);
        }

    }
}
