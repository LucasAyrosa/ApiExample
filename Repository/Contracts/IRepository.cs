using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Repository.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> All { get; }
        Task<TEntity> FindAsync(long id);
        Task<List<TEntity>> GetAllAsync();
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<bool> SaveChangesAsync();
    }
}