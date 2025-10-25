<!--
Sync Impact Report
Version: 0.0.0 -> 1.0.0
Modified Principles: (placeholders -> concrete definitions)
Added Sections: Engineering Workflow & Quality Gates; Development Workflow & Review Process
Removed Sections: none
Templates Updated:
 - .specify/memory/constitution.md ✅
 - .specify/templates/plan-template.md ✅
 - .specify/templates/spec-template.md ✅ (no changes required)
 - .specify/templates/tasks-template.md ✅
Deferred TODOs: none
-->

# Agents Hub Constitution

## Core Principles

### 1. Modular Boundary Integrity
All runtime code MUST be organized as explicit ABP / module-centric units with clear public
contracts (service interfaces, DTOs, events). A module MUST NOT: (a) reach across another
module's internal namespace, (b) duplicate domain logic already owned elsewhere, or (c)
introduce circular dependencies. Each module MUST build, test, and (where practical) run
in isolation using its own DI composition root. Cross-module communication MUST occur only
through versioned contracts (C# interfaces, HTTP/REST endpoints, or integration events).
Rationale: Enforces evolvable architecture, enables parallel work, and reduces change blast
radius.

### 2. Contract-Driven & Versioned APIs
Every externally consumed interface (public C# API surface, HTTP endpoint, message contract)
MUST declare an explicit semantic version. Breaking changes require a MAJOR bump and MUST
coexist with the previous contract for at least one release cycle or provide an automated
migration path. Request/response DTOs MUST be immutable (init-only setters) and backward
compatible fields MAY ONLY be added (never repurposed). Contracts MUST be documented via
generated OpenAPI (for HTTP) or markdown schema docs (for messages) committed to the repo.
Rationale: Prevents accidental breakage and enables safe consumer upgrades.

### 3. Test-First Fast Feedback
For new logic, at least one failing automated test (unit, contract, or integration) MUST
exist before implementation. The hierarchy: (a) Unit tests cover pure logic; (b) Contract
tests validate external surface behaviors; (c) Integration tests cover cross-module
composition and persistence concerns; (d) End-to-end smoke path executed in CI. Tests MUST
execute in <5 minutes total for a clean repo; any suite exceeding 60s SHOULD be profiled.
Flaky tests MUST be quarantined within 24h. Rationale: Ensures correctness and accelerates
iteration without regressions.

### 4. Observability & Operational Transparency
All services MUST emit structured logs (JSON), distributed traces (OpenTelemetry), and
domain metrics (counters/timers) for critical paths (authentication, job dispatch, agent
execution). Correlation IDs MUST propagate end-to-end (traceparent). Error logs MUST include
root cause classification (user, transient, bug). Production incidents MUST be reconstructable
from telemetry within 15 minutes. Rationale: Enables rapid diagnosis and continuous
improvement.

### 5. Secure-by-Default & Least Privilege
All external inputs MUST be validated (length, type, format). Secrets MUST NOT be committed
and MUST be sourced from secure configuration providers. Each module/service MUST operate
with the minimum required permissions (DB schemas, queues, storage). Authentication &
authorization MUST wrap every externally reachable endpoint; unauthorized access attempts
MUST be logged with no sensitive data leaked. Dependencies MUST be scanned weekly; critical
CVEs patched within 72 hours. Rationale: Minimizes attack surface and risk propagation.

## Engineering Workflow & Quality Gates

The following non-functional gates are mandatory across the repository:
1. Language & Runtime: .NET 9, C# 13 (when GA) targeting LTS where applicable.
2. Dependency Hygiene: No unused package references; transitive vulnerabilities blocked.
3. Build Reproducibility: Deterministic builds (no network fetch during compile outside
	declared NuGet feeds). CI MUST fail on warnings treated as errors (TreatWarningsAsErrors).
4. Documentation: Every module exposes a README with purpose, public contracts, and
	example usage. Public HTTP endpoints auto-documented via OpenAPI and published artifact.
5. Performance Baseline: Each service MUST define at least one measurable SLO (latency or
	throughput) in its spec before production exposure. Placeholder SLOs MUST NOT persist past
	first release candidate.
6. Observability Implementation: Logs, metrics, traces verified in staging before prod gate.
7. Security Checklist: Threat model documented for new externally exposed modules.
8. Change Size: A single PR SHOULD NOT exceed 500 added lines unless justified in the PR
	description referencing principle rationale.

## Development Workflow & Review Process

Phases (automated where possible): Spec → Plan → Research → Data Model → Quickstart →
Contracts → Tasks → Implementation → Release Notes.

Pull Request Checklist (all MUST be satisfied unless explicitly waived with rationale):
- Principles Referenced: PR description MUST state which principles are impacted.
- Tests: New logic has failing test first commit OR justification for exclusion (rare).
- Contract Diff: Any contract change includes version bump & backward compatibility note.
- Telemetry: New critical path includes at least one metric + trace span + structured log.
- Security: Input validation paths & auth policies present for new endpoints.
- Docs: Updated module README/CHANGELOG when public behavior changes.

CI Quality Gates:
- Fast Test Suite (<5m) MUST pass.
- Static analysis (nullable reference types enabled, analyzers) MUST have zero errors.
- Formatting enforced (dotnet format) prior to merge.

Release Process:
1. Merge to main triggers version tagging (Git tag vX.Y.Z).
2. Release notes auto-generated from conventional commits & augmented manually for breaking changes.
3. Staging soak validation (telemetry coverage + smoke test) required before prod promotion.

## Governance

Authority: This constitution supersedes ad-hoc conventions. Conflicts resolve in favor of
these principles. Proposed deviations MUST include an explicit rollback plan.

Amendments:
- Initiated via PR modifying this file + Sync Impact Report.
- Version Bump Classification:
  * MAJOR: Removal or incompatible redefinition of a principle or governance rule.
  * MINOR: Addition of a new principle, new required gate, or material expansion.
  * PATCH: Clarifications, grammar, non-normative examples.
- Approval: Requires ≥2 maintainers + one uninvolved reviewer. Blocking concerns MUST cite
  specific violated principle or risk.

Compliance & Review:
- Quarterly architectural review ensures module boundaries and telemetry completeness.
- Random PR sampling (≥5% monthly) for adherence metrics; repeated violations escalate.
- Security incident postmortems MUST map findings to constitution adjustments (if needed).

Exception Handling:
- Exceptions are time-boxed (expiry date) and tracked in an Exceptions Register (future
  automation). Expired exceptions without renewal are invalid.

Enforcement:
- CI gates enforce measurable items. Reviewers enforce qualitative items (clarity, rationale).
- Non-compliant merges trigger retro review; systemic gaps result in constitution update.

**Version**: 1.0.0 | **Ratified**: 2025-10-25 | **Last Amended**: 2025-10-25
