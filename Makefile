.PHONY: up down build logs migrate seed

# Start all services
up:
	docker-compose up -d

# Stop all services
down:
	docker-compose down

# Build Docker images
build:
	docker-compose build

# View API logs
logs:
	docker-compose logs -f api

# Run EF migrations locally
migrate:
	dotnet ef database update --project WifiCdmx.Infrastructure --startup-project WifiCdmx.API

# Remove volumes (reset database)
clean:
	docker-compose down -v
