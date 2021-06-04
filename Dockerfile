FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY . /app
WORKDIR /app/src/PaymentGateway.API
RUN dotnet restore /app/PaymentGateway.sln
RUN dotnet publish --configuration release --output publish_location

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
RUN dotnet --info
WORKDIR /app
COPY --from=build /app/src/PaymentGateway.API/publish_location .
ENTRYPOINT ["dotnet", "PaymentGateway.API.dll"]
