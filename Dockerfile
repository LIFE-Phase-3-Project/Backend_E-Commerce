FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Life-Ecommerce/Service.csproj", "Life-Ecommerce/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Presistence/Presistence.csproj", "Presistence/"]
COPY ["BackgroundJobs/BackgroundJobs.csproj", "BackgroundJobs/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "Life-Ecommerce/Service.csproj"
COPY . .
WORKDIR "/src/Life-Ecommerce"
RUN dotnet build "Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Service.dll" ]