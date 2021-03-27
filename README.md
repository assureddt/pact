# Pact 📦

A collection of useful common services and helpers for .NET (Core 3.1 and beyond).

Encompassing: Primitive Extension Methods; Distributed Caching; Email & SMS Delivery; File System Impersonation; Localization; 
Structured Logging; Message Queue; Web Application Error Handling; and Tag Helpers.

Most packages involve the opinionated use of some preferred upstream dependencies, most of which should be listed below.  All code within is comprehensively unit tested and documented.

Package releases can be found on [NuGet.org](https://www.nuget.org/profiles/assureddt).

For packaging and deployment information, look [here](./DEPLOYMENT.md).

The API Wiki, containing automatically generated documentation, can be found [here](https://github.com/assureddt/pact/wiki).

## Status

![main](https://github.com/assureddt/pact/workflows/test/badge.svg)
[![CodeQL](https://github.com/assureddt/pact/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/assureddt/pact/actions/workflows/codeql-analysis.yml)
![main](https://github.com/assureddt/pact/workflows/publish%20packages/badge.svg)

## Package List

* [Pact.Cache](./src/Pact.Cache)
* [Pact.Core](./src/Pact.Core)
* [Pact.Email](./src/Pact.Email)
* [Pact.EntityFrameworkCore](./src/Pact.EntityFrameworkCore)
* [Pact.Impersonation](./src/Pact.Impersonation)
* [Pact.Kendo](./src/Pact.Kendo)
* [Pact.Localization](./src/Pact.Localization)
* [Pact.Logging](./src/Pact.Logging)
* [Pact.RabbitMQ](./src/Pact.RabbitMQ)
* [Pact.Sms](./src/Pact.Sms)
* [Pact.TagHelpers](./src/Pact.TagHelpers)
* [Pact.Web](./src/Pact.Web)
* [Pact.Web.Vue.Grid](./src/Pact.Web.Vue.Grid)
* [Pact.Web.ErrorHandling](./src/Pact.Web.ErrorHandling)

## Dependencies

Omitting framework dependencies for brevity _[Last updated: 2021-03-26]_

| Reference                                              | Version | Licence Type    | License                                                                              |
|--------------------------------------------------------|---------|-----------------|--------------------------------------------------------------------------------------|
| AutoMapper                                             | 10.1.1  | MIT             | https://licenses.nuget.org/MIT                                                       |
| coverlet.collector                                     | 3.0.3   | MIT             | https://licenses.nuget.org/MIT                                                       |
| IPNetwork2                                             | 2.5.292 |                 | https://github.com/lduchosal/ipnetwork/blob/master/LICENSE                           |
| MailKit                                                | 2.11.1  | MIT             | https://licenses.nuget.org/MIT                                                       |
| Moq                                                    | 4.16.1  |                 | https://raw.githubusercontent.com/moq/moq4/master/License.txt                        |
| Moq.AutoMock                                           | 2.3.0   | LICENSE         | https://aka.ms/deprecateLicenseUrl                                                   |
| NetEscapades.AspNetCore.SecurityHeaders                | 0.13.0  | MIT             | https://licenses.nuget.org/MIT                                                       |
| Newtonsoft.Json                                        | 13.0.1  | MIT             | https://licenses.nuget.org/MIT                                                       |
| RabbitMQ.Client                                        | 6.2.1   | LICENSE         | https://aka.ms/deprecateLicenseUrl                                                   |
| RichardSzalay.MockHttp                                 | 6.0.0   |                 | https://github.com/richardszalay/mockhttp/blob/master/LICENSE                        |
| Serilog.AspNetCore                                     | 4.0.0   | Apache-2.0      | https://licenses.nuget.org/Apache-2.0                                                |
| Shouldly                                               | 4.0.3   | BSD-2-Clause    | https://licenses.nuget.org/BSD-2-Clause                                              |
| Twilio.AspNet.Core                                     | 5.37.2  |                 | https://github.com/twilio/twilio-aspnet/blob/master/LICENSE                          |
| xunit                                                  | 2.4.1   |                 | https://raw.githubusercontent.com/xunit/xunit/master/license.txt                     |
| Xunit.Combinatorial                                    | 1.4.1   |                 | https://raw.githubusercontent.com/AArnott/Xunit.Combinatorial/c9b71e2aca/LICENSE.txt |
| xunit.runner.visualstudio                              | 2.4.3   | MIT             | https://licenses.nuget.org/MIT                                                       |
| Xunit.SkippableFact                                    | 1.4.13  | MS-PL           | https://licenses.nuget.org/MS-PL                                                     |
