using GraphQL.Types;
using ToDoAPI.Domain.Models;

namespace API.GraphQL.Types
{
    public class TodoItemType : ObjectGraphType<TodoItem>
    {
        public TodoItemType()
        {
            Name = "TodoItem";
            Field(ti => ti.Id);
            Field(ti => ti.Name);
            Field(ti => ti.IsComplete);
        }
    }
}