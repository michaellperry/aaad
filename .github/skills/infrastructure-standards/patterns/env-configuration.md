# Environment Configuration

Sample `.env` structure for compose and app configuration.

```yaml
COMPOSE_PROJECT_NAME=globoticket

# Database
POSTGRES_VERSION=15.2-alpine
POSTGRES_DB=GloboTicket
POSTGRES_USER=globoticket
POSTGRES_PASSWORD=password

# API
API_VERSION=1.2.3
API_PORT=8080
ASPNETCORE_ENVIRONMENT=Development

# Web
WEB_VERSION=1.2.3
WEB_PORT=3000
REACT_APP_API_URL=http://localhost:8080/api
```
