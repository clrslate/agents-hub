using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;

namespace ClrSlate.AgentHub.Modules.ToDoModule.Models;

public class TodoItem: ExtensibleObject
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = default!;
    public bool IsComplete { get; set; }
}
