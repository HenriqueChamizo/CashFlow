# Etapa de build usando o SDK do .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar os arquivos de projeto .csproj
COPY ./CashFlowInfra/CashFlowInfra.csproj ./CashFlowInfra/
COPY ./CashFlowReport/CashFlowReport.csproj ./CashFlowReport/

# Restaurar dependências
RUN dotnet restore ./CashFlowReport/CashFlowReport.csproj

# Copiar o código do projeto
COPY ./CashFlowInfra/ ./CashFlowInfra/
COPY ./CashFlowReport/ ./CashFlowReport/

# Publicar o projeto CashFlowReport
WORKDIR /app/CashFlowReport
RUN dotnet publish CashFlowReport.csproj -c Release -o /out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar os arquivos publicados da etapa de build
COPY --from=build /out .

# Expor a porta da aplicação
EXPOSE 5132

# Definir o ponto de entrada
ENTRYPOINT ["dotnet", "CashFlowReport.dll"]