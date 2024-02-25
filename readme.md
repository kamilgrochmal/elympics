# Elympics recruitment task

## Introduction
This project involves creation of REST APIs using ASP.NET and Golang, focusing on data retrieval, processing and storage.
The goal is to demonstrate integration, end-to-end testing, containerization with
Docker and deployment using Kubernetes with Helm.

## Table of Contents
- [Elympics recruitment task](#elympics-recruitment-task)
  - [Introduction](#introduction)
  - [Table of Contents](#table-of-contents)
  - [Task description](#task-description)
  - [My interpretation](#my-interpretation)
  - [Technologies Used](#technologies-used)
    - [.NET App](#net-app)
    - [Go app:](#go-app)
    - [E2E tests:](#e2e-tests)
  - [Features](#features)
  - [Prerequisites](#prerequisites)
  - [Installation and Setup](#installation-and-setup)
  - [Running the Project](#running-the-project)
  - [Integration Testing](#integration-testing)
  - [CI/CD Pipeline](#cicd-pipeline)
  - [Local Deployment](#local-deployment)
  - [End-to-End Testing](#end-to-end-testing)
  - [Configuration](#configuration)
  - [Contact](#contact)

## Task description
The task is to write a REST API in ASP.NET and another in Golang. API
ASP.NET has one endpoint whose job is to retrieve data from the API
Golang API, storing this data in a PostgreSQL database, and then returning a history of N
(e.g. 1-10, retrieved from config, possibly overwritten by an environment variable) of the most recent
entries (EF Core). The Golang API also has one endpoint that returns arbitrary
data (e.g. a random number).
You then need to create an integration test with the database and with the dockerised service
Golang service (e.g. using docker compose), in any technology (e.g. bash)
a sample CI/CD pipeline that would build and run this test.
Finally, create a simple deployment with instructions to run on the local k8s
(Helm) and an E2E test that will benefit from this deployment.

## My interpretation

Worth mentioning, gowebpi returns random podcast object, which consists of random 3 words as a `Title`, and random First Name and Surname as `HostedBy`.

*In `./docs/adr/.` is ADR which describes my choices.*
## Technologies Used

### .NET App
- ASP.NET Core (.NET 8.0)
- Golang (1.21.5)
- PostgreSQL (version 16.1)
- ORM Entity Framework Core (8.0.0)
- Polly
- Logger - Serilog 
- Docker
- Kubernetes (Helm v3)
- CI/CD (GitHub Actions)
- SwaggerUI
- XUnit
- WireMock
- TestContainers
- FluentAssertions

### Go app:
- Echo v4
- gofakeit v6

### E2E tests:
- Postman collections
- newman


## Features

- **gowebapi**: exposes two endpoints one `/health` which is simple health check and `/podcast` which create fake podcast with use of `gofakeit` library 
- **aspnetapi**: exposes one endpoint which makes call to **gowebapi**, gets the podcast, saves it to db and returns historic data amount that is set in `appsettings.json`. This value can be overrided in `./docker-compose.yaml` (for local use) as in `./charts/parent-app/values.yaml` for deployemnt configuration.

## Prerequisites

- Docker
- Kubernetes
- Helm
- Minikube
- Newman (for running E2E tests)

## Installation and Setup


| WARNING                                                                                                                                                            |
|:-------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| *Mind the paths, they are adjusted to Windows environment*. If you want to run it on macOS or linux, exchange **'\\'** with **'/'** in places where they are used. |



1. Install Docker https://docs.docker.com/engine/install/
2. Install Minikube https://minikube.sigs.k8s.io/docs/start/
3. Install newman https://blog.postman.com/installing-newman-on-windows/ (If you don't have Windows, then look up for other OS versions)
4. Make sure that you have access to `kubectl`
5. Open terminal and execute: `minikube start` then `kubectl get all`
6. If you don't have install with this instruction: https://kubernetes.io/docs/tasks/tools/#kubectl
7. Clone this repository: `git clone https://github.com/qhugra/elympics.git`

## Running the Project
1. Run Docker
2. Run `minikube start`
3. Open terminal and go to `YOUR:PC/elympics/charts/parent-app/.`
4. Exectute `helm upgrade --install myreleasev1 -f .\values.yaml .` (Keep attention to name - **myreleasev1** - *don't change it*)
5. Open new terminal and execute `minikube tunnel` (Now you shoule be able to access **aspnetapi** at `http:127.0.0.1:8080`)
Instructions on how to run both the ASP.NET and Golang APIs, including any necessary commands.

| WARNING                                                                                                                                                                                                         |
|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **aspnetapi** pod will propably need to restart twice (because it depends on postgresql). Check if all pods have status `Running` and are `Ready 1/1` - if not, then just wait a little more before proceeding. |

## Integration Testing

There is written one integration test aka E2E but in closed network. You can execute it by going to `elympics\aspnetapi\tests\ElympicsNet.Api.Tests.Integration` and executing in terminal `dotnet test`
or just run it with use of IDE UI.\
This test uses TestContainers for creating database and WireMock for creating mock for **gowebapi** call. \
After execution of this test, all resources are being disposed. 
Execution of Tests in .NET App are also part of CI/CD step.

## CI/CD Pipeline

The CI/CD pipeline for this project is designed to automate the process of building, testing and deploying application.
It is configured to trigger on any push or pull request to the main branch.

Relevant steps:
1. Setting up Docker
2. Login into Docker
3. Building and tagging docker images
4. Running services
5. Executing Integration tests (Only these exist)
6. Pushing images to DockerHub to allow k8s to use images with newest changes.

## Local Deployment

If you want to test all applications locally, you can do it with use of `docker compose`.

1. Move to main repo directory, execute in terminal `docker-compose up -d`
2. You can also execute E2E test by executing `newman run .\E2E\Elympics.postman_collection.json`.

## End-to-End Testing

1. Make sure you did everything from 'Running the Project' and your deployment is running. You can exectute in terminal this command `helm status myreleasev1` to check it out.
2. Open terminal, go to `YOUR:PC/elympics/.` and execute `newman run .\E2E\Elympics.postman_collection.json`

Take into account that this test checks if there are 3 items in response (2 seeded, one from **gowebapi**).\
x = amount of items to fetch from `/podcasts` (value from `appsettings.json` `Entry:MaxFetchedAmount`) \
If you execute `/podcast` endpoint x-2 times, before executing E2E test it will fail, because there will be newer items in database.
So this test should pass one time per `PVC` lifetime.

To be able to run this test successfully again:
1. You have to uninstall current release - `helm uninstall myreleasev1`
2. You have to delete bounded PVC - `kubectl get pvc` and `kubectl delete pvc [pvc-name-from-previous-command]`
3. Run `helm upgrade --install myreleasev1 -f .\values.yaml .`
4. Run `newman run .\E2E\Elympics.postman_collection.json`


## Configuration

Configuration for local deployment is placed in `elympics/docker-compose.yaml` - here you can override `Entry:MaxFetchedAmount` for amount of items that you want to be returned as well as other `ENV` variables.
Configuration for local k8s deployment is placed in `elympics/charts/parent-app/values.yaml`.

## Contact

linkedin: https://www.linkedin.com/in/kamil-grochmal/

---

