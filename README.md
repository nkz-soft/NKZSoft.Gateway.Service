# NKZSoft Gateway Service

A modern microservices architecture built with .NET 9.0, featuring an API Gateway with authentication, authorization, and reverse proxy capabilities.

## 🏗️ Architecture

This project implements a microservices architecture with the following components:

- **Gateway API**: Main entry point using YARP (Yet Another Reverse Proxy) for request routing and OpenID Connect authentication
- **Data API**: Backend service providing data access and business logic
- **Blazor Frontend**: Web UI components for administration and user interfaces
- **Keycloak**: Identity and access management server for authentication and authorization
- **PostgreSQL**: Primary database for data persistence

## 🚀 Technologies

- **Backend**: .NET 9.0, ASP.NET Core
- **Gateway**: YARP Reverse Proxy
- **Authentication**: OpenID Connect with Keycloak (configured for legacy compatibility)
- **Frontend**: Blazor Server/WebAssembly
- **Database**: PostgreSQL 15
- **Containerization**: Docker & Docker Compose
- **Testing**: xUnit, ASP.NET Core Testing Framework

> **Note**: This project uses .NET 9.0 with custom configurations to ensure compatibility with Keycloak legacy. The OpenID Connect implementation automatically falls back to standard OAuth 2.0 flow to avoid PAR (Pushed Authorization Request) issues.

## 📋 Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/install/)
- [Git](https://git-scm.com/downloads)

## 🚦 Quick Start

### 1. Clone the Repository

```bash
git clone <repository-url>
cd NKZSoft.Gateway.Service
```

### 2. Setup Environment Variables

Copy the environment example file and configure your settings:

```bash
cp deployment/env.example deployment/.env
```

Edit `deployment/.env` with your preferred configuration:

```env
# Database Configuration
POSTGRES_PASSWORD=your_secure_password

# Keycloak Configuration
KEYCLOAK_POSTGRES_HOST=postgres
KEYCLOAK_DATABASE_NAME=keycloak
KEYCLOAK_DATABASE_SCHEMA_NAME=keycloak
KEYCLOAK_DATABASE_USERNAME=postgres
KEYCLOAK_DATABASE_PASSWORD=your_secure_password

# Keycloak Admin
KEYCLOAK_USER=admin
KEYCLOAK_PASSWORD=admin_password
KEYCLOAK_SRV=http://keycloak:8080/auth
KEYCLOAK_REALM=test
```

### 3. Start with Docker Compose

From the deployment directory:

```bash
cd deployment
docker-compose up --build
```

This will start all services:
- **PostgreSQL**: http://localhost:5432
- **Keycloak**: http://localhost:8080
- **Gateway API**: http://localhost:5000
- **Data API**: http://localhost:5050

### 4. Access the Application

- **Keycloak Admin Console**: http://localhost:8080/auth/admin
  - Username: `admin`
  - Password: `admin` (or as configured in .env)

- **Gateway API**: http://localhost:5000
- **Data API**: http://localhost:5050

## 🛠️ Development Setup

### Local Development

1. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```

2. **Build the Solution**:
   ```bash
   dotnet build
   ```

3. **Run Tests**:
   ```bash
   dotnet test
   ```

4. **Run Individual Services**:
   ```bash
   # Gateway API
   cd src/NKZSoft.Gateway.API
   dotnet run

   # Data API
   cd src/NKZSoft.Data.API
   dotnet run
   ```

### Development with External Dependencies

For local development with external services (PostgreSQL, Keycloak):

```bash
# Start only infrastructure services
docker-compose up postgres keycloak

# Run .NET services locally
dotnet run --project src/NKZSoft.Gateway.API
dotnet run --project src/NKZSoft.Data.API
```

## ⚙️ Configuration

### Gateway API Configuration

The Gateway API uses YARP for reverse proxy configuration. Key configuration files:

- `src/NKZSoft.Gateway.API/appsettings.json`: Base configuration
- `src/NKZSoft.Gateway.API/appsettings.Development.json`: Development overrides
- `src/NKZSoft.Gateway.API/appsettings.Docker.json`: Docker environment settings

### Authentication Configuration

OpenID Connect settings are configured in the `OpenIDConnect` section:

```json
{
  "OpenIDConnect": {
    "Authority": "http://localhost:8080/auth",
    "MetadataAddress": "http://localhost:8080/auth/realms/Test/.well-known/openid-configuration",
    "ClientId": "yarn-proxy",
    "ClientSecret": "pHaEdIKaFTOuJvxTFsNezENmPYUpx9yA",
    "RedirectUrl": "http://localhost:5000"
  }
}
```

### Authentication Flow

1. **Access Protected Route**: When accessing `/app/*` routes (protected by authorization policy)
2. **Redirect to Login**: Unauthenticated users are redirected to `/login`
3. **OIDC Challenge**: The `/login` endpoint initiates OpenID Connect authentication with Keycloak
4. **Keycloak Authentication**: User authenticates via Keycloak UI
5. **Callback**: Keycloak redirects back to `/signin-oidc` with authorization code
6. **Token Exchange**: Gateway exchanges code for access token and creates authentication cookie
7. **Access Granted**: User can now access protected routes

### Reverse Proxy Configuration

YARP routing is configured in the `ReverseProxy` section of appsettings.json:

```json
{
  "ReverseProxy": {
    "Routes": {
      "data-api": {
        "ClusterId": "data-cluster",
        "Match": {
          "Path": "/api/data/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "data-cluster": {
        "Destinations": {
          "destination1": {
            "Address": "http://localhost:5050/"
          }
        }
      }
    }
  }
}
```

## 🧪 Testing

Run all tests:

```bash
dotnet test
```

Run tests with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

- `tests/NKZSoft.Gateway.API.Tests/`: Gateway API integration and unit tests
- `tests/NKZSoft.Gateway.API.Tests/Controllers/`: Controller-specific tests
- `tests/NKZSoft.Gateway.API.Tests/Common/`: Shared test utilities

## 📦 Deployment

### Production Deployment

1. **Build Production Images**:
   ```bash
   cd deployment
   docker-compose -f docker-compose.yaml build
   ```

2. **Deploy to Production**:
   ```bash
   docker-compose -f docker-compose.yaml up -d
   ```

### Environment-Specific Configurations

- **Development**: Uses `appsettings.Development.json`
- **Docker**: Uses `appsettings.Docker.json`
- **Production**: Configure production-specific settings in your deployment environment

## 🔧 API Documentation

### Gateway API Endpoints

- **Health Check**: `GET /healthz`
- **Login**: `GET /login` - Initiates OpenID Connect authentication flow
- **Logout**: `GET /logout` - Signs out the user
- **User Info**: `GET /userinfo` - Returns current user claims (requires authentication)
- **Authentication**: Handled via OpenID Connect flow with Keycloak

### Data API Endpoints

- **Weather Forecast**: `GET /WeatherForecast`
- **Health Check**: `GET /healthz`

API documentation is available via Swagger when running in development mode:
- Gateway API: http://localhost:5000/swagger
- Data API: http://localhost:5050/swagger

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

### Development Guidelines

- Follow .NET coding standards and conventions
- Write unit tests for new functionality
- Update documentation for API changes
- Ensure all tests pass before submitting PR

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Troubleshooting

### Common Issues

1. **Port Conflicts**: Ensure ports 5000, 5050, 5432, and 8080 are available
2. **Docker Issues**: Run `docker-compose down` and `docker-compose up --build` to rebuild
3. **Authentication Issues**: 
   - Verify Keycloak is running: `curl http://localhost:8080/auth/realms/Test/.well-known/openid-configuration`
   - Check Keycloak admin console: http://localhost:8080/auth/admin
   - Verify client secret matches between app config and Keycloak client settings
   - Ensure redirect URIs include `/signin-oidc` and `/signout-callback-oidc` endpoints
4. **Database Connection**: Check PostgreSQL container logs and connection strings
5. **OpenID Connect Errors**:
   - Check container logs: `docker-compose logs gateway-service`
   - Verify Authority URL format: should be `http://keycloak:8080/auth/realms/Test`
   - Ensure MetadataAddress is reachable from within Docker network
   - **PAR (Pushed Authorization Request) Issues**: 
     - .NET 9.0 tries to use PAR by default, but Keycloak legacy doesn't support it properly
     - The application is configured to automatically fall back to standard OAuth flow
     - If you see "invalid_request" errors related to PAR, restart the services: `docker-compose restart gateway-service`

### Support

For support and questions:
- Create an issue in the repository
- Check existing documentation
- Review container logs: `docker-compose logs [service-name]`

---

