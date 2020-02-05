using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Domain.Models;

namespace ToDoAPI.Repository.Data
{
    public class TodoContext : IdentityDbContext
    {
        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {

        }
        public DbSet<TodoItem> TodoItems { get; set; }
    }
}