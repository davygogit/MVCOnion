using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Interface générique pour les repositories avec support soft delete
    /// </summary>
    public interface IRepository<TEntity> where TEntity : Entity
    {
        // Requêtes
        Task<TEntity?> GetByIdAsync(int id, bool includeDeleted = false);
        Task<List<TEntity>> GetAllAsync(bool includeDeleted = false);
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDeleted = false);
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync(bool includeDeleted = false);
        
        // Requêtes avec relations (pour Include)
        IQueryable<TEntity> GetQueryable(bool includeDeleted = false);
        
        // CRUD
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id, bool hardDelete = false); // Soft delete par défaut
        Task RestoreAsync(int id); // Restaurer une entité soft-deleted
        
        // Sauvegarde
        Task<int> SaveChangesAsync();
    }
}
