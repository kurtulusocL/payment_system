# 💳 PaymentSystem — Modular Payment & Finance Management Platform

**PaymentSystem** is an enterprise-grade payment platform based on .NET 9.0, designed to meet the high security, scalability, and real-time transaction requirements of the modern FinTech ecosystem.

## 🚀 Key Features

* **Modern Development Process:** The project architecture and complex business logic were optimized using *agentic workflow* principles supported by **Qwen 2.5/3.5** models, ensuring superior code quality and long-term maintainability.
* **Advanced API & UI:** A comprehensive management panel and user dashboard featuring 16 API Controllers (190+ endpoints) and 120+ Razor Views.
* **Real-Time Communication:** Powered by 3 different SignalR hubs (Payment, Wallet, Transaction), ensuring all financial movements are updated instantly across all interfaces.
* **Dual Database & Fallback:** A resilient data strategy that automatically switches to **Azure SQL Server** if the local SQL Server instance fails.

## 🔒 Security & Protection

A multi-layered defense line has been established to protect financial data:

* **IDOR & URL Manipulation Prevention:** Strict policy-based authorization controls are implemented for all resource access. Attacks via query strings or URL parameters are blocked by custom middleware layers.
* **AES-GCM 256-bit Encryption:** Sensitive fields such as phone numbers, emails, and card details are stored encrypted in the database.
* **XSS Protection:** All user inputs are sanitized using `HtmlSanitizer`.
* **Multi-Layered Authentication:** Support for JWT, Cookie, API Key, and Google OAuth.

## 🏗 Architecture (Clean Architecture)

The project consists of 6 main layers with inward-pointing dependencies:

1.  **Domain:** Entities, Value Objects, and core business rules (Zero dependencies).
2.  **Application:** Service interfaces, DTOs, FluentValidation, and SignalR hub definitions.
3.  **Infrastructure:** SQL Server (EF Core), Redis, RabbitMQ, and Azure integrations.
4.  **Shared:** `Result<T>` pattern, common DTOs, and View Models.
5.  **API:** RESTful service layer with full Swagger documentation.
6.  **WebUI:** ASP.NET Core MVC-based user and admin panels.

## 🧪 Testing & Quality Assurance

The project features a comprehensive test suite, proving high code coverage and reliability:

* **Total Test Status:** ✅ **310 Passed** (0 Failed / 0 Skipped).
* **MoqTests (~210):** Validation of controller and service logic using mock objects.
* **IntegrationTests (~50):** Integration tests performed across 15 different entities using EF Core InMemory.
* **UnitTests (~50):** Validation of DTOs and Result pattern logic.

## 🛠 Technology Stack

| Domain | Technology |
| :--- | :--- |
| **Backend** | .NET 9.0, EF Core 9.0, C# 13 |
| **Database** | MSSQL Server (Local + Azure Fallback) |
| **Caching** | Redis & MemoryCache (with Fallback support) |
| **Messaging** | RabbitMQ (Reliable delivery via Outbox Pattern) |
| **Frontend** | Bootstrap, jQuery, DataTables.js, Font Awesome |

## 📦 Installation & Setup

### Running with Docker (Quick Start)
The project consists of 6 services, including API, WebUI, SQL Server, Redis, and RabbitMQ.

```bash
# Clone the project
git clone [https://github.com/kurtulusocal/PaymentSystem.git](https://github.com/kurtulusocal/PaymentSystem.git)

# Spin up the Docker containers
docker-compose up -d --build

Manual Installation (.NET CLI)
Update the connection strings in the appsettings.json file.

Install dependencies and update the database:

Bash
dotnet restore
dotnet ef database update --project PaymentSystem.Infrastructure
Run the project and tests:

Bash
dotnet test
dotnet run --project PaymentSystem.Api

✅ Status: Production-Ready
✅ Security: IDOR, XSS & URL Manipulation Protected
✅ Test Status: 310 Passed / 0 Failed