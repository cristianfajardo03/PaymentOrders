# Payment Orders Design

## Purpose

Provide a .NET 8 academic API skeleton where students implement Factory, Builder, and Strategy plus Factory while the surrounding Clean Architecture, database wiring, state rules, and delivery API are already operational.

## Architecture

The solution has four production projects: `PaymentOrders.Api` depends on `Application` and `Infrastructure`; `Infrastructure` depends on `Application` and `Domain`; `Application` depends on `Domain`; `Domain` has no project or framework dependencies. Dependency inversion is expressed through repository and idempotency ports owned by Application. The API composition root is the only place that registers Infrastructure implementations.

`PaymentOrder` is the aggregate root. It owns its immutable audit events and enforces amount, account, SWIFT, scheduled-date, and state-transition invariants. The concrete National, International, and Scheduled types are only instantiable from the Domain assembly through the Builder supplied by the Factory.

## Pedagogical boundary

Only these implementations deliberately throw `NotImplementedException`: `PaymentOrderFactory.CreateBuilder`, every fluent member plus `Build` in `PaymentOrderBuilder`, each commission strategy calculation, and `CommissionStrategyFactory.GetFor`. Their XML or inline comments explain responsibilities without algorithms, code hints, or pseudocode. Application calls their interfaces exclusively.

## Persistence and delivery

EF Core maps a TPH payment-order hierarchy, immutable audit events, and idempotency records to a local SQLite file by default. An optional MySQL provider can be selected through configuration, with its own versioned EF Core migration assembly. Running the default API requires no container or external database. A unique idempotency key permits a repeated create request to return its original order identifier. The REST controller contains no type branching: it delegates use cases and maps their DTOs. Swagger is exposed in every environment.

## Verification

Architecture tests make forbidden outward Domain dependencies fail automatically. Domain tests cover business invariants and allowed/forbidden state changes. Contract samples are intentionally failing until students implement the three patterns and include the required warning comment. Build and test commands are documented in the README.

## Scope decisions

- Solution name: `PaymentOrders`.
- Currency is an uppercase ISO-like three-character code.
- The universal order amount maximum is 1,000,000.00.
- API writes use UTC timestamps supplied through an Application clock abstraction.
- A versioned initial EF Core migration is kept in Infrastructure for SQLite, and a separate migration assembly is kept for MySQL.
