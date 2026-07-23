# MyShopClean

MyShopClean is a Persian, right-to-left e-commerce application built with ASP.NET Core MVC, Entity Framework Core, SQL Server, and ASP.NET Core Identity. The solution follows a layered architecture and currently targets **.NET 10 LTS**.

The repository includes a Docker-based development environment, dynamic storefront settings, protected marketplace feeds for Torob and E-malls, payment gateway integrations, automated tests, and responsive-layout auditing.

## Features

- Persian and right-to-left storefront
- Responsive pages for mobile, tablet, and desktop
- Product, category, banner, discount, inventory, and attribute management
- Shopping cart, favorites, comments, questions, and order history
- User registration, email confirmation, password recovery, and account lockout
- Administrative dashboard with role-based access
- Dynamic site name, slogan, colors, contact details, address, and footer content
- Torob and E-malls product feeds
- Zarinpal and IDPay payment gateway support
- Secure image uploads and sanitized rich-text product descriptions
- Rate limiting, anti-forgery protection, security headers, and health checks
- Docker volumes for SQL Server data, uploaded images, and data-protection keys
- Automated unit, smoke, integration-feed, and responsive-layout tests

## Technology Stack

- .NET 10 and ASP.NET Core MVC
- Entity Framework Core 10
- ASP.NET Core Identity
- SQL Server 2022
- Razor views, Bootstrap, and JavaScript
- Docker and Docker Compose
- xUnit

## Solution Structure

```text
MyShopClean/
├── Application/       Application services, view models, and utilities
├── Domain/            Domain models and repository contracts
├── Infra.Data/        EF Core context, repositories, and migrations
├── Infra.Ioc/         Dependency-injection and Identity configuration
├── MyShop/            ASP.NET Core MVC web application
├── Tests/             Automated security and repository tests
├── scripts/           Smoke and responsive-site audit scripts
├── compose.yaml       Local Docker environment
└── Dockerfile         Production-style web image
```

## Prerequisites

Choose one of the following options:

- Docker Desktop, for the recommended quick-start setup
- .NET SDK `10.0.302` or a compatible .NET 10 patch, plus an accessible SQL Server instance

Google Chrome and SQL Server LocalDB are also required to run the complete responsive site-audit script on Windows.

## Quick Start with Docker

Create a local environment file:

```powershell
Copy-Item .env.example .env
```

Open `.env` and replace both example passwords with strong, unique values:

```dotenv
MSSQL_SA_PASSWORD=YOUR_STRONG_DATABASE_PASSWORD
SEED_ADMIN_PASSWORD=YOUR_STRONG_ADMIN_PASSWORD
ZARINPAL_MERCHANT_ID=
IDPAY_API_KEY=
PAYMENTS_CALLBACK_BASE_URL=http://localhost:8080
```

Build and start the application:

```powershell
docker compose up --build
```

Open the storefront at:

```text
http://localhost:8080
```

The Docker environment automatically creates the database, applies migrations, creates the required roles and administrator account, and inserts demonstration products and banners.

Sign in to the administration area with:

- Username: `admin`
- Password: the value of `SEED_ADMIN_PASSWORD` in `.env`

Never publish the example passwords. The `.env` file is excluded from Git.

Stop the application:

```powershell
docker compose down
```

Remove the local containers and all Docker-managed application data:

```powershell
docker compose down --volumes
```

> Removing volumes permanently deletes the local Docker database, uploaded images, and persisted data-protection keys.

## Running Directly with .NET

Set the required configuration through environment variables:

```powershell
$env:ConnectionStrings__ConnectToDataBase = "Server=localhost,1433;Database=MyShop;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
$env:SeedAdmin__Password = "YOUR_STRONG_ADMIN_PASSWORD"
$env:Database__ApplyMigrationsOnStartup = "true"
$env:SeedDemoData__Enabled = "true"

dotnet restore .\MyShopClean.sln
dotnet run --project .\MyShop\MyShop.csproj
```

`Database__ApplyMigrationsOnStartup` and `SeedDemoData__Enabled` are intended for local development. In production, apply migrations as a separate deployment step and keep both options disabled.

## Database Migrations

Apply the migrations manually with:

```powershell
dotnet ef database update `
  --project .\Infra.Data `
  --startup-project .\MyShop
```

The production-hardening migration checks for duplicate financial and discount records before creating unique indexes. If the migration reports duplicate data, review and correct those records instead of deleting them automatically.

Older migrations on this branch produced an incompatible schema and were replaced with a clean initial migration. Use a new database for this version whenever possible. Back up any existing database before attempting an upgrade.

## Configuration

Do not store real credentials in `appsettings.json`. Use environment variables, .NET User Secrets, or your deployment platform's secret manager.

| Setting | Purpose |
|---|---|
| `ConnectionStrings__ConnectToDataBase` | SQL Server connection string |
| `SeedAdmin__Password` | Initial administrator password |
| `Database__ApplyMigrationsOnStartup` | Applies pending migrations during startup |
| `SeedDemoData__Enabled` | Inserts demonstration products and banners |
| `MailSettings__Mail` | Sender email address |
| `MailSettings__UserName` | SMTP username |
| `MailSettings__Password` | SMTP password or application password |
| `reCAPTCHA__SiteKey` | Public reCAPTCHA key |
| `reCAPTCHA__SecretKey` | Private reCAPTCHA key |
| `Payments__Zarinpal__MerchantId` | Zarinpal merchant identifier |
| `Payments__IdPay__ApiKey` | IDPay API key |
| `Payments__CallbackBaseUrl` | Public base URL used for payment callbacks |
| `DataProtection__KeysPath` | Persistent directory for data-protection keys |

The Development environment uses public reCAPTCHA test keys. Configure keys issued for your own domain before deploying to production.

## Payment Gateways

Zarinpal and IDPay settings must be provided through secure configuration:

```powershell
$env:Payments__Zarinpal__MerchantId = "YOUR_ZARINPAL_MERCHANT_ID"
$env:Payments__IdPay__ApiKey = "YOUR_IDPAY_API_KEY"
$env:Payments__CallbackBaseUrl = "https://shop.example.com"
```

Payment finalization is transactional and idempotent: stock updates, cart finalization, payment status, and invoice creation are committed together. Repeated callbacks do not create duplicate invoices or reduce stock more than once.

An IDPay credential was previously committed to the repository history. Revoke and replace that credential in the IDPay dashboard before enabling production payments. Removing it from the latest source code does not remove it from Git history.

## Dynamic Storefront Settings

Sign in as an administrator and open **Site Settings** to manage:

- Site name and slogan
- Primary, secondary, and accent colors
- Public site URL
- Support email and phone number
- Business address
- Footer title and content
- Torob and E-malls feed status and access tokens

These values are stored in the database and applied to the storefront without requiring a code change.

## Torob and E-malls Feeds

Enable the required marketplace integration in **Site Settings**, set the final public site URL, and create a random access token containing at least 32 characters.

Available endpoints:

```text
GET /integrations/torob/products
GET /integrations/emalls/products
GET /integrations/emalls/products.xml
```

Send the configured token in the request header:

```http
X-Integration-Token: YOUR_SECURE_TOKEN
```

Example:

```powershell
$headers = @{
    "X-Integration-Token" = "YOUR_SECURE_TOKEN"
}

Invoke-RestMethod `
  -Uri "https://shop.example.com/integrations/torob/products" `
  -Headers $headers
```

Feed prices are returned in tomans. Enabling an endpoint does not automatically register the store with Torob or E-malls. Complete the merchant registration process and provide the approved feed URL and access information to the relevant marketplace.

## Health Checks

The application exposes two operational endpoints:

```text
GET /health/live
GET /health/ready
```

- `/health/live` confirms that the web process is running.
- `/health/ready` also checks whether the application can access its database.

## Build and Test

Restore, build, and run the automated test suite:

```powershell
dotnet restore .\MyShopClean.sln
dotnet build .\MyShopClean.sln -c Release --no-restore
dotnet test .\MyShopClean.sln -c Release --no-build
```

Run a local application smoke test:

```powershell
& .\scripts\smoke-test.ps1
```

Run the complete site audit:

```powershell
& .\scripts\site-audit.ps1
```

The site audit:

- Opens seven public pages at mobile, tablet, and desktop viewport sizes
- Checks all 21 page and viewport combinations for horizontal overflow
- Captures screenshots in `artifacts/responsive-audit`
- Validates the Torob JSON feed
- Validates the E-malls JSON and XML feeds

Check NuGet dependencies for known vulnerabilities:

```powershell
dotnet list .\MyShopClean.sln package --vulnerable --include-transitive
```

## Production Checklist

Before deploying the application:

1. Use a dedicated production SQL Server database.
2. Replace every example password and revoke the previously exposed IDPay key.
3. Store all credentials in a managed secret store.
4. Configure production SMTP and reCAPTCHA credentials.
5. Configure the final HTTPS payment callback URL.
6. Disable automatic migrations and demonstration-data seeding.
7. Apply database migrations once during deployment.
8. Persist data-protection keys outside the application container.
9. Store uploaded images in durable storage with a backup policy.
10. Place the application behind an HTTPS reverse proxy or load balancer.
11. Monitor the readiness endpoint and application logs.
12. Run the build, tests, vulnerability scan, and site audit.

## Security Notes

- State-changing forms use anti-forgery protection.
- Cart, favorites, and invoice operations are scoped to the authenticated user.
- Marketplace access tokens are stored as hashes.
- Uploaded images are validated by size, file signature, and allowed format.
- Product HTML is sanitized before it is rendered.
- Remote Instagram media is restricted to approved HTTPS hosts and validated before storage.
- Authentication endpoints use rate limiting and account lockout.
- Passwords require at least 12 characters with uppercase, lowercase, numeric, and symbol characters.

Security fixes in the latest source do not remove secrets from older Git commits. Rotate any credential that has ever been committed.
