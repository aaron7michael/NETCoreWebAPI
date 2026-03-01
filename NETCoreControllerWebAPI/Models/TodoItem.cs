using System.ComponentModel.DataAnnotations;

namespace NETCoreControllerWebAPI.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        [Range(1, int.MaxValue)]
        public int Priority { get; set; }
        public bool IsComplete { get; set; }
    }

    public record TodoItemDTO(string Name, int? Priority, bool IsComplete = false);
    public record TodoItemPatchDTO(string? Name, int? Priority, bool? IsComplete);
}
