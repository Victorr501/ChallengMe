# ChallengMe! 🏆

**Red social de retos diarios potenciada por IA** — plataforma donde los usuarios completan desafíos generados por inteligencia artificial, acumulan puntos, mantienen rachas y compiten con otros jugadores.

![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=flat&logo=blazor&logoColor=white)
![Azure](https://img.shields.io/badge/Azure-0078D4?style=flat&logo=microsoftazure&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)
![SendGrid](https://img.shields.io/badge/SendGrid-1A82E2?style=flat&logo=sendgrid&logoColor=white)

---

## Índice

- [Arquitectura](#arquitectura)
- [Estructura de carpetas](#estructura-de-carpetas)
- [Requisitos previos](#requisitos-previos)
- [Variables de entorno](#variables-de-entorno)
- [Ejecución en local](#ejecución-en-local)
- [Endpoints de autenticación](#endpoints-de-autenticación)
- [Tests](#tests)
- [Licencia](#licencia)

---

## Arquitectura

El repositorio contiene **dos aplicaciones principales** y varios proyectos de biblioteca organizados en capas:

```
┌─────────────────────────────────────────────────────────────┐
│  ChallengMe.Web  (Blazor InteractiveServer)                 │
│  Frontend SPA — consume la API mediante HttpClient + JWT    │
└────────────────────────────┬────────────────────────────────┘
                             │ HTTPS
┌────────────────────────────▼────────────────────────────────┐
│  ChallengMe.API  (ASP.NET Core 8 — REST)                    │
│  Controllers · Middleware · Extensions (DI, Auth, Swagger)  │
└──┬──────────────┬─────────────┬────────────────────────────┘
   │              │             │
   ▼              ▼             ▼
Services      Repositories  External / Infra
─────────     ───────────   ──────────────────
AuthService   Usuario       AzureAD (OpenID)
JwtService    TokenReset    EmailServices (SendGrid)
              DbFactory     GeminiServices (Gemini AI)
                            Functions (Azure Functions)
```

### Proyectos de la solución

| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `ChallengMe.API` | ASP.NET Core 8 | API REST, controladores, middleware, pipeline HTTP |
| `ChallengMe.Web` | Blazor Server | Interfaz de usuario, cliente de la API |
| `ChallengMe.Services` | Biblioteca | Lógica de negocio, servicios de aplicación |
| `ChallengMe.Repositories` | Biblioteca | Acceso a datos con Dapper + SQL Server |
| `ChallengMe.Models` | Biblioteca | Entidades, DTOs de request/response y opciones de configuración |
| `ChallengMe.AzureAD` | Biblioteca | Autenticación OAuth 2.0 con Microsoft via OpenID Connect |
| `ChallengMe.EmailSevices` | Biblioteca | Envío de emails transaccionales con SendGrid |
| `ChallengMe.GeminiServices` | Biblioteca | Integración con Gemini AI (en desarrollo) |
| `ChallengMe.Functions` | Azure Functions | Tareas en segundo plano y triggers |
| `ChallengMe.Tests.API` | xUnit | Tests unitarios e integración de la API |

---

## Estructura de carpetas

```
ChallengMe/
├── ChallengMe.API/
│   ├── Controllers/
│   │   └── AuthController.cs
│   ├── Extensions/
│   │   ├── AuthExtensions.cs             # JWT Bearer
│   │   ├── AddServiceExtensions.cs       # DI servicios
│   │   ├── AddRepositorysExtensions.cs   # DI repositorios
│   │   ├── RateLimitExtensions.cs        # Políticas de rate limiting
│   │   ├── AddConfigurationExtensions.cs
│   │   └── SwaggerExtensions.cs
│   ├── ExceptionMiddleware/
│   │   └── ExceptionMiddleware.cs        # Manejo global de errores
│   ├── Program.cs
│   └── appsettings.json
│
├── ChallengMe.Web/
│   ├── Models/
│   ├── Services/
│   │   ├── ApiClient.cs
│   │   ├── AuthApiService.cs
│   │   ├── JwtAuthStateProvider.cs       # Estado de autenticación Blazor
│   │   └── AuthTokenHandler.cs           # DelegatingHandler para JWT
│   ├── Tools/
│   ├── Extensions/
│   └── Program.cs
│
├── ChallengMe.Models/
│   ├── Usuario/
│   │   └── Usuario.cs
│   ├── Auth/
│   │   ├── Request/
│   │   │   ├── LoginEmailRequest.cs
│   │   │   ├── RegistroRequest.cs
│   │   │   ├── RecuperarPasswordRequest.cs
│   │   │   ├── ResetPasswordRequest.cs
│   │   │   └── TokenMicrosoftRequest.cs
│   │   ├── Shipment/
│   │   │   ├── AuthResponse.cs
│   │   │   └── TokenResponse.cs
│   │   └── SendGridOptions.cs
│   └── TokensResetPassword/
│       └── TokensResetPassword.cs
│
├── ChallengMe.Services/
│   ├── AuthService/
│   │   ├── IAuthService.cs
│   │   └── AuthService.cs
│   ├── JwtService/
│   │   ├── IJwtService.cs
│   │   └── JwtService.cs
│   └── Exceptions/
│       ├── ChallengeMeException.cs       # Excepción base
│       └── GenericExcepcions/
│           ├── AuthException.cs          # 401
│           ├── ConflictException.cs      # 409
│           ├── NotFoundException.cs      # 404
│           ├── ValidationException.cs    # 400
│           └── Auth/
│               ├── CredencialesInvalidasException.cs
│               ├── EmailYaExisteException.cs
│               ├── ProveedorIncorrectoException.cs
│               ├── TokenMicrosoftInvalidoException.cs
│               ├── TokenResetInvalidoException.cs
│               ├── TokenResetExpiradoException.cs
│               ├── ProveedorNoPermitePasswordException.cs
│               ├── CuentaBloqueadaException.cs
│               └── EmailNoVerificadoException.cs
│
├── ChallengMe.Repositories/
│   ├── UsuarioRepository/
│   │   ├── IUsuarioRepository.cs
│   │   └── UsuarioRepository.cs
│   ├── TokenResetPassWordRepository/
│   │   ├── ITokenResetPasswordRepository.cs
│   │   └── TokenResetPasswordRepository.cs
│   └── DbConnectionFactory/
│       ├── IDbConnectionFactory.cs
│       └── DbConnectionFactory.cs
│
├── ChallengMe.AzureAD/
│   └── AzureAd/
│       ├── IAzureAdService.cs
│       └── AzureAdService.cs
│
├── ChallengMe.EmailSevices/
│   └── EmailSevices/
│       ├── IEmailService.cs
│       └── EmailService.cs
│
├── ChallengMe.GeminiServices/           # En desarrollo
│
├── ChallengMe.Functions/
│   ├── Program.cs
│   └── Function1.cs
│
└── ChallengMe.Tests.API/
    ├── Unit/
    │   ├── Services/AuthServiceTests.cs
    │   ├── Repositories/UsuarioRepositoryTests.cs
    │   └── ExternalAcreditacion/AzureAdServiceTests.cs
    ├── Integration/
    │   └── AuthControllerTests.cs
    └── Helpers/
        ├── Unit/
        │   ├── FakeDbConnectionFactory.cs
        │   ├── FakeHttpMessageHandler.cs
        │   └── GuidTypeHandler.cs
        └── Integration/
            └── CustoWebApplicationFactory.cs
```

---

## Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server 2019+ o Azure SQL Database
- Cuenta de [SendGrid](https://sendgrid.com/) con una API key activa
- Aplicación registrada en [Azure Active Directory](https://portal.azure.com/) con flujo OAuth 2.0
- API key de [Google Gemini](https://aistudio.google.com/) *(opcional — módulo en desarrollo)*
- Visual Studio 2022+ o VS Code con la extensión C# Dev Kit

---

## Variables de entorno

Todos los secretos se gestionan mediante **User Secrets** en desarrollo (`dotnet user-secrets`) o variables de entorno en producción. Nunca se almacenan en `appsettings.json`.

### ChallengMe.API

| Clave | Descripción |
|---|---|
| `ConnectionStrings:SqlDb` | Cadena de conexión a SQL Server |
| `ConnectionStrings:CosmosDb` | Cadena de conexión a Azure Cosmos DB |
| `BlobStorage:ConnectionString` | Cadena de conexión a Azure Blob Storage |
| `BlobStorage:ContainerName` | Nombre del contenedor (`evidencias` por defecto) |
| `Jwt:SecretKey` | Clave secreta para firmar los tokens JWT (mín. 32 caracteres) |
| `Jwt:Issuer` | Issuer del token (`challengeme-api`) |
| `Jwt:Audience` | Audience del token (`challengeme-app`) |
| `Jwt:ExpirationHours` | Duración del token en horas (24 por defecto) |
| `AzureAd:Instance` | Endpoint de Microsoft (`https://login.microsoftonline.com/`) |
| `AzureAd:TenantId` | ID del tenant de Azure AD |
| `AzureAd:ClientId` | ID de la aplicación registrada en Azure AD |
| `AzureAd:ClientSecret` | Secreto de la aplicación Azure AD |
| `AzureAd:RedirectUri` | URI de callback OAuth 2.0 |
| `Gemini:ApiKey` | API key de Google Gemini |
| `Gemini:Model` | Modelo a usar (`gemini-2.0-flash` por defecto) |
| `SendGrid:ApiKey` | API key de SendGrid |
| `SendGrid:EmailRemitente` | Dirección de origen de los emails |
| `SendGrid:NombreRemitente` | Nombre del remitente (`ChallengMe!`) |

#### Configurar User Secrets (desarrollo)

```bash
cd ChallengMe.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:SqlDb" "Server=localhost;Database=ChallengMe;..."
dotnet user-secrets set "Jwt:SecretKey" "tu-clave-secreta-de-al-menos-32-caracteres"
dotnet user-secrets set "AzureAd:TenantId" "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
dotnet user-secrets set "AzureAd:ClientId" "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
dotnet user-secrets set "AzureAd:ClientSecret" "tu-secreto-azure"
dotnet user-secrets set "SendGrid:ApiKey" "SG.xxxxxxxxxxxx"
dotnet user-secrets set "Gemini:ApiKey" "AIza..."
```

---

## Ejecución en local

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/ChallengMe.git
cd ChallengMe
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. Configurar los secretos

Sigue los pasos de la sección [Variables de entorno](#variables-de-entorno).

### 4. Ejecutar la API

```bash
cd ChallengMe.API
dotnet run
# HTTP:  http://localhost:5241
# HTTPS: https://localhost:7256
# Swagger UI: https://localhost:7256/swagger
```

### 5. Ejecutar la aplicación web

```bash
cd ChallengMe.Web
dotnet run
# HTTP:  http://localhost:5071
# HTTPS: https://localhost:7252
```

### 6. Ejecutar las Azure Functions (opcional)

```bash
cd ChallengMe.Functions
func start
```

---

## Endpoints de autenticación

Base URL: `/api/auth`

| Método | Ruta | Descripción | Rate Limit |
|---|---|---|---|
| `POST` | `/login-microsoft` | Autenticación con cuenta Microsoft (OAuth 2.0 + OpenID Connect) | — |
| `POST` | `/login-email` | Login con email y contraseña | 5 intentos / 15 min |
| `POST` | `/registro` | Registro de nuevo usuario | 3 solicitudes / hora |
| `POST` | `/recuperar-password` | Solicita token de restablecimiento por email | 3 solicitudes / hora |
| `POST` | `/reset-password` | Establece nueva contraseña con el token recibido | 3 solicitudes / hora |

> Política global: **20 peticiones por minuto por IP** sobre todos los endpoints.

### Ejemplos de request/response

#### POST `/login-email`
```json
// Request
{ "email": "usuario@ejemplo.com", "password": "MiPassword123!" }

// Response 200
{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." }
```

#### POST `/registro`
```json
// Request
{ "email": "usuario@ejemplo.com", "password": "MiPassword123!", "nombreUsuario": "victor123" }

// Response 200
{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." }
```

#### POST `/login-microsoft`
```json
// Request
{ "tokenMicrosoft": "code_de_microsoft_oauth" }

// Response 200
{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." }
```

#### POST `/recuperar-password`
```json
// Request
{ "email": "usuario@ejemplo.com" }
// Response 200 — envía email con enlace de recuperación (token válido 1 hora)
```

#### POST `/reset-password`
```json
// Request
{ "token": "token-de-reset", "nuevaPassword": "NuevaPassword456!" }
// Response 200
```

### Códigos de error

| Código HTTP | Situación |
|---|---|
| `400` | Datos de entrada inválidos |
| `401` | Credenciales incorrectas, token Microsoft inválido, token de reset inválido o expirado |
| `409` | El email ya está registrado |
| `429` | Límite de peticiones superado |
| `500` | Error interno del servidor |

---

## Tests

El proyecto `ChallengMe.Tests.API` cubre la API con tests unitarios e integración usando **xUnit**, **Moq** y **FluentAssertions**.

### Ejecutar todos los tests

```bash
dotnet test ChallengMe.Tests.API
```

### Solo tests unitarios

```bash
dotnet test ChallengMe.Tests.API --filter "FullyQualifiedName~Unit"
```

### Solo tests de integración

```bash
dotnet test ChallengMe.Tests.API --filter "FullyQualifiedName~Integration"
```

### Con informe de cobertura

```bash
dotnet test ChallengMe.Tests.API --collect:"XPlat Code Coverage"
```

### Descripción de los tests

| Archivo | Tipo | Qué cubre |
|---|---|---|
| `Unit/Services/AuthServiceTests.cs` | Unitario | Lógica de login, registro y reset de contraseña |
| `Unit/Repositories/UsuarioRepositoryTests.cs` | Unitario | CRUD de usuarios contra SQLite in-memory |
| `Unit/ExternalAcreditacion/AzureAdServiceTests.cs` | Unitario | Validación de tokens de Microsoft |
| `Integration/AuthControllerTests.cs` | Integración | Endpoints HTTP completos con `CustomWebApplicationFactory` |

Los tests de integración sustituyen SQL Server por **SQLite in-memory** (`Microsoft.Data.Sqlite`) y mockean las llamadas HTTP externas mediante `FakeHttpMessageHandler`.

---

## Licencia

Copyright © 2026 Víctor Rubín Rubio. Todos los derechos reservados.

Este software y su código fuente son propiedad exclusiva del autor. Queda **estrictamente prohibido**:

- Copiar, modificar o distribuir este software o cualquier parte del mismo.
- Usar este software con fines comerciales sin autorización expresa y por escrito del autor.
- Realizar ingeniería inversa, descompilar o desensamblar el software.
- Sublicenciar o transferir los derechos sobre el software a terceros.

El acceso a este repositorio se concede únicamente con fines de revisión y evaluación. Cualquier otro uso requiere la autorización previa y por escrito del autor.

Para solicitar una licencia comercial o de uso, contacta en: **vrubinr501@gmail.com**
