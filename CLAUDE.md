# CLAUDE.md — Project Guidelines for Claude Code

## Project
WiFi CDMX API — Backend REST service for querying Mexico City public WiFi access points.
Built with .NET 8, PostgreSQL, Entity Framework Core and Docker.

## Stack
- .NET 8 / C#
- PostgreSQL
- Entity Framework Core
- CsvHelper
- StackExchange.Redis
- Docker + docker-compose
- xUnit + Moq

## Code Rules
- All code, comments, variable names and commit messages must be in English
- Follow Clean Architecture: Domain → Application → Infrastructure → API
- Use primary constructors for dependency injection
- Use records for DTOs
- All public methods and classes must have XML summary comments
- No Spanish in code — CSV column names are only mapped in CsvSeeder.cs

## Project Structure
- `WifiCdmx.Domain` — Entities only, zero dependencies
- `WifiCdmx.Application` — Interfaces, Services, DTOs
- `WifiCdmx.Infrastructure` — EF Core, Repositories, Seeders
- `WifiCdmx.API` — Controllers, Middleware, Program.cs
- `WifiCdmx.Tests` — xUnit tests with Moq

## Git Rules — IMPORTANT
- After completing every task or file creation, always make a commit
- Use conventional commits format:
  - `feat:` for new features or files
  - `fix:` for bug fixes
  - `chore:` for maintenance tasks
  - `docs:` for documentation
  - `test:` for adding tests
- Commit messages must be descriptive and in English
- Do NOT add Co-authored-by tags to any commit
- Push to origin main after every commit

## Commit hook
After every task run:
```
git add .
git commit -m "your descriptive message here"
git push origin main
```
