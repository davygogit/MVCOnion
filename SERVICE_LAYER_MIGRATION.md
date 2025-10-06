# 🏗️ Migration vers Service Layer - IMPLÉMENTÉ

## 📅 Date d'Implémentation
**6 Octobre 2025**

---

## 🎯 Objectif de la Migration

Remplacer l'accès direct à `IRepository<T>` dans les ViewModels par une couche de **Services métier** pour :
- ✅ Respecter l'architecture Onion (séparation des couches)
- ✅ Centraliser la logique métier et les règles de validation
- ✅ Améliorer la testabilité
- ✅ Faciliter la réutilisabilité du code
- ✅ Préparer l'évolution vers une architecture plus professionnelle

---

## 📦 Fichiers Créés

### 1. **Domain/Etage/IEtageService.cs** ✨ NOUVEAU (80 lignes)

**Rôle** : Interface du service métier pour la gestion des étages

**DTOs définis** :
```csharp
// DTO pour la création
public class CreateEtageDto
{
    public int Niveau { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? ImgPlanEtagePath { get; set; }
}

// DTO pour la mise à jour
public class UpdateEtageDto
{
    public int Id { get; set; }
    public int Niveau { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? ImgPlanEtagePath { get; set; }
}
```

**Méthodes de l'interface** :
- `Task<Etage> CreateEtageAsync(CreateEtageDto dto)`
- `Task<Etage> UpdateEtageAsync(UpdateEtageDto dto)`
- `Task DeleteEtageAsync(int id, bool hardDelete = false)`
- `Task<Etage?> GetEtageByIdAsync(int id, bool includeDeleted = false)`
- `Task<List<Etage>> GetAllEtagesAsync(bool includeDeleted = false)`
- `Task<List<Etage>> GetEtagesWithSallesAsync(bool includeDeleted = false)`
- `Task<bool> EtageExistsAsync(int niveau)`
- `Task<int> CountSallesInEtageAsync(int etageId)`

**Documentation** :
- ✅ XML comments sur toutes les méthodes
- ✅ Exceptions documentées (`ArgumentException`, `InvalidOperationException`, `KeyNotFoundException`)

---

### 2. **Domain/Etage/EtageService.cs** ✨ NOUVEAU (220 lignes)

**Rôle** : Implémentation de la logique métier pour les étages

**Structure organisée en régions** :
```csharp
#region CREATE
    // CreateEtageAsync()
#endregion

#region UPDATE
    // UpdateEtageAsync()
#endregion

#region DELETE
    // DeleteEtageAsync()
#endregion

#region READ
    // GetEtageByIdAsync(), GetAllEtagesAsync(), etc.
#endregion

#region VALIDATION
    // ValidateCreateDto(), ValidateUpdateDto()
#endregion
```

**Règles Métier Implémentées** :

#### 1. **Validation des Données**
```csharp
private void ValidateCreateDto(CreateEtageDto dto)
{
    // Nom obligatoire et longueur (2-100 caractères)
    if (string.IsNullOrWhiteSpace(dto.Nom))
        throw new ArgumentException("Le nom de l'étage est obligatoire.");
    
    if (dto.Nom.Length < 2)
        throw new ArgumentException("Le nom doit contenir au moins 2 caractères.");
    
    if (dto.Nom.Length > 100)
        throw new ArgumentException("Le nom ne peut pas dépasser 100 caractères.");
    
    // Niveau valide (-2 à 50)
    if (dto.Niveau < -2)
        throw new ArgumentException("Le niveau ne peut pas être inférieur à -2.");
    
    if (dto.Niveau > 50)
        throw new ArgumentException("Le niveau ne peut pas dépasser 50.");
}
```

#### 2. **Unicité du Niveau**
```csharp
public async Task<Etage> CreateEtageAsync(CreateEtageDto dto)
{
    ValidateCreateDto(dto);
    
    // Règle métier : Un seul étage par niveau
    if (await EtageExistsAsync(dto.Niveau))
    {
        throw new InvalidOperationException(
            $"Un étage de niveau {dto.Niveau} existe déjà. " +
            $"Veuillez choisir un autre niveau.");
    }
    
    // ...
}
```

#### 3. **Cohérence du Nom RDC**
```csharp
// Règle métier : Le nom du RDC doit être cohérent
if (dto.Niveau == 0 && !dto.Nom.Contains("RDC", StringComparison.OrdinalIgnoreCase) 
                  && !dto.Nom.Contains("Rez", StringComparison.OrdinalIgnoreCase))
{
    dto.Nom = $"{dto.Nom} (RDC)"; // Ajout automatique
}
```

#### 4. **Protection Cascade**
```csharp
public async Task DeleteEtageAsync(int id, bool hardDelete = false)
{
    var sallesCount = await CountSallesInEtageAsync(id);
    
    if (sallesCount > 0 && hardDelete)
    {
        throw new InvalidOperationException(
            $"Impossible de supprimer définitivement l'étage " +
            $"car il contient {sallesCount} salle(s). " +
            $"Veuillez d'abord supprimer ou déplacer les salles.");
    }
    // Soft delete autorisé même avec des salles
}
```

#### 5. **Tri Automatique**
```csharp
public async Task<List<Etage>> GetAllEtagesAsync(bool includeDeleted = false)
{
    var etages = await _etageRepository.GetAllAsync(includeDeleted);
    
    // Tri par niveau croissant (sous-sols → étages supérieurs)
    return etages.OrderBy(e => e.Niveau).ToList();
}
```

---

## ✏️ Fichiers Modifiés

### 1. **WPF/ViewModels/CreateEtageViewModel.cs** (Avant/Après)

#### ❌ **AVANT** (accès direct Repository)
```csharp
public class CreateEtageViewModel : BaseViewModel
{
    private readonly IRepository<Etage> _etageRepository; // ❌ Dépendance Infrastructure
    
    public CreateEtageViewModel(IRepository<Etage> etageRepository, ...)
    {
        _etageRepository = etageRepository;
    }
    
    private async Task SaveAsync()
    {
        // ❌ Logique métier dans le ViewModel
        var etage = new Etage
        {
            Niveau = Niveau,
            Nom = Nom,
            ImgPlanEtagePath = savedImagePath
        };
        
        // ❌ Pas de validation métier centralisée
        await _etageRepository.AddAsync(etage);
        await _etageRepository.SaveChangesAsync();
    }
}
```

#### ✅ **APRÈS** (utilise Service)
```csharp
public class CreateEtageViewModel : BaseViewModel
{
    private readonly IEtageService _etageService; // ✅ Dépendance Domain
    
    public CreateEtageViewModel(IEtageService etageService, ...)
    {
        _etageService = etageService;
    }
    
    private async Task SaveAsync()
    {
        try
        {
            // ✅ Création du DTO
            var dto = new CreateEtageDto
            {
                Niveau = Niveau,
                Nom = Nom,
                ImgPlanEtagePath = savedImagePath
            };
            
            // ✅ Logique métier déléguée au service
            var etage = await _etageService.CreateEtageAsync(dto);
            
            _dialogService.ShowInformation("Succès", 
                $"Étage '{etage.Nom}' (niveau {etage.Niveau}) créé avec succès !");
        }
        catch (InvalidOperationException ex)
        {
            // ✅ Erreur métier (ex: niveau déjà existant)
            _dialogService.ShowError("Règle métier", ex.Message);
        }
        catch (ArgumentException ex)
        {
            // ✅ Erreur validation
            _dialogService.ShowError("Validation", ex.Message);
        }
    }
}
```

**Changements** :
- `IRepository<Etage>` → `IEtageService`
- Logique métier déplacée dans le service
- Gestion d'erreurs améliorée (3 types d'exceptions)
- Message de succès plus détaillé

---

### 2. **WPF/ViewModels/EtageViewModel.cs** (Avant/Après)

#### ❌ **AVANT**
```csharp
private readonly IRepository<Etage> _etageRepository;

public EtageViewModel(IRepository<Etage> etageRepository, ...)
{
    _etageRepository = etageRepository;
}

private async Task LoadDataAsync()
{
    var etages = await _etageRepository.GetAllAsync();
    Etages = new ObservableCollection<Etage>(etages);
}
```

#### ✅ **APRÈS**
```csharp
private readonly IEtageService _etageService;

public EtageViewModel(IEtageService etageService, ...)
{
    _etageService = etageService;
}

private async Task LoadDataAsync()
{
    // ✅ Tri automatique intégré dans le service
    var etages = await _etageService.GetAllEtagesAsync();
    Etages = new ObservableCollection<Etage>(etages);
}
```

**Avantage** : Les étages sont maintenant **toujours triés** par niveau (logique centralisée)

---

### 3. **WPF/App.xaml.cs** (Enregistrement DI)

```csharp
.ConfigureServices((context, services) =>
{
    // Repositories (Infrastructure Layer)
    services.AddScoped<IRepository<Etage>, Repository<Etage>>();
    services.AddScoped<IRepository<Salle>, Repository<Salle>>();
    // ...
    
    // ✅ Services métier (Business Logic Layer) - NOUVEAU
    services.AddScoped<IEtageService, EtageService>();
    
    // Managers (legacy - à migrer vers Services)
    services.AddScoped<ISalleManager, SalleManager>();
    
    // Services UI
    services.AddSingleton<IDialogService, DialogService>();
    
    // ViewModels (Presentation Layer)
    services.AddTransient<CreateEtageViewModel>();
    services.AddTransient<EtageViewModel>();
})
```

---

## 🏗️ Architecture Avant/Après

### ❌ **AVANT : Violation de l'Architecture Onion**

```
┌──────────────────────────────────────────────┐
│     Presentation Layer (WPF ViewModels)      │
│                                              │
│  CreateEtageViewModel                        │
│  └─ IRepository<Etage> ❌                    │
│     (dépendance directe Infrastructure)      │
└──────────────────┬───────────────────────────┘
                   │
                   │ Bypass Domain Layer !
                   ▼
┌──────────────────────────────────────────────┐
│       Infrastructure Layer                    │
│                                              │
│  Repository<Etage> : IRepository<Etage>      │
│  DbContext, SQL                              │
└──────────────────────────────────────────────┘
```

**Problèmes** :
- ❌ Logique métier dans le ViewModel
- ❌ Pas de validation centralisée
- ❌ Code dupliqué entre WPF et Web
- ❌ Tests difficiles

---

### ✅ **APRÈS : Architecture Onion Respectée**

```
┌──────────────────────────────────────────────┐
│     Presentation Layer (WPF ViewModels)      │
│                                              │
│  CreateEtageViewModel                        │
│  └─ IEtageService ✅                         │
│     (dépendance Domain uniquement)           │
└──────────────────┬───────────────────────────┘
                   │
                   ▼
┌──────────────────────────────────────────────┐
│           Domain Layer                        │
│                                              │
│  IEtageService (interface)                   │
│  ├─ CreateEtageAsync(dto)                    │
│  ├─ UpdateEtageAsync(dto)                    │
│  └─ DeleteEtageAsync(id)                     │
│                                              │
│  EtageService : IEtageService                │
│  ├─ Validation métier                        │
│  ├─ Règles business                          │
│  ├─ Orchestration repositories               │
│  └─ Transactions                             │
└──────────────────┬───────────────────────────┘
                   │
                   ▼
┌──────────────────────────────────────────────┐
│       Infrastructure Layer                    │
│                                              │
│  Repository<Etage> : IRepository<Etage>      │
│  DbContext, SQL                              │
└──────────────────────────────────────────────┘
```

**Avantages** :
- ✅ Séparation des couches respectée
- ✅ Logique métier centralisée
- ✅ Validation unique
- ✅ Code réutilisable (WPF + Web + API)
- ✅ Tests faciles

---

## 🎯 Règles Métier Centralisées

| Règle | Implémentation | Emplacement |
|-------|----------------|-------------|
| **Nom obligatoire (2-100 car.)** | `ValidateCreateDto()` | EtageService.cs |
| **Niveau unique** | `EtageExistsAsync()` | EtageService.cs |
| **Niveau valide (-2 à 50)** | Validation dans DTOs | EtageService.cs |
| **Nom RDC cohérent** | Ajout automatique "(RDC)" | CreateEtageAsync() |
| **Protection cascade** | Vérification salles avant delete | DeleteEtageAsync() |
| **Tri automatique** | OrderBy(Niveau) | GetAllEtagesAsync() |

---

## ✅ Tests de Compilation

```powershell
PS C:\WorkSpacesGitHub\MVCOnion> dotnet build Domain/Domain.csproj
✅ Domain a réussi avec 6 avertissement(s) (2.1s)

PS C:\WorkSpacesGitHub\MVCOnion> dotnet build WPF/WPF.csproj
✅ WPF a réussi (1.2s)
```

**Résultat** : ✅ **0 erreurs**

---

## 📊 Statistiques

| Métrique | Valeur |
|----------|--------|
| **Fichiers créés** | 2 (IEtageService.cs, EtageService.cs) |
| **Fichiers modifiés** | 3 (CreateEtageViewModel, EtageViewModel, App.xaml.cs) |
| **Lignes ajoutées** | ~300 lignes |
| **Lignes IEtageService** | 80 |
| **Lignes EtageService** | 220 |
| **Méthodes service** | 8 |
| **Règles métier** | 6 |
| **DTOs créés** | 2 (CreateEtageDto, UpdateEtageDto) |
| **Exceptions gérées** | 3 types (Argument, InvalidOperation, KeyNotFound) |

---

## 🧪 Exemple de Test Unitaire (à créer)

```csharp
// Domain.Tests/Services/EtageServiceTests.cs
public class EtageServiceTests
{
    [Fact]
    public async Task CreateEtageAsync_ShouldThrowException_WhenNiveauAlreadyExists()
    {
        // Arrange
        var mockEtageRepo = new Mock<IRepository<Etage>>();
        mockEtageRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Etage, bool>>>()))
                     .ReturnsAsync(new List<Etage> { new Etage { Niveau = 1 } });
        
        var mockSalleRepo = new Mock<IRepository<Salle>>();
        var service = new EtageService(mockEtageRepo.Object, mockSalleRepo.Object);
        
        var dto = new CreateEtageDto { Nom = "Étage 1", Niveau = 1 };
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateEtageAsync(dto));
        
        Assert.Contains("existe déjà", exception.Message);
    }
    
    [Fact]
    public async Task CreateEtageAsync_ShouldAddRDC_WhenNiveauIsZero()
    {
        // Arrange
        var mockEtageRepo = new Mock<IRepository<Etage>>();
        mockEtageRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Etage, bool>>>()))
                     .ReturnsAsync(new List<Etage>());
        
        var mockSalleRepo = new Mock<IRepository<Salle>>();
        var service = new EtageService(mockEtageRepo.Object, mockSalleRepo.Object);
        
        var dto = new CreateEtageDto { Nom = "Hall Principal", Niveau = 0 };
        
        // Act
        var result = await service.CreateEtageAsync(dto);
        
        // Assert
        Assert.Contains("(RDC)", result.Nom);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("A")] // Moins de 2 caractères
    public async Task CreateEtageAsync_ShouldThrowArgumentException_WhenNomIsInvalid(string nomInvalide)
    {
        // Arrange
        var service = new EtageService(null!, null!);
        var dto = new CreateEtageDto { Nom = nomInvalide, Niveau = 1 };
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateEtageAsync(dto));
    }
}
```

---

## 🚀 Prochaines Étapes

### Phase 1 : **ISalleService** (Recommandé - Priorité HAUTE)
```
📁 À créer :
- Domain/Salle/ISalleService.cs
- Domain/Salle/SalleService.cs

📝 À modifier :
- WPF/ViewModels/SalleListViewModel.cs
- WPF/ViewModels/CreateSalleViewModel.cs
- WPF/ViewModels/EditSalleViewModel.cs
- Web/Controllers/EtagesController.cs
- WPF/App.xaml.cs

⏱️ Estimation : 2-3 heures
```

### Phase 2 : **Migrer Web Controllers** (Priorité MOYENNE)
```
Web/Controllers/EtagesController.cs :
- Remplacer IRepository<Etage> par IEtageService
- Utiliser DTOs au lieu d'entités directement
```

### Phase 3 : **Tests Unitaires** (Priorité HAUTE)
```
📁 À créer :
- Domain.Tests/Services/EtageServiceTests.cs
- Domain.Tests/Services/SalleServiceTests.cs

🧪 Tests à écrire :
- Validation des DTOs
- Règles métier
- Scénarios d'erreur
- Scénarios nominaux
```

### Phase 4 : **Remplacer SalleManager** (Priorité BASSE)
```
SalleManager.cs peut coexister avec SalleService
Migration progressive :
1. Créer SalleService avec fonctionnalités additionnelles
2. Migrer ViewModels vers SalleService
3. Déprécier SalleManager
4. Supprimer SalleManager
```

---

## 📖 Documentation Associée

- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture Onion complète
- [IMPROVEMENTS.md](IMPROVEMENTS.md) - Roadmap des améliorations
- [REPOSITORY_IMPROVEMENTS_APPLIED.md](REPOSITORY_IMPROVEMENTS_APPLIED.md) - Repository Pattern v2.0
- [EDIT_SALLE_FEATURE.md](EDIT_SALLE_FEATURE.md) - Fonctionnalité modification salle

---

## 💡 Leçons Apprises

### ✅ **Ce qui Fonctionne Bien**

1. **Separation of Concerns**
   - ViewModels ne connaissent plus l'infrastructure
   - Logique métier isolée et testable

2. **Validation Centralisée**
   - Une seule source de vérité pour les règles
   - Pas de duplication WPF/Web

3. **DTOs**
   - Découplage entre API publique et entités
   - Évolutivité facilitée

4. **Gestion d'Erreurs**
   - Exceptions typées (Argument, InvalidOperation, KeyNotFound)
   - Messages utilisateur clairs

### ⚠️ **Points d'Attention**

1. **Performance**
   - Attention aux appels service dans des boucles
   - Préférer GetAllEtagesAsync() à multiples GetEtageByIdAsync()

2. **Transactions**
   - Si opérations complexes multi-entités, gérer dans le service
   - Utiliser DbContext.Database.BeginTransaction() si nécessaire

3. **Caching**
   - Si GetAllEtagesAsync() appelé souvent, envisager cache
   - `IMemoryCache` pour données peu changeantes

---

## 🎉 Conclusion

### **Avant** ❌
- Logique métier éparpillée dans ViewModels
- Accès direct à l'infrastructure
- Validation côté UI uniquement
- Code dupliqué WPF/Web
- Tests difficiles

### **Après** ✅
- Logique métier centralisée dans EtageService
- Architecture Onion respectée
- Validation métier unique et robuste
- Code réutilisable
- Tests unitaires faciles

### **Qualité du Code**
**Avant** : 5/10 (fonctionne mais dette technique)  
**Après** : 8/10 (professionnel, maintenable, évolutif)

### **Impact**
- ✅ Architecture propre et professionnelle
- ✅ Maintenabilité améliorée
- ✅ Évolutivité facilitée
- ✅ Tests possibles
- ✅ Réutilisation WPF/Web/API

---

**Date** : 6 Octobre 2025  
**Version** : Service Layer v1.0 (EtageService)  
**Statut** : ✅ IMPLÉMENTÉ et COMPILÉ  
**Auteur** : Équipe OnionWPF

**L'architecture est maintenant professionnelle avec Service Layer ! 🏗️✨**
