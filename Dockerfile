# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TaskManagement.sln ./
COPY src/TaskManagement.API/TaskManagement.API.csproj ./src/TaskManagement.API/
COPY src/TaskManagement.Application/TaskManagement.Application.csproj ./src/TaskManagement.Application/
COPY src/TaskManagement.Domain/TaskManagement.Domain.csproj ./src/TaskManagement.Domain/
COPY src/TaskManagement.Infrastructure/TaskManagement.Infrastructure.csproj ./src/TaskManagement.Infrastructure/
COPY tests/TaskManagement.Tests/TaskManagement.Tests.csproj ./tests/TaskManagement.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish
WORKDIR /src/src/TaskManagement.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "TaskManagement.API.dll"]