using API.GraphQL.Types;
using API.GraphQL.Types.InputTypes;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Domain.Models;
using ToDoAPI.Repository.Data;

namespace API.GraphQL
{
    public class Mutation : ObjectGraphType
    {
        public Mutation(TodoItemRepository _todoItemRepository)
        {
            Field<TodoItemType>("addTodoItem",
                arguments: new QueryArguments
                {
                    new QueryArgument<InputTodoItemType>() {Name = "todoItem"},
                },
                resolve: context =>
                {
                    var todoItem = context.GetArgument<TodoItem>("todoItem");
                    _todoItemRepository.Add(todoItem);
                    _todoItemRepository.SaveChangesAsync().GetAwaiter();
                    return todoItem;
                }
            );

            Field<TodoItemType>("removeTodoItem",
                arguments: new QueryArguments
                {
                    new QueryArgument<StringGraphType>() {Name = "id"}
                },
                resolve: context =>
                {
                    var id = context.GetArgument<long>("id");
                    var todoItem = _todoItemRepository.FindAsync(id).GetAwaiter().GetResult();
                    _todoItemRepository.Remove(todoItem);
                    _todoItemRepository.SaveChangesAsync().GetAwaiter();
                    return todoItem;
                }
            );

            Field<TodoItemType>("updateTodoItem",
                arguments: new QueryArguments
                {
                    new QueryArgument<IntGraphType>() {Name = "id"},
                    new QueryArgument<InputTodoItemType>() {Name = "todoItem"}
                },
                resolve: context =>
                {
                    var id = context.GetArgument<long>("id");
                    var todoItem = context.GetArgument<TodoItem>("todoItem");
                    if (id != todoItem.Id) return "BadRequest";
                    if (!_todoItemRepository.TodoItemExists(id)) return "NotFound";
                    _todoItemRepository.Update(todoItem);
                    _todoItemRepository.SaveChangesAsync().GetAwaiter();
                    return todoItem;
                }
            );
        }
    }
}