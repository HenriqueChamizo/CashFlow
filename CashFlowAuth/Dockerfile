FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app/CashFlowAuth

COPY *.csproj ./
RUN dotnet restore

COPY . ./
COPY ../ ../
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /out .

EXPOSE 5135

ENTRYPOINT ["dotnet", "CashFlowAuth.dll"]
