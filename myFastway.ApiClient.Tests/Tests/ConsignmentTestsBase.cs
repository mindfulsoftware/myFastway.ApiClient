using myFastway.ApiClient.Tests.Models;
using System.Threading.Tasks;

namespace myFastway.ApiClient.Tests {
    public abstract class ConsignmentTestsBase : TestBase
    {
        protected const string BASE_ROUTE = "consignments";

        protected async Task<PersistedConsignmentModel> Consign(CreateConsignmentModel consignment = null) {
            consignment = consignment ?? GetStandardConsignment();
            return await PostSingle<PersistedConsignmentModel>(BASE_ROUTE, consignment);
        }

        protected CreateConsignmentModel GetStandardConsignment() {
            var result = new CreateConsignmentModel {
                ConTypeId = ConTypeId.Standard,
                To = new ContactModel {
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
                },
                Items = new[]
                {
                    new CreateConsignmentItemModel
                    {
                        Quantity = 1,
                        PackageType = "P",
                        Reference = "Parcel",
                        WeightDead = 5,
                        Length = 25,
                        Width = 10,
                        Height = 10
                    },
                    new CreateConsignmentItemModel
                    {
                        Quantity = 1,
                        PackageType = "S",
                        Reference = "Satchel",
                        SatchelSize = "A4"
                    }
                }
            };
            return result;
        }

        protected ContactModel GetContact() {
            return new ContactModel {
                ContactName = "Sarah Sender",
                BusinessName = "Sarahs' Stuff",
                Email = "sarah@sarahs-stuff.com.au",
                PhoneNumber = "0400 000 111",
                Address = new AddressModel {
                    StreetAddress = "333 Collins St",
                    Locality = "Melbourne",
                    PostalCode = "3000",
                    StateOrProvince = "VIC",
                    Country = "AU"
                }
            };
        }
    }
}
