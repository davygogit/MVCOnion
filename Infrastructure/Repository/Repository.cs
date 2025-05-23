using Domain;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        internal MVCOnionContext context;
        internal DbSet<TEntity> dbSet;
        public Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public Task<TEntity> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangeAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
