# GlossaryService
Microservice responsible for managing product tags and search keywords/synonyms for the ZipCart grocery delivery platform.

## Tech Stack
- .NET 10 / ASP.NET Core
- SQL Server
- Entity Framework Core
- Docker + Docker Compose

## Features
- Product tag management with CRUD
- Search keyword and synonym management with CRUD
- Pagination and search filtering
- Soft delete pattern
- Global error handling middleware
- Consistent API response wrapper
- API versioning (v1)
- Rate limiting
- Unit tests with xUnit and Moq

## Getting Started

### Run With Docker
docker-compose up --build
API available at: http://localhost:5050/swagger

### Run Without Docker
Update connection string in appsettings.json then:
dotnet run --project GlossaryService

### Run Tests
dotnet test

## Project Structure

GlossaryService/
├── Common/         → Constants, shared DTOs
├── Controllers/    → API endpoints
├── Data/           → DbContext
├── DTOs/           → Request and response models
├── Middlewares/    → Global error handling
├── Migrations/     → EF Core migrations
├── Models/         → Database entities
├── Repository/     → Data access layer
└── Services/       → Business logic layer

GlossaryService.Tests/
└── Services/       → Unit tests for TagService and KeywordService
