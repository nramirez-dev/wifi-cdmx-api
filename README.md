# WiFi CDMX API

REST API for querying public WiFi access points in Mexico City, built with .NET 8 and PostgreSQL.

## Table of Contents
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Environment Variables](#environment-variables)
- [Database Schema](#database-schema)
- [Design Decisions](#design-decisions)

---

## Architecture

This project follows **Clean Architecture** with clear separation of concerns across 4 layers:
```
┌─────────────────────────────────────┐
│           WifiCdmx.API              │  HTTP Controllers, Swagger, Middleware
├─────────────────────────────────────┤
│        WifiCdmx.Application         │  Business Logic, Interfaces, DTOs
├─────────────────────────────────────┤
│       WifiCdmx.Infrastructure       │  EF Core, PostgreSQL, CSV Seeder
├─────────────────────────────────────┤
│          WifiCdmx.Domain            │  Entities (no dependencies)
└─────────────────────────────────────┘
```

**Dependency rule:** outer layers depend on inner layers, never the reverse.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 8 |
| Framework | ASP.NET Core Web API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 8 |
| CSV Parsing | CsvHelper |
| Documentation | Swagger / OpenAPI |
| Containerization | Docker + docker-compose |
| Testing | xUnit + Moq |

---

## Project Structure

```
WifiCdmx/
├── WifiCdmx.API/                  # Entry point
│   ├── Controllers/
│   │   └── WifiPointsController.cs
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
├── WifiCdmx.Application/          # Business logic
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
├── WifiCdmx.Infrastructure/       # Data access
│   ├── Data/
│   ├── Migrations/
│   ├── Repositories/
│   └── Seeders/
├── WifiCdmx.Domain/               # Core entities
│   └── Entities/
├── WifiCdmx.Tests/                # Unit tests
│   └── Services/
├── NuGet.Config
├── data/
│   └── wifi_cdmx.csv              # Source dataset
├── docker-compose.yml
├── Makefile
└── .env.example
```

---

## API Endpoints

Base URL: `http://localhost:5000/api`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/wifi-points` | Paginated list of all WiFi points |
| GET | `/wifi-points/{id}` | Single WiFi point by ID |
| GET | `/wifi-points/borough/{borough}` | WiFi points filtered by borough |
| GET | `/wifi-points/nearby?lat={}&lon={}` | WiFi points ordered by proximity |
| GET | `/wifi-points/stats` | Aggregated statistics |

### Query Parameters

All list endpoints support:
- `page` (default: 1)
- `pageSize` (default: 20)

### Example Responses

**GET /api/wifi-points**
```json
{
  "data": [
    {
      "id": "T-III Dr. Manuel Escontria",
      "neighborhood": "SAN ANGEL",
      "borough": "ÁLVARO OBREGÓN",
      "latitude": 19.3428,
      "longitude": -99.1933,
      "accessPointCount": 2,
      "program": "Centros_de_Salud"
    }
  ],
  "total": 539,
  "page": 1,
  "pageSize": 20
}
```

**GET /api/wifi-points/stats**
```json
{
  "totalPoints": 539,
  "totalAccessPoints": 1250,
  "byBorough": {
    "ÁLVARO OBREGÓN": 45,
    "IZTAPALAPA": 120
  },
  "byProgram": {
    "PILARES": 224,
    "Centros_de_Salud": 215,
    "Sitios_Publicos": 100
  }
}
```

---

## Getting Started

### Prerequisites
- Docker Desktop
- make (optional but recommended)

### 1. Clone the repository
```bash
git clone https://github.com/nramirez-dev/wifi-cdmx-api.git
cd wifi-cdmx-api
```

### 2. Set up environment variables
```bash
cp .env.example .env
# Edit .env with your preferred credentials
```

### 3. Add the CSV dataset
Place the `wifi_cdmx.csv` file inside the `data/` folder at the project root.
The API will automatically seed the database on first startup.

### 4. Start the services
```bash
make up
# or
docker-compose up -d
```

### 5. Access the API
- Swagger UI: http://localhost:5000/swagger
- API Base: http://localhost:5000/api/wifi-points

### Available Make commands
```bash
make up       # Start all services
make down     # Stop all services
make build    # Rebuild Docker images
make logs     # Follow API logs
make migrate  # Run EF Core migrations locally
make clean    # Remove containers and volumes (resets DB)
```

---

## Environment Variables

| Variable | Description | Default |
|---|---|---|
| `POSTGRES_DB` | Database name | `wifi_cdmx` |
| `POSTGRES_USER` | Database user | `postgres` |
| `POSTGRES_PASSWORD` | Database password | — |
| `CSV_FILE_PATH` | Path to the CSV file inside the container | `/app/Data/wifi_cdmx.csv` |

---

## Database Schema

### Table: `WifiPoints`

| Column | Type | Description |
|---|---|---|
| `Id` | `varchar(200)` | Primary key — taken from CSV |
| `Neighborhood` | `varchar(100)` | Colonia where the point is located |
| `Borough` | `varchar(100)` | Alcaldía (indexed for fast filtering) |
| `Latitude` | `double precision` | Geographic latitude |
| `Longitude` | `double precision` | Geographic longitude |
| `AccessPointCount` | `int` | Number of APs at this location |
| `Program` | `varchar(100)` | Installation program (indexed) |

---

## Design Decisions

**Why PostgreSQL?**
Relational structure fits the dataset well. Indexes on `Borough` and `Program` make the most common queries fast without needing a full-text search engine.

**Why Haversine in EF Core instead of PostGIS?**
PostGIS adds operational complexity. For a dataset of ~500 points, the Haversine formula translated by EF Core into SQL is efficient enough and keeps the stack simple.

**Why Clean Architecture?**
Separating Domain, Application and Infrastructure makes the codebase testable — the service layer is fully unit-testable with mocked repositories, no database required.

**Why CSV auto-seed on startup?**
Zero manual steps to get a running instance. `docker-compose up` is all you need.

**Stats endpoint (bonus)**
Not required by the spec but natural for a data company — aggregated views of the dataset show awareness of the data beyond simple CRUD.
