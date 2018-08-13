# myFastway Api Client

A client for the myFastway API, demonstrating the best practices for authentication and data retrieval

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

This project is created using [ASP.NET Core 2.1](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.1) Unit test project

Once cloned, the appSettings.json document will have to be updated with your **client_id** and **secret** obtained from the website.  

Finally the **base-address** of the api you're connecting to will have to be provided.  The current production api's are


Country | base-address
:-------| :-------
Australia | https://api.myfastway.com.au
New Zealand | https://api.myfastway.co.nz


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
