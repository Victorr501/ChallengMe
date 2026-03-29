# ChallengMe

Plataforma de retos de programación competitiva con sistema de usuarios, puntos y rachas. Los usuarios se autentican mediante Microsoft (Azure AD) o credenciales propias, resuelven retos y acumulan puntos que reflejan su progreso.

## Tecnologías

| Capa | Tecnología |
|---|---|
| Backend API | ASP.NET Core 8 (REST) |
| Frontend | Blazor Server (.NET 8) |
| ORM | Dapper |
| Base de datos | SQL Server (Azure SQL) |
| Documentos | Azure Cosmos DB |
| Almacenamiento | Azure Blob Storage |
| Autenticación | Microsoft Entra ID (Azure AD) + JWT |
| Funciones background | Azure Functions v4 (Timer Trigger) |
| IA (pendiente) | Google Gemini |

## Estructura del proyecto

```
ChallengMe/
├── ChallengMe.API/          # API REST (autenticación, endpoints)
├── ChallengMe.Web/          # Frontend Blazor Server
├── ChallengMe.Services/     # Lógica de negocio (auth, JWT)
├── ChallengMe.Repositories/ # Acceso a datos con Dapper
├── ChallengMe.Models/       # Modelos compartidos
├── ChallengMe.AzureAD/      # Integración Microsoft Entra ID
├── ChallengMe.Functions/    # Azure Functions (tareas en background)
└── ChallengMe.GeminiServices/ # Integración IA (en desarrollo)
```

## Configuración

### 1. Variables de entorno

Copia `.env.example` como `.env` y rellena los valores:

```bash
cp .env.example .env
```

Variables requeridas:

| Variable | Descripción |
|---|---|
| `SQL_PASSWORD` | Contraseña del usuario de SQL Server |
| `COSMOS_KEY` | Clave de acceso de Cosmos DB |
| `BLOB_CONNECTION_STRING` | Cadena de conexión de Azure Blob Storage |
| `JWT_SECRET_KEY` | Clave secreta para firmar tokens JWT (mínimo 32 caracteres) |
| `AZURE_AD_CLIENT_SECRET` | Secreto de la app registrada en Microsoft Entra ID |

### 2. appsettings.json

Los proyectos `ChallengMe.API` y `ChallengMe.Web` leen configuración desde `appsettings.json`. Rellena los campos marcados con `TU_*` con los valores reales (o usa `appsettings.Development.json` para sobreescribirlos localmente sin tocar el archivo base).

**Nunca subas contraseñas ni claves reales a git.**

### 3. Azure Functions

El proyecto `ChallengMe.Functions` usa `local.settings.json` para desarrollo local. Este archivo ya está en `.gitignore`.

## Ejecución local

### API

```bash
cd ChallengMe.API
dotnet run
# HTTP:  http://localhost:5241
# HTTPS: https://localhost:7256
# Swagger: https://localhost:7256/swagger
```

### Web (Blazor)

```bash
cd ChallengMe.Web
dotnet run
# HTTP:  http://localhost:5071
# HTTPS: https://localhost:7252
```

### Azure Functions

```bash
cd ChallengMe.Functions
func start
```

## Autenticación

El sistema soporta dos flujos:

1. **Microsoft (SSO):** El frontend obtiene un token de Microsoft y lo envía a la API. La API lo valida contra el endpoint OpenID Connect de Azure AD y genera un JWT propio.
2. **Credenciales propias:** Email + contraseña hasheada con BCrypt. La API devuelve un JWT.

## Modelo de usuario

| Campo | Tipo | Descripción |
|---|---|---|
| `Id` | Guid | Identificador único |
| `Email` | string | Email del usuario |
| `PasswordHash` | string? | Null para usuarios SSO |
| `NombreUsuario` | string | Nombre de pantalla |
| `NivelDificultad` | byte (1-3) | Nivel elegido por el usuario |
| `PuntosTotal` | int | Puntos acumulados |
| `RachaActual` | int | Racha activa actual |
| `RachaMaxima` | int | Mejor racha histórica |
| `UltimaActividad` | DateOnly? | Fecha del último reto resuelto |
| `FechaRegistro` | DateTime | Fecha de alta |

## Recursos Azure

| Recurso | Nombre |
|---|---|
| SQL Server | `sql-challengeme.database.windows.net` |
| Base de datos | `db-challengeme` |
| Cosmos DB | `cosmos-challengeme.documents.azure.com` |
| Blob Storage | `storagechallengeme` |
| Contenedor blobs | `evidencias` |
| Cosmos DB database | `challengeme-db` |
| Cosmos DB container | `perfiles` |
