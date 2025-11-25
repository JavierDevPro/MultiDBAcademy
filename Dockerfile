# === Build stage ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["MultiDBAcademy.Api/MultiDBAcademy.Api.csproj", "MultiDBAcademy.Api/"]
COPY ["MultiDBAcademy.Application/MultiDBAcademy.Application.csproj", "MultiDBAcademy.Application/"]
COPY ["MultiDBAcademy.Domain/MultiDBAcademy.Domain.csproj", "MultiDBAcademy.Domain/"]
COPY ["MultiDBAcademy.Infrastructure/MultiDBAcademy.Infrastructure.csproj", "MultiDBAcademy.Infrastructure/"]

RUN dotnet restore "MultiDBAcademy.Api/MultiDBAcademy.Api.csproj"

COPY . .

RUN dotnet build "MultiDBAcademy.Api/MultiDBAcademy.Api.csproj" -c Release -o /app/build

# === Publish ===
FROM build AS publish
WORKDIR /src
RUN dotnet publish "MultiDBAcademy.Api/MultiDBAcademy.Api.csproj" -c Release -o /app/publish

# === Final stage ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render asigna el puerto en la variable $PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT
EXPOSE 8080

ENTRYPOINT ["dotnet", "MultiDBAcademy.Api.dll"]
