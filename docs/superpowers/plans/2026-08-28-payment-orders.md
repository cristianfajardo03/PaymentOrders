# Payment Orders Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use `superpowers:executing-plans` to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a compilable .NET 8 Clean Architecture payment-order API skeleton with three intentionally unfinished creational-pattern exercises.

**Architecture:** Domain owns the aggregate, concrete order types, pending pattern interfaces, and invariants. Application owns use cases and ports. Infrastructure implements EF Core persistence and idempotency; SQLite is the default and MySQL is an optional provider with a separate migration assembly. Api owns HTTP, Swagger, and composition.

**Tech Stack:** C#/.NET 8, ASP.NET Core, EF Core 8, SQLite provider, Swagger, xUnit, and NetArchTest.

**Spec:** `docs/superpowers/specs/2026-08-28-payment-orders-design.md`

## Global Constraints

- Target `net8.0`; do not introduce an ORM or web dependency into Domain.
- The Factory, Builder, commission strategies, and commission-strategy factory remain explicitly unfinished.
- Entity concrete constructors are `protected internal`; Application and Api only use pattern interfaces.
- Controllers contain no order-type decision logic.
- Use a versioned EF Core migration for the local SQLite schema; the activity does not require a database service.

---

### Task 1: Bootstrap solution and dependency graph

**Files:** Create the solution, four production projects, three test projects, central build/package files, and project references.

- [ ] Create the .NET 8 solution and projects under `src/` and `tests/`.
- [ ] Add references so dependencies are `Api -> Application -> Domain` and `Api -> Infrastructure -> Application, Domain`.
- [ ] Add central package versions for EF Core, SQLite, Pomelo MySQL, Swagger, xUnit, and NetArchTest.
- [ ] Run `dotnet restore PaymentOrders.sln` and correct package/reference errors.

### Task 2: Test the domain boundaries before implementation

**Files:** Create Domain unit tests and architecture tests under `tests/`.

- [ ] Add failing tests for amount, account, SWIFT, scheduled-date, and state transition invariants.
- [ ] Add failing architecture tests proving Domain does not depend on Api, Infrastructure, EF Core, or SQLite.
- [ ] Run the narrow test projects and confirm the expected red state.
- [ ] Implement the minimum framework-free domain model that makes the invariant and architecture tests green.

### Task 3: Add the learning seams and application use cases

**Files:** Create Domain pattern contracts/classes and Application DTOs, ports, services, and use cases.

- [ ] Define Factory, Builder, Strategy, and Strategy Factory signatures without algorithms.
- [ ] Add comments explaining each student responsibility and throw `NotImplementedException` in each designated member.
- [ ] Implement create, read, list, cancel, process, and event-query orchestration using only these abstractions.
- [ ] Add the public, clearly marked failing contract samples for the unfinished patterns.

### Task 4: Add EF Core and API wiring

**Files:** Create `PaymentOrdersDbContext`, configurations, repository/idempotency adapters, controllers, and local configuration.

- [ ] Map TPH orders, their append-only audit events, and unique idempotency records using Fluent API.
- [ ] Register SQLite, repositories, factories, Swagger, and controllers in the Api composition root.
- [ ] Implement routes for create, get, list, cancel, process, and events without type branching.
- [ ] Generate and apply an initial EF Core migration for the local SQLite schema.

### Task 5: Document and verify the deliverable

**Files:** Create `README.md`, `AGENTS.md`, and operational documentation.

- [ ] Document local startup, tests, student-owned files, and the oral-defense prompts.
- [ ] Add AI-agent guardrails preventing completion or test tampering.
- [ ] Run restore, build, and every test suite; report intentionally failing contract tests separately from implementation defects.
- [ ] Search the repository for `NotImplementedException` and compare it to the approved pedagogical list.
