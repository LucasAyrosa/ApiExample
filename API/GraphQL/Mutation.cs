using System.Collections.Generic;
using API.GraphQL.Types;
using API.GraphQL.Types.InputTypes;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
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
                    new QueryArgument<NonNullGraphType<InputTodoItemType>>() {Name = "todoItem"},
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
                    new QueryArgument<NonNullGraphType<IdGraphType>>() {Name = "id"}
                },
                resolve: context =>
                {
                    var id = context.GetArgument<long>("id");
                    var todoItem = _todoItemRepository.FindAsync(id).GetAwaiter().GetResult();
                    if (todoItem is null) throw new ExecutionError(StatusCodes.Status404NotFound.ToString());
                    _todoItemRepository.Remove(todoItem);
                    _todoItemRepository.SaveChangesAsync().GetAwaiter();
                    return todoItem;
                }
            );

            Field<TodoItemType>("updateTodoItem",
                arguments: new QueryArguments
                {
                    new QueryArgument<NonNullGraphType<IdGraphType>>() {Name = "id"},
                    new QueryArgument<NonNullGraphType<InputTodoItemType>>() {Name = "todoItem"}
                },
                resolve: context =>
                {
                    var id = context.GetArgument<long>("id");
                    var todoItem = context.GetArgument<TodoItem>("todoItem");
                    if (id != todoItem.Id) throw new ExecutionError(StatusCodes.Status400BadRequest.ToString());
                    if (!_todoItemRepository.TodoItemExists(id)) throw new ExecutionError(StatusCodes.Status404NotFound.ToString());
                    _todoItemRepository.Update(todoItem);
                    _todoItemRepository.SaveChangesAsync().GetAwaiter();
                    return todoItem;
                }
            );
        }
    }
}