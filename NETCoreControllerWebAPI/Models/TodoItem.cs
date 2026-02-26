using System.ComponentModel.DataAnnotations;

namespace NETCoreControllerWebAPI.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool IsComplete { get; set; }
    }

    public record TodoItemDTO(string Name, int? Priority, bool IsComplete = false);
}
