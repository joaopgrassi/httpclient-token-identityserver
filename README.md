# Getting access tokens from IdentityServer via a HttpMessageHandler

This repo contains the source code for the 
[blog post](https://blog.joaograssi.com/typed-httpclient-with-messagehandler-getting-accesstokens-from-identityserver/) 
where I explained how to encapsulate the action of getting access tokens
from IdentityServer (or any token service) by using the `HttpClientFactory` and `HttpMessageHandler`.

## What's in this repo

The sample is composed of 3 ASP.NET services:

1. IdentityServer
2. Consumer API
3. Protected API

How they talk to each other is explained in the blog post. In a nutshell, the Consumer API
exposes endpoints that returns data obtained from the Protected API. The Consumer API
obtains access tokens from IdentityServer and uses it to retrieves "confidential" information
from the Protected API.

## Running the sample

- Run `docker-compose up` from the root to start all apps.
- Send a request to the `Consumer API` on: `http://localhost:5006/api/consumer/version3`
- Optionally, you can try `version1` and `version2`. 
  The results are the same, but the code is different. See blog post for more info.

### Requirements

- Docker/Compose
- .NET 6 SDK (if you want to build/debug locally)



