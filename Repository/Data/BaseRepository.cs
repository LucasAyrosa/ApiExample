using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Repository.Contracts;

namespace ToDoAPI.Repository.Data
{
    abstract public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly TodoContext _context;
        public BaseRepository(TodoContext context)
        {
            _context = context;
        }
        public IQueryable<TEntity> All => _context.Set<TEntity>().AsQueryable();
        public async Task<TEntity> FindAsync(long id)
        {
            return await _context.FindAsync<TEntity>(id);
        }
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await All.ToListAsync();
        }
        public void Add(TEntity entity)
        {
            _context.Add(entity);
        }
        public void Update(TEntity entity)
        {
            _context.Update(entity);
        }
        public void Remove(TEntity entity)
        {
            _context.Remove(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}