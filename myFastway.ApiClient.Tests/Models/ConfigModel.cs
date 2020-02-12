using Microsoft.Extensions.Configuration;

namespace myFastway.ApiClient.Tests.Models {
    public class ConfigModel {
        public OAuthConfig OAuth { get; }
        public ApiConfig Api { get; }

        public ConfigModel(IConfigurationRoot config) {
            OAuth = new OAuthConfig(config);
            Api = new ApiConfig(config);
        }
    }

    public class OAuthConfig {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string Scope { get; set; }
        public bool RequireHttps { get; set; }

        public OAuthConfig(IConfigurationRoot config) {
            Authority = config["oauth:authority"];
            ClientId = config["oauth:client_id"];
            Secret = config["oauth:secret"];
            Scope = config["oauth:scope"];
            RequireHttps = bool.Parse(config["oauth:requireHttps"]);
        }
    }

    public class ApiConfig {
        public string BaseAddress { get; set; }
        public string ApiVersion { get; set; }

        public ApiConfig(IConfigurationRoot config) {
            BaseAddress = config["api:base-address"];
            ApiVersion = config["api:version"];
        }
    }
}



