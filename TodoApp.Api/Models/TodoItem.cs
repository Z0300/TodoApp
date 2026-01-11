using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.Models;

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(100)]
    public string Title { get; set; } = default!;
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
