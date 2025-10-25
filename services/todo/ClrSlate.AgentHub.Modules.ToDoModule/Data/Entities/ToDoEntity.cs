using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace ClrSlate.AgentHub.Modules.ToDoModule.Models;

public class ToDoEntity : FullAuditedAggregateRoot<string>
{
    [Required]
    public string Title { get; set; } = default!;

    public bool IsComplete { get; set; }
}
