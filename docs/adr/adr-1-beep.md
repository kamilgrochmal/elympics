# ADR 1: Approaches to Applications and technologies

Date: [20-12-2023]

## Status

Accepted

## Context

Create simple REST applications just as proof of concept.

## Decision

Create .NET WEB API with use of newest .NET 8.0 LTS version,
without any sophisticated architecture like Clean/Onion/Hex, MM. \
Application exposes two endpoints written with use of MinimalApi, one is `/health` as Health Check
, second is `/podcasts` for resolving one of main tasks from recruitment task. \
Regarding to REST maturity in .NET API is level 2 reached, without going deeper into HATEOAS - It would be a problem with just a single endpoint :) \
I used IHttpFactory with naive retry policy with use of Polly. \
Introduceed custom exceptions with simple ExceptionMiddleware \
CORS is allowed for every origin, because everything is encapsulated in cluster. \
I also used some level on encapsulation to shoud that I know that something like this exist.  I have often encountered developers who have forgotten about them.
In one DLL it could look silly in some places.

For testing I used XUnit, TestContainers, WireMock, FluentAssertions - kind of good industry standard.
This Integration Test could be consider as E2E test, because it tests much.
So, so I added E2E as the task requirement implied. I choose Postman, because it is closest to me.

Go application is written with Echo.
Echo just seemed nicer to me than Gin but was also placed as second in terms of performance.
Used gofakeit - I was looking for framework as Bogus in .NET and I found this one.

In Chart structure I choose solution with one parent chart and sub charts, it looked the most simple and pragmatic.
I didn't combine with connectionString creation from username, password, database retreived from some secrets.yaml, as with retrieving services names with {{ .Release.Name }}. Instead of I exposed connectionstring and gowebapiUrl via configMap.yaml.
Usually, I have encountered an approach in which connectionString is fetched from azure keyvault and before it gets into the keyvault, scripts are run to generate it.

## Consequences

As for recruitment task, there are no consequences.
Just in case more serious deployment, among others there should be smarter secret managment introduced.
