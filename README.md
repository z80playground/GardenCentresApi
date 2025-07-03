# Garden Centres API

## Overview
The Garden Centres API is a RESTful web service built with ASP.NET Core for managing garden centres and their locations. It supports sharding by region (`UK` or `US`) based on user authentication via JWT, ensuring data isolation per region. The API uses a repository pattern for data access, DTOs (`GardenCentreDto`, `LocationDto`) to handle serialization and decouple database models, and is designed for extensibility with in-memory caching in the service layer. It follows API versioning (`/api/v1/`) and includes unit tests for reliability.

## Features
- **Entities**:
  - `GardenCentre`: Represents a garden centre with properties `Id`, `Name`, `LocationId`, `Region`, and a navigation property to `Location`.
  - `Location`: Represents a location with `Id`, `Name`, `Region`, and a collection of `GardenCentres` (one-to-many relationship).
  - `UserProfile`: Stores user-region mappings for sharding.
- **Sharding**: Data is partitioned by `Region` (`UK` or `US`), enforced via JWT claims and repository logic.
- **API Versioning**: All endpoints use `/api/v1/` with `Microsoft.AspNetCore.Mvc.Versioning`.
- **Authentication**: JWT-based authentication with a `Region` claim required for all requests.
- **DTOs**: Uses `GardenCentreDto` and `LocationDto` to prevent serialization cycles and decouple database models.
- **Repositories**: `IGardenCentreRepository`, `ILocationRepository`, `IUserProfileRepository` handle data access.
- **Service Layer**: Planned `IGardenCentreService` with `IMemoryCache` for caching `GardenCentreDto` and `LocationDto`.
- **Testing**: Unit tests for controllers using Moq to mock repositories.

## Project Structure
```
GardenCentresApi/
├── Controllers/
│   ├── GardenCentresController.cs
│   ├── LocationsController.cs
│   └── AuthController.cs
├── Data/
│   └── GardenCentreContext.cs
├── Models/
│   ├── GardenCentre.cs
│   ├── Location.cs
│   ├── UserProfile.cs
│   ├── GardenCentreDto.cs
│   └── LocationDto.cs
├── Repositories/
│   ├── IGardenCentreRepository.cs
│   ├── GardenCentreRepository.cs
│   ├── ILocationRepository.cs
│   ├── LocationRepository.cs
│   ├── IUserProfileRepository.cs
│   └── UserProfileRepository.cs
├── Services/
│   ├── IGardenCentreService.cs
│   └── GardenCentreService.cs
├── Tests/
│   ├── GardenCentresControllerTests.cs
│   └── LocationsControllerTests.cs
├── Program.cs
├── appsettings.json
└── README.md
```

## Prerequisites
- **.NET 8.0 SDK** or later
- **SQL Server** (local or remote)
- **Postman** or a similar API client for testing
- **Visual Studio** or **VS Code** (optional, for development)
- **Entity Framework Core CLI** (`dotnet ef`)

## Setup Instructions
1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd GardenCentresApi
   ```

2. **Configure Connection String**:
   Update `appsettings.json` with your SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=GardenCentresDb;Trusted_Connection=True;MultipleActiveResultSets=true"
     },
     "Jwt": {
       "Key": "your-secure-jwt-key-here",
       "Issuer": "your-issuer",
       "Audience": "your-audience"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.EntityFrameworkCore": "Debug",
         "GardenCentresApi": "Debug"
       }
     }
   }
   ```

3. **Install Dependencies**:
   Ensure the following NuGet packages are installed (via `csproj` or `dotnet add package`):
   ```xml
   <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.*" />
   <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.*" />
   <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.*" />
   <PackageReference Include="Microsoft.AspNetCore.Mvc.Versioning" Version="5.1.0" />
   <PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.0.*" />
   ```
   Run:
   ```bash
   dotnet restore
   ```

4. **Apply Database Migrations**:
   ```bash
   dotnet ef database update --project GardenCentresApi
   ```

5. **Seed Initial Data** (optional):
   Run the following SQL to create test data:
   ```sql
   INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash)
   VALUES ('1', 'john', 'JOHN', 'john@example.com', 'JOHN@EXAMPLE.COM', 'AQAAAAEAACcQAAAA...'); -- Use Identity password hasher
   INSERT INTO UserProfiles (UserId, Region) VALUES ('1', 'UK');
   INSERT INTO Locations (Name, Region) VALUES ('London', 'UK'), ('New York', 'US');
   INSERT INTO GardenCentres (Name, LocationId, Region) VALUES ('UK Garden 1', 1, 'UK');
   ```

6. **Run the Application**:
   ```bash
   dotnet run --project GardenCentresApi
   ```
   The API will be available at `https://localhost:5001` (or configured port).

7. **Test Endpoints**:
   Use Postman or curl to test:
   - **Login**: `POST https://localhost:5001/api/auth/login`
     ```json
     {
       "username": "john",
       "password": "Password123"
     }
     ```
     Copy the returned JWT token.
   - Set `Authorization: Bearer <token>` for all subsequent requests.

## API Endpoints
All endpoints require authentication and a `Region` claim (`UK` or `US`) in the JWT.

### GardenCentres
- **GET /api/v1/GardenCentres?page={page}&pageSize={pageSize}**
  - Returns paginated `GardenCentreDto` objects for the user’s region.
  - Query Parameters: `page` (default: 1), `pageSize` (default: 10).
  - Response:
    ```json
    {
      "totalCount": 2,
      "totalPages": 1,
      "currentPage": 1,
      "pageSize": 10,
      "data": [
        {
          "id": 1,
          "name": "UK Garden 1",
          "locationId": 1,
          "region": "UK",
          "locationName": "London"
        },
        ...
      ]
    }
    ```
- **GET /api/v1/GardenCentres/{id}**
  - Returns a `GardenCentreDto` by ID.
  - Response: `{ "id": 1, "name": "UK Garden 1", "locationId": 1, "region": "UK", "locationName": "London" }`
- **POST /api/v1/GardenCentres**
  - Creates a `GardenCentre`.
  - Request:
    ```json
    {
      "name": "UK Garden 2",
      "locationId": 1,
      "region": "UK"
    }
    ```
  - Response: `201 Created` with the created `GardenCentre`.
- **PUT /api/v1/GardenCentres/{id}**
  - Updates a `GardenCentre`.
  - Request: Same as POST, with matching `id`.
  - Response: `204 No Content`.
- **DELETE /api/v1/GardenCentres/{id}**
  - Deletes a `GardenCentre`.
  - Response: `204 No Content`.

### Locations
- **GET /api/v1/Locations**
  - Returns all `LocationDto` objects for the user’s region.
  - Response: `[ { "id": 1, "name": "London", "region": "UK" }, ... ]`
- **GET /api/v1/Locations/{id}**
  - Returns a `LocationDto` by ID.
  - Response: `{ "id": 1, "name": "London", "region": "UK" }`
- **GET /api/v1/Locations/{id}/GardenCentres**
  - Returns `GardenCentre` objects for a given location ID.
  - Response: `[ { "id": 1, "name": "UK Garden 1", "locationId": 1, "region": "UK" }, ... ]`
- **POST /api/v1/Locations**
  - Creates a `Location`.
  - Request: `{ "name": "Manchester", "region": "UK" }`
  - Response: `201 Created` with `LocationDto`.
- **PUT /api/v1/Locations/{id}**
  - Updates a `Location`.
  - Request: Same as POST, with matching `id`.
  - Response: `204 No Content`.
- **DELETE /api/v1/Locations/{id}**
  - Deletes a `Location` (fails if associated `GardenCentres` exist).
  - Response: `204 No Content`.

### Authentication
- **POST /api/auth/login**
  - Authenticates a user and returns a JWT with a `Region` claim.
  - Request:
    ```json
    {
      "username": "john",
      "password": "Password123"
    }
    ```

## Running Tests
1. Install test dependencies:
   ```xml
   <PackageReference Include="xunit" Version="2.4.*" />
   <PackageReference Include="xunit.runner.visualstudio" Version="2.4.*" />
   <PackageReference Include="Moq" Version="4.18.*" />
   ```
2. Run tests:
   ```bash
   dotnet test
   ```

## Configuration
- **appsettings.json**: Configure connection string, JWT settings, and logging.
- **Caching**: Planned service layer uses `IMemoryCache` with 5-minute cache duration for `GardenCentreDto` and `LocationDto`.
- **Sharding**: Enforced via `Region` in repository methods, validated against JWT claims.
- **Dependency Injection**: Repositories (`IGardenCentreRepository`, `ILocationRepository`, `IUserProfileRepository`) and services (`IGardenCentreService`) are scoped, with `Region` injected via `IHttpContextAccessor`.

## Future Improvements
- **Service Layer**: Fully implement `ILocationService` for caching `LocationDto`.
- **Additional Endpoints**: Add filtering (e.g., `GET /api/v1/Locations?name=London`).
- **Extended Testing**: Add tests for `Create`, `Update`, `Delete` in controllers.
- **Performance**: Optimize cache invalidation strategies.
- **Monitoring**: Add Application Insights for request tracing.

## Troubleshooting
- **Serialization Errors**: Ensure DTOs (`GardenCentreDto`, `LocationDto`) are used to avoid cycles.
- **Sharding Issues**: Verify `Region` claim in JWT and repository validation.
- **Database Errors**: Check migrations and connection string in `appsettings.json`.
- **Logs**: Enable `Debug` logging for `Microsoft.EntityFrameworkCore` and `GardenCentresApi` in `appsettings.json`.