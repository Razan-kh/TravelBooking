# Travel & Accommodation Booking Platform – REST API

A scalable and secure hotel booking platform built with ASP.NET Core, following Clean Architecture and Domain-Driven Design (DDD) principles.
This API provides a complete backend solution for authentication, hotel discovery, booking management, secure checkout, admin operations.

## 🌐 Overview

This API powers the core features of the platform, including:

- **Authentication & Users**: Login, registration, and role-based access control.
- **Hotel Discovery**: Home page previews, search, and filtering.
- **Hotel Details**: Room availability and reviews.
- **Booking Workflow**: Secure, concurrency-safe booking with transactions.
- **Admin Operations**: Full CRUD for hotels, rooms, and users.
Designed for clean, maintainable, and testable code, with strong separation of concerns and consistent error handling.

## ✨ Key Features

### User & Authentication
- JWT-based authentication
- Role-based authorization
- 
### Hotel & Booking System
- Advanced search and filtering
- Room availability checks
- Concurrency-safe bookings

### Admin Tools
- CRUD for hotels, rooms, and cities
- Image uploads via Cloudinary

### Notifications
- Booking confirmation emails (SMTP)
- PDF invoice generation using QuestPDF

### Observability & Quality
- Structured logging with Serilog
- Elasticsearch integration
- Global filtering, sorting, and pagination with Sieve
- Consistent result pattern for predictable error handling

## 🏛 Architecture

The system is structured into four independent layers:

- **Domain** – Entities, value objects, and business rules.
- **Application** – Use cases, validators, DTOs, and domain services.
- **Infrastructure** – EF Core, database persistence, and external integrations (Email, Cloudinary, Logging).
- **WebAPI** – Controllers, endpoints, authentication, and request pipeline.

Each domain is clearly separated, making the codebase extensible and maintainable.

## 🧰 Tech Stack

- ASP.NET Core 9 (Web API)
- Entity Framework Core 9 + SQL Server
- FluentValidation
- Mapperly
- Serilog
- QuestPDF
- Sieve (filtering, sorting, pagination)
- Cloudinary (image management)
- xUnit, Moq, AutoFixture, FluentAssertions (testing)

## ⚙️ Getting Started

### Clone the repository
```bash
git clone https://github.com/your-repo/TravelBookingPlatform.git
cd TravelBookingPlatform
