# Quickstart: Agents CRUD API

## Create an Agent
```http
POST /api/agents
Content-Type: application/json

{
  "name": "support-bot",
  "displayName": "Support Bot",
  "model": "openai:gpt-4o",
  "instructions": "Act as a helpful support assistant for billing questions."
}
```
201 Created
Location: /api/agents/support-bot

## Get Agent
```http
GET /api/agents/support-bot
```
200 OK
```json
{
  "name": "support-bot",
  "displayName": "Support Bot",
  "model": "openai:gpt-4o",
  "description": null,
  "instructions": "Act as a helpful support assistant for billing questions.",
  "version": 1,
  "updatedAtUtc": "2025-01-01T12:00:00Z"
}
```

## Update Agent (Optimistic Concurrency)
```http
PUT /api/agents/support-bot
If-Match: 1
Content-Type: application/json

{
  "displayName": "Support Bot",
  "model": "openai:gpt-4o-mini",
  "description": "Billing-focused assistant",
  "instructions": "Answer billing questions precisely.",
  "version": 1
}
```
Possible responses:
- 200 OK (body with version=2)
- 409 Conflict (stale If-Match / version mismatch)

## List Agents
```http
GET /api/agents?skip=0&take=20
```
200 OK
```json
{
  "items": [ { "name": "support-bot", "displayName": "Support Bot", "model": "openai:gpt-4o", "updatedAtUtc": "2025-01-01T12:01:00Z" } ],
  "total": 1
}
```

Include deleted:
```http
GET /api/agents?includeDeleted=true
```

## Delete (Soft)
```http
DELETE /api/agents/support-bot
If-Match: 2
```
204 No Content

Subsequent GET (without includeDeleted) → 404

## Validation Constraints
| Field | Rule |
|-------|------|
| name | slug ^[a-z0-9]+(?:-[a-z0-9]+)*$, ≤64 |
| instructions | ≤16000 chars |
| version | required on update/delete |

## Error Model (Example)
```json
{
  "code": "ConcurrencyConflict",
  "message": "Version mismatch (expected 2)",
  "details": { "currentVersion": 3 }
}
```

## Local Dev Checklist
- Ensure Mongo running
- Apply unique index on `Name`
- Start API project; verify `/swagger` or served OpenAPI
- Run validator tests

## Next Steps (Beyond v1)
- Restore endpoint
- Ownership / Auth integration
- Search & tagging
