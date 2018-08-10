using myFastway.ApiClient.Tests.Models;
using System.Threading.Tasks;
using Xunit;

namespace myFastway.ApiClient.Tests
{
    public class ContactTests : TestBase
    {
        public const string BASE_ROUTE = "contacts";


        [Fact]
        public async Task CanGetContacts() {

            var addresses = await GetCollection<ContactModel>(BASE_ROUTE);

            Assert.NotNull(addresses);
            Assert.NotEmpty(addresses);
        }

        [Fact]
        public async Task CanGetContactById() {

            const string ID = "7";

            var address = await GetSingle<ContactModel>($"{BASE_ROUTE}/{ID}");

            Assert.NotNull(address);
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
                Address = new AddressModel
                {
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
