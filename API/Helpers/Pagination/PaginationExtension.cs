using System.Linq;

namespace ToDoAPI.API.Helpers.Pagination
{
    public static class PaginationExtension
    {
        public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> query, IPagination pagination)
        {
            if (pagination.Limit <= 0) pagination.Limit = 10;
            if (pagination.Page <= 0) pagination.Page = 1;
            return query.Skip(pagination.Offset()).Take(pagination.Limit);
        }
    }
}