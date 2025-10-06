# 🔧 Amélioration du Pattern Repository - Proposition

## 📊 État Actuel vs Proposé

### ❌ **Problèmes Actuels**

1. **Soft Delete Incohérent**
   - `GetAll()` filtre `IsDeleted` ✅
   - Mais `DeleteAsync()` fait un hard delete ❌

2. **`FindAsync(int idEtageDel)` non implémentée**
   ```csharp
   throw new NotImplementedException(); // Crash si appelée
   ```

3. **Manque de méthodes essentielles**
   - Pas de `UpdateAsync()`
   - Pas de filtrage par prédicat
   - Pas de pagination
   - Pas d'include pour relations

4. **`SaveChangeAsync()` dans Repository**
   - Viole le pattern Unit of Work
   - Difficile de gérer les transactions multi-entités

---

## ✅ **Solution Proposée**

### **Option 1 : Amélioration Rapide** (1-2 heures)

#### Fichier : `Domain/Repository/IRepository.cs`

```csharp
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
        
        // Requêtes avec relations
        IQueryable<TEntity> GetQueryable(bool includeDeleted = false);
        
        // CRUD
        Task<TEntity> AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id, bool hardDelete = false); // Soft delete par défaut
        Task RestoreAsync(int id); // Restaurer une entité soft-deleted
        
        // Sauvegarde (à déplacer vers Unit of Work idéalement)
        Task<int> SaveChangesAsync();
    }
}
```

#### Fichier : `Infrastructure/Repository/Repository.cs`

```csharp
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
```

---

## 🎯 **Changements Clés**

### 1. **Soft Delete Cohérent**
```csharp
// Avant (hard delete)
_dbSet.Remove(entity);

// Après (soft delete par défaut)
entity.IsDeleted = true;
entity.Update();
_dbSet.Update(entity);

// Hard delete si nécessaire
await DeleteAsync(id, hardDelete: true);
```

### 2. **Méthodes de Requête Améliorées**
```csharp
// Recherche avec prédicat
var salles = await _repository.FindAsync(s => s.Favori && s.TypeSalle == TypeSalle.Reunion);

// Vérifier existence
if (await _repository.ExistsAsync(salleId)) { ... }

// Compter
var count = await _repository.CountAsync();
```

### 3. **Gestion des Relations**
```csharp
// Avant : impossible de charger Etage
var salle = await _repository.GetByIdAsync(1);

// Après : utiliser GetQueryable()
var salle = await _repository.GetQueryable()
    .Include(s => s.Etage)
    .FirstOrDefaultAsync(s => s.Id == 1);
```

### 4. **Restauration d'Entités Supprimées**
```csharp
// Restaurer une salle soft-deleted
await _repository.RestoreAsync(salleId);
await _repository.SaveChangesAsync();
```

### 5. **Sécurité et Validation**
```csharp
// Validation des arguments
if (entity == null)
    throw new ArgumentNullException(nameof(entity));

// Exception si entité non trouvée
if (entity == null)
    throw new KeyNotFoundException($"Entity with ID {id} not found.");
```

---

## 📊 **Comparaison Avant/Après**

| Fonctionnalité | Avant | Après |
|----------------|-------|-------|
| **Soft Delete** | ❌ Incohérent | ✅ Cohérent par défaut |
| **Hard Delete** | ✅ Par défaut | ✅ Option `hardDelete: true` |
| **Restauration** | ❌ Impossible | ✅ `RestoreAsync()` |
| **Requêtes filtrées** | ❌ Manquant | ✅ `FindAsync(predicate)` |
| **Include relations** | ❌ Difficile | ✅ `GetQueryable()` + Include |
| **Validation** | ❌ Aucune | ✅ ArgumentNullException |
| **Erreurs** | ❌ Silencieuses | ✅ KeyNotFoundException |
| **UpdateAsync** | ❌ Manquant | ✅ Présent |
| **ExistsAsync** | ❌ Manquant | ✅ Présent |
| **CountAsync** | ❌ Manquant | ✅ Présent |

---

## 🚀 **Migration du Code Existant**

### ViewModels à Mettre à Jour

#### SalleListViewModel.cs
```csharp
// ❌ Avant
private async Task LoadSallesAsync()
{
    var salles = _repository.GetAll().ToList(); // Synchrone !
    Salles = new ObservableCollection<Salle>(salles);
}

// ✅ Après
private async Task LoadSallesAsync()
{
    var salles = await _repository.GetAllAsync();
    Salles = new ObservableCollection<Salle>(salles);
}

// ✅ Avec filtrage
private async Task SearchSallesAsync(string searchTerm)
{
    var salles = await _repository.FindAsync(s => 
        s.Nom.Contains(searchTerm) || 
        s.Numero.ToString().Contains(searchTerm));
    Salles = new ObservableCollection<Salle>(salles);
}
```

#### CreateSalleViewModel.cs
```csharp
// ❌ Avant
await _repository.AddAsync(salle);
await _repository.SaveChangeAsync(); // Typo: SaveChange sans 's'

// ✅ Après
await _repository.AddAsync(salle);
await _repository.SaveChangesAsync(); // Avec 's'
```

#### Suppression de Salle
```csharp
// ❌ Avant (hard delete)
await _repository.DeleteAsync(salleId);
await _repository.SaveChangesAsync();

// ✅ Après (soft delete par défaut)
await _repository.DeleteAsync(salleId);
await _repository.SaveChangesAsync();

// ✅ Hard delete si vraiment nécessaire
await _repository.DeleteAsync(salleId, hardDelete: true);
await _repository.SaveChangesAsync();
```

#### Chargement avec Relations
```csharp
// ✅ Charger Salle avec Etage
var salle = await _repository.GetQueryable()
    .Include(s => s.Etage)
    .FirstOrDefaultAsync(s => s.Id == salleId);

// ✅ Charger Etage avec toutes ses Salles
var etage = await _etageRepository.GetQueryable()
    .Include(e => e.Salles)
    .FirstOrDefaultAsync(e => e.Id == etageId);
```

---

## 🎓 **Patterns Avancés (Optionnel)**

### **Option 2 : Specification Pattern** (Plus avancé)

Pour des requêtes complexes réutilisables :

```csharp
// Infrastructure/Specifications/ISpecification.cs
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
}

// Domain/Specifications/SallesFavorisSpec.cs
public class SallesFavorisSpec : ISpecification<Salle>
{
    public Expression<Func<Salle, bool>> Criteria => 
        s => s.Favori && !s.IsDeleted;
    
    public List<Expression<Func<Salle, object>>> Includes => 
        new() { s => s.Etage };
    
    public List<string> IncludeStrings => new();
}

// Utilisation
var spec = new SallesFavorisSpec();
var salles = await _repository.FindAsync(spec);
```

### **Option 3 : Unit of Work Pattern** (Encore plus avancé)

Pour gérer les transactions multi-entités :

```csharp
// IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    IRepository<Salle> Salles { get; }
    IRepository<Etage> Etages { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}

// Utilisation
using var uow = new UnitOfWork(_context);
await uow.BeginTransactionAsync();
try
{
    await uow.Salles.AddAsync(salle);
    await uow.Etages.UpdateAsync(etage);
    await uow.SaveChangesAsync();
    await uow.CommitAsync();
}
catch
{
    await uow.RollbackAsync();
    throw;
}
```

---

## ✅ **Checklist d'Implémentation**

### Phase 1 : Corrections Critiques (30 min)
- [ ] Remplacer `IRepository<TEntity>` par la nouvelle version
- [ ] Remplacer `Repository<TEntity>` par la nouvelle version
- [ ] Compiler et vérifier les erreurs

### Phase 2 : Migration ViewModels (1h)
- [ ] `SalleListViewModel.cs` - Remplacer `GetAll()` par `GetAllAsync()`
- [ ] `CreateSalleViewModel.cs` - Utiliser `UpdateAsync()`
- [ ] Tous les ViewModels - Remplacer `SaveChangeAsync()` par `SaveChangesAsync()`

### Phase 3 : Tests (30 min)
- [ ] Tester CRUD salles
- [ ] Tester CRUD étages
- [ ] Tester soft delete
- [ ] Tester recherche avec filtres

### Phase 4 : Documentation (15 min)
- [ ] Mettre à jour [ARCHITECTURE.md](ARCHITECTURE.md)
- [ ] Ajouter exemples dans [IMPROVEMENTS.md](IMPROVEMENTS.md)

---

## 📈 **Bénéfices Attendus**

1. **✅ Cohérence** : Soft delete partout
2. **✅ Robustesse** : Validation et gestion d'erreurs
3. **✅ Flexibilité** : Requêtes avec prédicats
4. **✅ Performance** : Include pour éviter N+1
5. **✅ Maintenabilité** : Code clair et testable

---

## 🚨 **Points d'Attention**

### Breaking Changes
- `GetAll()` retourne maintenant `Task<List<TEntity>>`
- `SaveChangeAsync()` renommé en `SaveChangesAsync()`
- `DeleteAsync()` fait maintenant un soft delete par défaut

### Rétrocompatibilité
Si vous voulez éviter de casser le code existant :
1. Garder l'ancienne interface `IRepositoryLegacy`
2. Créer `IRepositoryV2` avec les améliorations
3. Migrer progressivement

---

**Estimation temps total : 2-3 heures**  
**Difficulté : Moyenne**  
**Impact : Haut (qualité du code ++)**
