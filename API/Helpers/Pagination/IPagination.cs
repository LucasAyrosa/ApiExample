namespace ToDoAPI.API.Helpers.Pagination
{
    public interface IPagination
    {
        int Limit { get; set; }
        int Page { get; set; }
        int Offset() => Limit * (Page - 1);
    }
}