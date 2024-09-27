# Etapa de build - usar SDK do .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Configurar o PATH global para a ferramenta dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiar os arquivos de projeto .csproj
COPY ../CashFlowInfra/CashFlowInfra.csproj ./CashFlowInfra/
COPY ../CashFlowControl/CashFlowControl.csproj ./CashFlowControl/

# Restaurar dependências
RUN dotnet restore ./CashFlowControl/CashFlowControl.csproj

# Copiar todo o código do projeto
COPY ../CashFlowInfra/ ./CashFlowInfra/
COPY ../CashFlowControl/ ./CashFlowControl/

# Executar migrações do Entity Framework (enquanto o SDK está disponível)
WORKDIR /app/CashFlowControl

# Publicar o projeto CashFlowControl
RUN dotnet publish CashFlowControl.csproj -c Release -o /out

# Etapa de runtime - usar a imagem mais leve do ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar o resultado da publicação
COPY --from=build /out .

# Expor a porta da aplicação
EXPOSE 5132

# Definir o ponto de entrada
ENTRYPOINT ["dotnet", "CashFlowControl.dll"]