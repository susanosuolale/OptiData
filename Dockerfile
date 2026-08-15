# Stage 1: Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Stage 2: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["OptiData.Presentation/OptiData.Presentation.csproj", "OptiData.Presentation/"]
COPY ["OptiData.Application/OptiData.Application.csproj", "OptiData.Application/"]
COPY ["OptiData.Domain/OptiData.Domain.csproj", "OptiData.Domain/"]
COPY ["OptiData.Infrastructure/OptiData.Infrastructure.csproj", "OptiData.Infrastructure/"]
RUN dotnet restore "OptiData.Presentation/OptiData.Presentation.csproj"
COPY . .
WORKDIR "/src/OptiData.Presentation"
RUN dotnet build "OptiData.Presentation.csproj" -c Release -o /app/build

# Stage 3: Publish the application
FROM build AS publish
RUN dotnet publish "OptiData.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final image for production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OptiData.Presentation.dll"]
