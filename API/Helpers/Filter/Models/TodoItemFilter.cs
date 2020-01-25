using ToDoAPI.API.Helpers.Pagination;
using ToDoAPI.API.Helpers.Sort;

namespace ToDoAPI.API.Helpers.Filter.Models
{
    public class TodoItemFilter : ISort, IPagination
    {
        public string Name { get; set; }
        public bool? IsComplete { get; set; }
        public string SortBy { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
    }
}