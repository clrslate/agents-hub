# Temporary Exception: Agents CRUD Auth Bypass

**Related Feature**: 002-agents-crud
**Constitution Principle Violated**: Principle 5 - Secure-by-Default & Least Privilege
**Endpoints Affected**: /api/agents (all CRUD operations)

## Rationale
Accelerate internal iteration and UX validation before shared auth module integration is available. Environment is non-production, access controlled at network boundary.

## Mitigations
- No PII stored; only agent configuration metadata.
- Structured logs include `unauthenticated=true` field for every request.
- OpenTelemetry spans tagged `security.exception=temporary-auth-bypass`.
- Metrics counters segregated with label `auth="none"` for later auditing.

## Expiry
Must be removed or replaced with real authentication by: 2025-12-01.
If not remediated by expiry date, feature is blocked from production promotion.

## Remediation Plan
1. Introduce minimal auth handler (API key or token) mapped to future identity provider.
2. Remove `unauthenticated` log and span tags.
3. Update OpenAPI with security scheme + mark exception file obsolete.
4. Delete this exception file upon completion.

## Owner
Agents CRUD feature owner (to be assigned) is responsible for tracking.

## Approval
Pending reviewer sign-off (add approvals below):
- [ ] Maintainer 1
- [ ] Maintainer 2

## Change Log
- 2025-10-25: Initial exception creation.
