using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    /// <summary>
    /// Implémentation générique du Repository avec support soft delete
    /// </summary>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        private readonly WebAppMapsContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(WebAppMapsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
        }

        #region Requêtes

        public IQueryable<TEntity> GetQueryable(bool includeDeleted = false)
        {
            return includeDeleted 
                ? _dbSet.AsQueryable() 
                : _dbSet.Where(e => !e.IsDeleted);
        }

        public async Task<TEntity?> GetByIdAsync(int id, bool includeDeleted = false)
        {
            return await GetQueryable(includeDeleted)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<TEntity>> GetAllAsync(bool includeDeleted = false)
        {
            return await GetQueryable(includeDeleted).ToListAsync();
        }

        public async Task<List<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate, 
            bool includeDeleted = false)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await GetQueryable(includeDeleted)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await GetQueryable(includeDeleted: false)
                .AnyAsync(e => e.Id == id);
        }

        public async Task<int> CountAsync(bool includeDeleted = false)
        {
            return await GetQueryable(includeDeleted).CountAsync();
        }

        #endregion

        #region CRUD

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            return entity;
        }

        public Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            entity.Update(); // Met à jour UpdatedAt
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(int id, bool hardDelete = false)
        {
            var entity = await GetByIdAsync(id, includeDeleted: true);
            
            if (entity == null)
                throw new KeyNotFoundException($"Entity with ID {id} not found.");

            if (hardDelete)
            {
                // Hard delete : suppression physique
                _dbSet.Remove(entity);
            }
            else
            {
                // Soft delete : marquer comme supprimé
                entity.IsDeleted = true;
                entity.Update();
                _dbSet.Update(entity);
            }
        }

        public async Task RestoreAsync(int id)
        {
            var entity = await GetByIdAsync(id, includeDeleted: true);
            
            if (entity == null)
                throw new KeyNotFoundException($"Entity with ID {id} not found.");

            if (entity.IsDeleted)
            {
                entity.IsDeleted = false;
                entity.Update();
                _dbSet.Update(entity);
            }
        }

        #endregion

        #region Sauvegarde

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion
    }
}