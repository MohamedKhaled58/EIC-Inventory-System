<!--
Sync Impact Report
==================
- Version change: 0.0.0 → 1.0.0 (MAJOR - Initial constitution adoption)
- Added principles:
  - I. Clean Architecture & Domain-Driven Design
  - II. Security-First Development
  - III. Complete Material Traceability
  - IV. Audit Trail & Compliance
  - V. Military Chain of Command Authorization
- Added sections:
  - Technology Standards
  - Development Workflow
  - Governance
- Templates requiring updates:
  - .specify/templates/plan-template.md ✅ (already aligned - has Constitution Check section)
  - .specify/templates/spec-template.md ✅ (already aligned - has mandatory sections)
  - .specify/templates/tasks-template.md ✅ (already aligned - supports phased approach)
- Deferred items: None
-->

# EIC Inventory System Constitution

## Core Principles

### I. Clean Architecture & Domain-Driven Design

The system MUST follow Clean Architecture principles with Domain-Driven Design (DDD) and CQRS patterns:

- **Layer Separation**: Presentation, Application, Domain, and Infrastructure layers MUST remain strictly separated
- **Domain Independence**: Domain entities MUST NOT depend on external frameworks or infrastructure concerns
- **Dependency Rule**: Dependencies MUST point inward - outer layers depend on inner layers, never the reverse
- **CQRS Pattern**: Commands (writes) and Queries (reads) MUST be handled through separate paths via MediatR
- **Repository Pattern**: Data access MUST be abstracted through repository interfaces defined in the Domain/Application layer

**Rationale**: Military systems require long-term maintainability, testability, and the ability to swap infrastructure components without affecting business logic.

### II. Security-First Development

All development MUST prioritize security as a non-negotiable constraint:

- **Authentication**: All API endpoints (except explicit public routes) MUST require JWT authentication
- **Authorization**: Every operation MUST verify user permissions against role-based access control
- **Commander's Reserve**: Access to emergency stock operations MUST be restricted to Commander roles only
- **Input Validation**: All user input MUST be validated using FluentValidation before processing
- **OWASP Compliance**: Code MUST NOT introduce OWASP Top 10 vulnerabilities (SQL injection, XSS, CSRF, etc.)
- **Secrets Management**: Credentials, connection strings, and API keys MUST NOT be committed to version control

**Rationale**: This is a mission-critical military inventory system. Security breaches could compromise national defense capabilities.

### III. Complete Material Traceability

Every inventory operation MUST maintain complete material traceability:

- **Chain of Custody**: Material movements from Supplier → Central Warehouse → Factory Warehouse → Project MUST be recorded
- **Quantity Tracking**: Stock levels MUST be accurate and updated in real-time
- **Reserve Allocation**: Incoming materials MUST be split between General Stock (70-80%) and Commander's Reserve (20-30%)
- **Location Tracking**: Every item MUST be associated with exactly one warehouse/location at any time
- **Transaction Linking**: Every stock change MUST reference the triggering transaction (requisition, transfer, adjustment)

**Rationale**: Military logistics require accountability for every item. Lost materials could indicate theft, misallocation, or supply chain compromise.

### IV. Audit Trail & Compliance

All transactions MUST be fully auditable:

- **Immutable History**: Transaction records MUST NOT be deleted or modified after creation
- **User Attribution**: Every operation MUST record the authenticated user who performed it
- **Timestamp Recording**: All records MUST include accurate creation and modification timestamps
- **Change Logging**: Sensitive operations MUST log before/after values via Serilog
- **Approval Chains**: Requisitions and transfers MUST record all approval/rejection decisions with approver identity

**Rationale**: Egyptian Armed Forces compliance requirements mandate complete audit trails for inventory transactions.

### V. Military Chain of Command Authorization

Access control MUST reflect the military organizational hierarchy:

- **Role Hierarchy**: Complex Commander > Factory Commander > Officers > Warehouse Keepers > Department Heads > Workers
- **Scope Boundaries**: Users MUST only access data within their assigned factory/warehouse/department
- **Escalation Paths**: Approval workflows MUST route to appropriate authority level based on operation type
- **Commander Authority**: Factory Commanders MUST have final authority over their factory's Commander's Reserve
- **Auditor Access**: Auditors MUST have read-only access to all data without modification capability

**Rationale**: Military operations require strict adherence to chain of command for accountability and operational security.

## Technology Standards

The following technology choices are mandated for consistency and maintainability:

### Backend Stack
- **.NET 8.0**: Primary framework - all backend code MUST target this version
- **ASP.NET Core Web API**: REST API implementation
- **Entity Framework Core 8.0**: ORM for data access - direct SQL MUST be avoided except for performance-critical queries
- **SQL Server 2022+**: Primary database - schema changes MUST use EF Core migrations
- **MediatR**: CQRS implementation - all commands/queries MUST flow through MediatR handlers
- **FluentValidation**: Input validation - all DTOs MUST have corresponding validators
- **Serilog**: Structured logging - all significant operations MUST be logged

### Frontend Stack
- **React 18 + TypeScript**: UI implementation - JavaScript MUST NOT be used for new code
- **Material-UI (MUI)**: Component library - custom styling MUST follow MUI theming patterns
- **Axios**: HTTP client - fetch API MUST NOT be used for consistency
- **Vite**: Build tooling

### Testing Requirements
- **xUnit**: Backend unit testing framework
- **Integration Tests**: API endpoints MUST have integration test coverage
- **Frontend Tests**: Critical user flows SHOULD have component test coverage

## Development Workflow

### Code Review Requirements
- All changes MUST be submitted via pull request
- PRs MUST pass automated checks before merge
- Security-sensitive changes MUST be reviewed by a senior developer

### Branching Strategy
- `main`: Production-ready code only
- `develop`: Integration branch for feature work
- `feature/*`: New feature development
- `bugfix/*`: Bug fixes
- `hotfix/*`: Critical production fixes

### Quality Gates
- Code MUST compile without warnings
- All existing tests MUST pass
- New features MUST include appropriate test coverage
- API changes MUST update OpenAPI/Swagger documentation

## Governance

This constitution supersedes all other development practices for the EIC Inventory System.

### Amendment Procedure
1. Proposed changes MUST be documented with rationale
2. Changes affecting security or architecture REQUIRE senior developer approval
3. All amendments MUST increment the constitution version appropriately
4. Dependent templates MUST be updated to reflect constitutional changes

### Versioning Policy
- **MAJOR**: Principle removal, redefinition, or backward-incompatible governance changes
- **MINOR**: New principles added or existing guidance materially expanded
- **PATCH**: Clarifications, wording improvements, typo fixes

### Compliance Review
- All PRs SHOULD verify alignment with constitutional principles
- Architecture decisions MUST be justified against constitution requirements
- Violations MUST be explicitly documented with justification in Complexity Tracking

**Version**: 1.0.0 | **Ratified**: 2026-02-02 | **Last Amended**: 2026-02-02
