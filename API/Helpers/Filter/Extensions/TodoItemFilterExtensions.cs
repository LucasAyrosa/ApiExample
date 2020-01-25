using System.Linq;
using ToDoAPI.API.Helpers.Filter.Models;
using ToDoAPI.API.Helpers.Pagination;
using ToDoAPI.API.Helpers.Sort;
using ToDoAPI.Domain.Models;

namespace ToDoAPI.API.Helpers.Filter.Extensions
{
    public static class TodoItemFilterExtensions
    {
        public static IQueryable<TodoItem> Filter(this IQueryable<TodoItem> query, TodoItemFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Name)) query = query.Where(ti => ti.Name.ToLower().Contains(filter.Name.ToLower()));
            if (filter.IsComplete.HasValue) query = query.Where(ti => ti.IsComplete == filter.IsComplete);
            return query;
        }

        public static IQueryable<TodoItem> Apply(this IQueryable<TodoItem> query, TodoItemFilter filter)
        {
            return query.Filter(filter).Sort(filter).Paginate(filter);
        }
    }
}