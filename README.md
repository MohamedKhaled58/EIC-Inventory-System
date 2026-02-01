# Military Industrial Complex Inventory Command System
## مجمع الصناعات الهندسية - نظام إدارة المخازن

A mission-critical inventory management platform for the Egyptian Armed Forces Engineering Industries Complex (مجمع الصناعات الهندسية للقوات المسلحة).

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Key Features](#key-features)
- [Commander's Reserve](#commanders-reserve)
- [Getting Started](#getting-started)
- [Development](#development)
- [Deployment](#deployment)
- [Documentation](#documentation)

---

## 🎯 Overview

This system provides complete material traceability from supplier to final product, hierarchical access control based on military chain of command, and specialized Commander's Reserve management for emergency stock.

### Primary Goals

1. ✅ **Complete Material Traceability**: Track every item from supplier → central warehouse → factory warehouse → project
2. ✅ **Hierarchical Access Control**: Enforce military chain of command
3. ✅ **Commander's Reserve Management**: Separate tracking and authorization for emergency stock
4. ✅ **Real-time Inventory Visibility**: Accurate stock levels across all locations
5. ✅ **Automated Workflows**: Digital requisition, approval, and transfer processes

---

## 🏗️ Architecture

The system follows **Clean Architecture** principles with **Domain-Driven Design (DDD)** and **CQRS** patterns.

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                      │
│  ┌──────────────┐         ┌──────────────┐              │
│  │   React SPA   │         │  ASP.NET API │              │
│  │  (Frontend)  │◄────────┤  (Backend)   │              │
│  └──────────────┘         └──────────────┘              │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │   Commands    │  │    Queries   │  │   Handlers   │ │
│  └──────────────┘  └──────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │   Entities    │  │Value Objects │  │   Events     │ │
│  └──────────────┘  └──────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐ │
│  │EF Core + SQL  │  │  Repositories │  │   Services   │ │
│  │    Server     │  │              │  │              │ │
│  └──────────────┘  └──────────────┘  └──────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

---

## 🛠️ Technology Stack

### Backend
- **.NET 8.0** - Framework
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 8.0** - ORM
- **SQL Server** - Database
- **JWT Authentication** - Security
- **MediatR** - CQRS pattern
- **FluentValidation** - Input validation
- **Serilog** - Logging

### Frontend
- **React 18** - UI Framework
- **TypeScript** - Type Safety
- **Material-UI (MUI)** - Component Library
- **React Router** - Routing
- **Axios** - HTTP Client
- **Vite** - Build Tool

---

## 📁 Project Structure

```
EIC Inventory System/
├── src/
│   ├── EICInventorySystem.Domain/          # Domain Layer
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   └── Interfaces/
│   │
│   ├── EICInventorySystem.Application/     # Application Layer
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Handlers/
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Interfaces/
│   │
│   ├── EICInventorySystem.Infrastructure/  # Infrastructure Layer
│   │   ├── Data/
│   │   ├── Repositories/
│   │   ├── Services/
│   │   └── BackgroundServices/
│   │
│   ├── EICInventorySystem.WebAPI/        # Presentation Layer (API)
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Filters/
│   │   └── Configuration/
│   │
│   └── EICInventorySystem.Frontend/      # Presentation Layer (UI)
│       ├── src/
│       │   ├── pages/
│       │   ├── components/
│       │   ├── services/
│       │   ├── types/
│       │   └── theme/
│       └── public/
│
├── Docs/                                 # Documentation
│   ├── The PRD.txt
│   ├── ARCHITECTURE.md
│   ├── API.md
│   └── DEPLOYMENT.md
│
└── README.md
```

---

## ✨ Key Features

### 1. Material Flow Chain
```
Suppliers → Central Warehouses → Factory Warehouses → Projects → Final Products
                                    │
                    ┌───────────────┴───────────────┐
                    │                               │
              General Stock                 Commander's Reserve ⭐
```

### 2. User Roles & Permissions

| Role | Authority | Key Permissions |
|------|-----------|-----------------|
| Complex Commander | Supreme | All factories, all data, user management |
| Factory Commander | High (Factory) | Factory operations, reserve management |
| Central Warehouse Keeper | Medium (Warehouse) | Central warehouse operations |
| Factory Warehouse Keeper | Medium (Warehouse) | Factory warehouse operations |
| Department Head | Low-Medium | Department requisitions |
| Project Manager | Medium (Project) | Project material management |
| Officer | Medium (Supervisory) | Operations supervision |
| Civil Engineer | Low-Medium (Technical) | Technical specifications |
| Worker | Minimal | Production tasks |
| Auditor | View-only | Full audit access |

### 3. Core Modules

- **Inventory Management**: Real-time stock tracking across all warehouses
- **Requisition System**: Digital material requests with approval workflows
- **Project Management**: Track material allocation and consumption per project
- **Transfer Management**: Handle material transfers between warehouses
- **Commander's Reserve**: Special emergency stock with commander-only access
- **Audit Trail**: Complete transaction history for compliance
- **Reporting & Analytics**: Comprehensive reports and dashboards
- **Notifications**: Real-time alerts for important events

---

## ⭐ Commander's Reserve

**احتياطي قائد المصنع (Commander's Reserve)** is a critical military inventory concept:

### Allocation Rules
- **General Stock**: 70-80% of received quantity
- **Commander's Reserve**: 20-30% of received quantity

### Authorization Matrix

| Action | General Stock | Commander's Reserve |
|--------|---------------|---------------------|
| View | All authorized users | All authorized users |
| Request | Department Heads, PMs | Same |
| Approve | Warehouse Keepers, Officers | **ONLY Commanders** |
| Release | Warehouse Keepers | **ONLY Commanders** |

### Use Cases
1. 🚨 Emergency Production
2. 🎯 Critical Projects
3. ⚡ Supply Chain Disruption
4. 🛡️ Strategic Stockpiling
5. 🔧 Unexpected Failures

---

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+
- SQL Server 2022+
- Git

### Backend Setup

```bash
# Navigate to backend directory
cd src/EICInventorySystem.WebAPI

# Restore dependencies
dotnet restore

# Update connection string in appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EICInventoryDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

# Run migrations
dotnet ef database update

# Start the API
dotnet run
```

The API will be available at `https://localhost:5001`

### Frontend Setup

```bash
# Navigate to frontend directory
cd src/EICInventorySystem.Frontend

# Install dependencies
npm install

# Create environment file
echo "VITE_API_BASE_URL=https://localhost:5001/api" > .env

# Start development server
npm run dev
```

The frontend will be available at `http://localhost:5173`

---

## 💻 Development

### Running Tests

```bash
# Backend tests
cd src/EICInventorySystem.Tests
dotnet test

# Frontend tests
cd src/EICInventorySystem.Frontend
npm test
```

### Code Style

- **Backend**: C# coding conventions, XML documentation
- **Frontend**: ESLint + Prettier configuration

### Branching Strategy

- `main` - Production code
- `develop` - Integration branch
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `hotfix/*` - Critical production fixes

---

## 📦 Deployment

### Backend Deployment

```bash
# Build for production
dotnet publish -c Release -o ./publish

# Deploy to IIS or Azure App Service
```

### Frontend Deployment

```bash
# Build for production
npm run build

# Deploy dist/ folder to web server
```

See [DEPLOYMENT.md](Docs/DEPLOYMENT.md) for detailed deployment instructions.

---

## 📚 Documentation

- [Product Requirements Document](Docs/The%20PRD.txt) - Complete system requirements
- [Architecture Documentation](Docs/ARCHITECTURE.md) - Technical architecture details
- [API Documentation](Docs/API.md) - REST API reference
- [Deployment Guide](Docs/DEPLOYMENT.md) - Deployment instructions

---

## 🔒 Security

- JWT-based authentication
- Role-based authorization
- Commander's Reserve special access control
- Audit trail for all transactions
- SQL injection prevention
- XSS protection
- CSRF protection

---

## 📞 Support

For technical support or questions, please contact the development team.

---

## 📄 License

Internal Use Only - Egyptian Armed Forces Engineering Industries Complex

---

**© 2025 مجمع الصناعات الهندسية للقوات المسلحة**
