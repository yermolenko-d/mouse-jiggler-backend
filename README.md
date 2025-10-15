# Virtual Mouse Jiggler Backend

A layered architecture ASP.NET Core Web API for the Virtual Mouse Jiggler Chrome Extension.

## Project Structure

This solution follows a clean architecture pattern with the following layers:

### 🏗️ **Architecture Overview**

```
┌─────────────────────────────────────┐
│           Presentation Layer        │
│        (MouseJigglerBackend)        │
│         - Controllers               │
│         - API Endpoints             │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│         Business Logic Layer        │
│        (MouseJigglerBackend.BLL)    │
│         - Services                  │
│         - Business Rules            │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│         Data Access Layer           │
│        (MouseJigglerBackend.DAL)    │
│         - Repositories              │
│         - DbContext                 │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│            Core Layer               │
│        (MouseJigglerBackend.Core)   │
│         - Entities                  │
│         - DTOs                      │
│         - Interfaces                │
└─────────────────────────────────────┘
```

### 📁 **Project Details**

#### **MouseJigglerBackend.Core**
- **Purpose**: Contains shared entities, DTOs, and interfaces
- **Contains**:
  - `Entities/` - Domain models (User, ActivationKey, Subscription)
  - `DTOs/` - Data Transfer Objects for API communication
  - `Interfaces/` - Repository and service contracts

#### **MouseJigglerBackend.DAL**
- **Purpose**: Data Access Layer with Entity Framework Core
- **Contains**:
  - `Data/ApplicationDbContext.cs` - EF Core DbContext
  - `Repositories/` - Repository implementations
- **Dependencies**: Core project, Entity Framework packages

#### **MouseJigglerBackend.BLL**
- **Purpose**: Business Logic Layer containing service implementations
- **Contains**:
  - `Services/` - Business logic services
- **Dependencies**: Core project, DAL project

#### **MouseJigglerBackend** (API)
- **Purpose**: Presentation layer with controllers and API endpoints
- **Contains**:
  - `Controllers/` - API controllers
  - `Program.cs` - Application configuration and DI setup
- **Dependencies**: All other projects

## 🚀 **Getting Started**

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL 12+ (installed locally)
- Visual Studio 2022 or VS Code

### Setup
1. **Clone the repository**
2. **Install PostgreSQL**:
   - **Windows**: Download from [postgresql.org](https://www.postgresql.org/download/windows/)
   - **macOS**: `brew install postgresql` or download from postgresql.org
   - **Linux**: `sudo apt-get install postgresql postgresql-contrib` (Ubuntu/Debian)

3. **Create PostgreSQL database**:
   ```sql
   CREATE DATABASE "MouseJigglerDb_Dev";
   CREATE DATABASE "MouseJigglerDb";
   ```

4. **Update connection string** in `appsettings.json` and `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=MouseJigglerDb_Dev;Username=postgres;Password=your_password_here"
   }
   ```

5. **Restore packages**:
   ```bash
   dotnet restore
   ```

6. **Create and apply migrations**:
   ```bash
   dotnet ef database update --project MouseJigglerBackend.DAL --startup-project MouseJigglerBackend
   ```

7. **Run the application**:
   ```bash
   dotnet run --project MouseJigglerBackend
   ```


### Database Migration
To create a new migration:
```bash
dotnet ef migrations add MigrationName --project MouseJigglerBackend.DAL --startup-project MouseJigglerBackend
```

## 📊 **Database Schema**

### **Users Table**
- `Id` (Primary Key)
- `Email` (Unique)
- `FirstName`, `LastName`
- `CreatedAt`, `LastLoginAt`
- `IsActive`

### **ActivationKeys Table**
- `Id` (Primary Key)
- `Key` (Unique activation key)
- `UserId` (Foreign Key)
- `CreatedAt`, `ActivatedAt`, `ExpiresAt`
- `IsActive`, `Notes`

### **Subscriptions Table**
- `Id` (Primary Key)
- `UserId` (Foreign Key)
- `PlanName`, `Status`
- `StartDate`, `EndDate`
- `CreatedAt`, `UpdatedAt`
- `IsActive`
- `StripeSubscriptionId`, `StripeCustomerId`

## 🔌 **API Endpoints**

### **GET** `/api/home`
- Returns API status and version information

### **POST** `/api/home/check-activation-key`
- Validates activation keys
- **Request Body**: `ActivationKeyValidationRequest`
- **Response**: `ActivationKeyValidationResponse`

## 🛠️ **Key Features**

- **Clean Architecture**: Separation of concerns with layered approach
- **Dependency Injection**: Proper DI container setup
- **Entity Framework Core**: Code-first approach with migrations
- **Repository Pattern**: Abstracted data access
- **Service Layer**: Business logic encapsulation
- **DTOs**: Type-safe data transfer
- **CORS Support**: Cross-origin requests enabled
- **Swagger/OpenAPI**: API documentation

## 🔧 **Configuration**

### Connection Strings
- **Development**: `MouseJigglerDb_Dev` (PostgreSQL)
- **Production**: `MouseJigglerDb` (PostgreSQL)

### CORS Policy
- **AllowAll**: Permits all origins, methods, and headers (for development)

## 📝 **Development Notes**

- All projects target .NET 9.0
- Entity Framework Core 9.0 with PostgreSQL provider
- Npgsql.EntityFrameworkCore.PostgreSQL for PostgreSQL integration
- Swagger for API documentation
- Nullable reference types enabled
- Implicit usings enabled

## 🐘 **PostgreSQL Features**

- **JSON Support**: Native JSON/JSONB column types
- **Full-Text Search**: Advanced text search capabilities
- **Array Support**: Native array data types
- **Extensions**: Support for PostGIS, pg_stat_statements, etc.
- **Performance**: Excellent performance for read-heavy workloads

## 🚀 **Deployment**

The API is designed to be deployed to cloud platforms like:
- Azure App Service (with Azure Database for PostgreSQL)
- Heroku (with Heroku Postgres addon)
- AWS Elastic Beanstalk (with RDS PostgreSQL)
- Railway
- DigitalOcean App Platform

### Environment Variables for Production
Set these environment variables in your hosting platform:
```
ConnectionStrings__DefaultConnection=Host=your_host;Database=MouseJigglerDb;Username=your_username;Password=your_password;SSL Mode=Require
```

Make sure to update connection strings and CORS policies for production deployment.
