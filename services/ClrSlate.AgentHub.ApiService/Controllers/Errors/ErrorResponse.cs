namespace ClrSlate.AgentHub.ApiService.Controllers.Errors;

public sealed record ErrorResponse(string Code, string Message, object? Details = null);
