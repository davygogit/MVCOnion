namespace Domain.Repository
{    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);
        Task SaveChangeAsync();
    }
}
