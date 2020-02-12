using GraphQL.Types;
using ToDoAPI.Domain.Models;

namespace API.GraphQL.Types
{
    public class TodoItemType : ObjectGraphType<TodoItem>
    {
        public TodoItemType()
        {
            Name = "TodoItemType";
            Field(ti => ti.Id);
            Field(ti => ti.Name);
            Field(ti => ti.IsComplete);
        }
    }
}