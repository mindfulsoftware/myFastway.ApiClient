namespace myFastway.ApiClient.Tests.Models.Internationals 
{
    public class CountryModel {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class StateModel : CountryModel {
    }
}
