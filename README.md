# myFastway Api Client

A client for the myFastway API, demonstrating the best practices for authentication and data retrieval

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.  For endpoint detail, including request and response objects, please consult the [wiki](https://github.com/mindfulsoftware/myFastway.ApiClient/wiki)

### Prerequisites

This project is created using [ASP.NET Core 2.1](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.1) Unit test project

Once cloned, the appSettings.json document will have to be updated with your **client_id** and **secret** obtained from the website.  A token can be obtained from the Security Token server endpoints below

Country | Endpoint  | Verb | Paging | Description
:------:|:-------- |--------|:------:|:-----------
AU | https://identity.aramexconnect.com.au/connect/token | POST|| returns bearer token
NZ | https://identity.aramexconnect.co.nz/connect/token | POST|| returns bearer token


Finally the **base-address** of the api you're connecting to will have to be provided.  The current production api's are:

Resource | Country |Description| base-address
:-------|:-------| :-------| :-------
aramexConnect API |Australia | api base address | https://api.aramexconnect.com.au
aramexConnect API |New Zealand | api base address | https://api.aramexconnect.co.nz





```json
{
  "oauth": {
    "authority": "https://identity.fastway.org/connect/token",
    "client_id": "<YOUR CLIENT_ID>",
    "secret": "<YOUR SECRET>",
    "scope": ""
  },
  "api": {
    "version": "1.0",
    "base-address":  ""
  }
}
```


## Built With

* [ASP.NET Core](https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.302-windows-x64-installer) - Core Framework
* [XUnit](https://xunit.github.io/) - Testing Framework
* [Identity Server Token Validation](https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation) - OAuth2 token validation framework (optional)
