# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos los archivos del proyecto
COPY . ./

# Restauramos dependencias y compilamos
WORKDIR /app/Instituto.C
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Puerto expuesto por defecto en ASP.NET Core
EXPOSE 80

# Comando para ejecutar la app
ENTRYPOINT ["dotnet", "Instituto.C.dll"]
