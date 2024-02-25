FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ElympicsNet.Api/ElympicsNet.Api.csproj", "ElympicsNet.Api/"]
RUN dotnet restore "ElympicsNet.Api/ElympicsNet.Api.csproj"
COPY . .
WORKDIR "/src/ElympicsNet.Api"
RUN dotnet publish "ElympicsNet.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ElympicsNet.Api.dll"]

