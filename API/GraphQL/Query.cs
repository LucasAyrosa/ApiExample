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
                var query = _todoItemRepository.All;

                var id = context.GetArgument<long?>("id");
                if (id.HasValue) query = query.Where(ti => ti.Id == id);

                var name = context.GetArgument<string>("name");
                if (!string.IsNullOrEmpty(name)) query = query.Where(ti => ti.Name.ToUpper().Contains(name.ToUpper()));

                var isComplete = context.GetArgument<bool?>("isComplete");
                if (isComplete.HasValue) query = query.Where(ti => ti.IsComplete == isComplete);

                return query.ToList();
            });
        }
    }
}