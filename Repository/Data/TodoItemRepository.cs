using System.Linq;
using ToDoAPI.Domain.Models;

namespace ToDoAPI.Repository.Data
{
    public class TodoItemRepository : BaseRepository<TodoItem>
    {
        public TodoItemRepository(TodoContext context) : base(context)
        {
        }

        public bool TodoItemExists(long id)
        {
            return All.Any(e => e.Id == id);
        }
    }
}