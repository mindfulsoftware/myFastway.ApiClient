using myFastway.ApiClient.Tests.Models.Internationals;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests.Tests.Internationals {
    public class AddressesTests : TestBase 
    {
        public const string BASE_ROUTE = "internationals/addresses";

        [Fact]
        public async Task CanReturnAllCountries()
        {
            var countries = await GetCollection<CountryModel>($"{BASE_ROUTE}/countries");

            Assert.NotNull(countries);
            Assert.NotEmpty(countries);
        }

        [Fact]
        public async Task CanReturnCountryByCode()
        {
            const string COUNTRY_CODE = "GB";

            var country = await GetSingle<CountryModel>($"{BASE_ROUTE}/countries/{COUNTRY_CODE}");

            Assert.NotNull(country);
            Assert.Equal(COUNTRY_CODE, country.Code);
            Assert.Equal("United Kingdom", country.Name);
        }

        [Fact]
        public async Task CanReturnCitiesForGivenCountry()
        {
            const string COUNTRY_CODE = "GB";
            const int PAGE_SIZE = 10;

            var cities = await GetCollection<string>($"{BASE_ROUTE}/countries/{COUNTRY_CODE}/cities?PageSize={PAGE_SIZE}&PageNumber=20");

            Assert.NotNull(cities);
            Assert.NotEmpty(cities);
            Assert.Equal(PAGE_SIZE, cities.Count());
        }

        [Fact]
        public async Task CanSearchCitiesForGivenCountry()
        {
            const string COUNTRY_CODE = "GB";
            const string STARTS_WITH = "Lond";

            var cities = await GetCollection<string>($"{BASE_ROUTE}/countries/{COUNTRY_CODE}/cities?startsWith={STARTS_WITH}");
            var city = cities.FirstOrDefault();

            Assert.NotNull(cities);
            Assert.Equal(STARTS_WITH, city.Substring(0, 4));
        }

        [Fact]
        public async Task CanReturnStatesForGivenCountry()
        {
            const string COUNTRY_CODE = "AU";

            var states = await GetCollection<StateModel>($"{BASE_ROUTE}/countries/{COUNTRY_CODE}/states");

            Assert.NotNull(states);
            Assert.NotEmpty(states);
        }

        [Fact]
        public async Task CanValidateAddress()
        {
            var model = new {
                AddressLine1 = "491 Kent St",
                AddressLine2 = "",
                AddressLine3 = "",
                City = "Sydney",
                StateOrProvince = "NSW",
                PostalCode = "2000",
                Country = "AU",
            };

            var response = await PostSingle($"{BASE_ROUTE}/validate", model);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UnknownAddressFailsValidation()
        {
            var model = new
            {
                AddressLine1 = "491 Kent St",
                AddressLine2 = "",
                AddressLine3 = "",
                City = "SydNEE", // <<== no such city.
                StateOrProvince = "NSW",
                PostalCode = "2000",
                Country = "AU",
            };

            var response = await PostSingle($"{BASE_ROUTE}/validate", model);
            var errors = await ParseErrors(response);

            Assert.NotEmpty(errors);
        }
    }
}
