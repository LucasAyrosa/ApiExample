using GraphQL.Types;
using ToDoAPI.Domain.Models;

namespace API.GraphQL.Types.InputTypes
{
    public class InputTodoItemType : InputObjectGraphType<TodoItem>
    {
        public InputTodoItemType()
        {
            Name = "InputTodoItem";
            Field(ti => ti.Id, true);
            Field(ti => ti.Name, false);
            Field(ti => ti.IsComplete, true);
        }
    }
}