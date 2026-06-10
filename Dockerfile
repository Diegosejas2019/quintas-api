FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/QuintasApp.API/QuintasApp.API.csproj", "src/QuintasApp.API/"]
COPY ["src/QuintasApp.Application/QuintasApp.Application.csproj", "src/QuintasApp.Application/"]
COPY ["src/QuintasApp.Domain/QuintasApp.Domain.csproj", "src/QuintasApp.Domain/"]
COPY ["src/QuintasApp.Infrastructure/QuintasApp.Infrastructure.csproj", "src/QuintasApp.Infrastructure/"]

RUN dotnet restore "src/QuintasApp.API/QuintasApp.API.csproj"

COPY . .

RUN dotnet publish "src/QuintasApp.API/QuintasApp.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QuintasApp.API.dll"]
