using myFastway.ApiClient.Tests.Models;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests
{
    public class ContactTests : TestBase
    {
        public const string BASE_ROUTE = "contacts";

        public ContactTests() {

        }


        [Fact]
        public async Task CanGetContacts() {

            var addresses = await GetCollection<ContactModel>(BASE_ROUTE);

            Assert.NotNull(addresses);
            Assert.NotEmpty(addresses);
        }

        [Fact]
        public async Task CanGetContactById() {

            var expected = await LoadModelFromFile<ContactModel>("contact");
            Assert.NotEqual(0, expected.ContactId);


            var actual = await GetSingle<ContactModel>($"{BASE_ROUTE}/{expected.ContactId}");

            Assert.NotNull(actual);

            Assert.Equal(expected.ContactId, actual.ContactId);
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.BusinessName, actual.BusinessName);
            Assert.Equal(expected.ContactName, actual.ContactName);
            Assert.Equal(expected.DisplayName, actual.DisplayName);
            Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Instructions, actual.Instructions);

            Assert.Equal(expected.Address.AddressId, actual.Address.AddressId);
            Assert.Equal(expected.Address.StreetAddress, actual.Address.StreetAddress);
            Assert.Equal(expected.Address.AdditionalDetails, actual.Address.AdditionalDetails);
            Assert.Equal(expected.Address.Locality, actual.Address.Locality);
            Assert.Equal(expected.Address.StateOrProvince, actual.Address.StateOrProvince);
            Assert.Equal(expected.Address.PostalCode, actual.Address.PostalCode);
            Assert.Equal(expected.Address.Country, actual.Address.Country);
            Assert.Equal(expected.Address.UserCreated, actual.Address.UserCreated);
            Assert.Equal(expected.Address.Hash, actual.Address.Hash);
            Assert.Equal(expected.Address.PlaceId, actual.Address.PlaceId);
            Assert.Equal(expected.Address.Lat, actual.Address.Lat);
            Assert.Equal(expected.Address.Lng, actual.Address.Lng);
        }

        [Fact]
        public async Task CanPost()
        {
            var contact = GetContactModel();
            var persistedContact = await PostSingle<ContactModel>(BASE_ROUTE, contact);
            Assert.True(persistedContact.ContactId > 0);
        }

        private ContactModel GetContactModel()
        {
            var result = new ContactModel
            {
                ContactName = "Tony Receiver",
                BusinessName = "Tony's Tools",
                Email = "tony@tonystools.com.au",
                PhoneNumber = "0400 123 456",
                Address = new AddressModel {
                    StreetAddress = "73 Katoomba St",
                    Locality = "Katoomba",
                    PostalCode = "2780",
                    StateOrProvince = "NSW",
                    Country = "AU"
                },
            };
            return result;
        }
    }
}
