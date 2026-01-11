# Docker Compose Standards

Use explicit image versions, health checks, and dependency ordering for API, Web, and DB services.

```yaml
version: '3.8'
services:
  api:
    image: globoticket-api:1.2.3
    container_name: globoticket-api
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=GloboTicket;Uid=globoticket;Pwd=password;
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    depends_on:
      db:
        condition: service_healthy
    restart: unless-stopped

  web:
    image: globoticket-web:1.2.3
    container_name: globoticket-web
    ports:
      - "3000:3000"
    environment:
      - REACT_APP_API_URL=http://localhost:8080/api
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:3000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
    depends_on:
      - api
    restart: unless-stopped

  db:
    image: postgres:15.2-alpine
    container_name: globoticket-db
    environment:
      - POSTGRES_DB=GloboTicket
      - POSTGRES_USER=globoticket
      - POSTGRES_PASSWORD=password
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./docker/init-db:/docker-entrypoint-initdb.d
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U globoticket -d GloboTicket"]
      interval: 10s
      timeout: 5s
      retries: 5
    restart: unless-stopped

volumes:
  postgres_data:
```
