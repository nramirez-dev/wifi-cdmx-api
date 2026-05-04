# WiFi CDMX API

REST API for querying public WiFi access points in Mexico City, built with .NET 8 and PostgreSQL.

## Preview

The `/api/wifi-points/heatmap` endpoint returns geographic grid data ready to be visualized with tools like [Kepler.gl](https://kepler.gl/demo).

![WiFi CDMX Heatmap](docs/heatmap-preview.png)

*Distribution of 539 public WiFi access points across Mexico City grouped into geographic grid cells. Denser clusters indicate higher WiFi coverage.*

---

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

![System Architecture](docs/architecture.png)

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
- Docker Desktop installed and running
- `make` (optional but recommended)
- A copy of `wifi_cdmx.csv` placed inside the `data/` folder at the solution root

### 1. Clone the repository
```bash
git clone https://github.com/nramirez-dev/wifi-cdmx-api.git
cd wifi-cdmx-api
```

### 2. Set up environment variables
```bash
cp .env.example .env
```
Open `.env` and set your preferred database password.

### 3. Add the CSV dataset
Place `wifi_cdmx.csv` inside the `data/` folder:
```
wifi-cdmx-api/
└── data/
    └── wifi_cdmx.csv
```
The API will automatically read, process and seed the database on first startup.

### 4. Start all services
```bash
make up
# or
docker-compose up -d
```

### 5. Verify the API is running
```bash
make logs
# or
docker-compose logs -f api
```
You should see: `Seeded 539 WiFi points successfully`

### 6. Open Swagger UI
http://localhost:5000/swagger

### Available commands
| Command | Description |
|---|---|
| `make up` | Start all services |
| `make down` | Stop all services |
| `make build` | Rebuild Docker images |
| `make logs` | Follow API logs |
| `make migrate` | Run EF Core migrations locally |
| `make clean` | Remove containers and volumes |

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

## Bonus Features

### Heatmap Endpoint
`GET /api/wifi-points/heatmap?gridSize=0.01`

Returns WiFi points aggregated into geographic grid cells, ready to feed mapping libraries like Kepler.gl, Leaflet or Google Maps. The `gridSize` parameter controls cell size in degrees (default `0.01` ≈ 1km²).

### Stats Endpoint
`GET /api/wifi-points/stats`

Returns aggregated statistics across the full dataset — total points, total access points, and breakdowns by borough and program. Useful for dashboards and data exploration.

### Functional Programming
Functional programming principles were applied throughout: pure static mapping functions (`ToDto`, `ToPagedResult`), immutable DTOs using C# records, and LINQ method chaining for data transformations instead of imperative loops.

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
