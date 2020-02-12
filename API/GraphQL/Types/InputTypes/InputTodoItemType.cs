using GraphQL.Types;
using ToDoAPI.Domain.Models;

namespace API.GraphQL.Types.InputTypes
{
    public class InputTodoItemType : InputObjectGraphType<TodoItem>
    {
        public InputTodoItemType()
        {
            Name = "InputTodoItemType";
            Field(ti => ti.Id);
            Field(ti => ti.Name);
            Field(ti => ti.IsComplete);
        }
    }
}