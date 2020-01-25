using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Domain.Models
{
    public class TodoItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}