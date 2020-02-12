using System.Linq;
using API.GraphQL.Types;
using GraphQL.Types;
using ToDoAPI.Repository.Data;

namespace API.GraphQL
{
    public class Query : ObjectGraphType
    {
        public Query(TodoItemRepository _todoItemRepository)
        {
            Field<ListGraphType<TodoItemType>>("todoItems",
            arguments: new QueryArguments
            {
                new QueryArgument<StringGraphType> {Name = "id"},
                new QueryArgument<StringGraphType> {Name = "name"},
                new QueryArgument<StringGraphType> {Name = "isComplete"}
            },
            resolve: context =>
            {
                var id = context.GetArgument<long?>("id");
                var name = context.GetArgument<string>("name");
                var isComplete = context.GetArgument<bool?>("isComplete");

                var query = _todoItemRepository.All;
                if (id != null) query = query.Where(ti => ti.Id == id);
                if (name != null) query = query.Where(ti => ti.Name.ToUpper().Contains(name.ToUpper()));
                if (isComplete != null) query = query.Where(ti => ti.IsComplete == isComplete);
                return query.ToList();
            });
        }
    }
}