# 🎯 Recommandations d'Amélioration - OnionWPF

## 📋 Plan d'Action Prioritaire

### 🔴 Priorité HAUTE (À faire immédiatement)

#### 1. **Ajouter des Tests Unitaires** 
**Impact** : Qualité du code, maintenabilité  
**Effort** : 2-3 jours

**Actions** :
```powershell
# Créer projet de tests
dotnet new xunit -n Domain.Tests
dotnet new xunit -n Infrastructure.Tests
dotnet new xunit -n WPF.Tests

# Ajouter à la solution
dotnet sln add Domain.Tests/Domain.Tests.csproj
dotnet sln add Infrastructure.Tests/Infrastructure.Tests.csproj
dotnet sln add WPF.Tests/WPF.Tests.csproj
```

**Packages recommandés** :
- `xUnit` ou `NUnit` - Framework de tests
- `Moq` - Mocking pour tests
- `FluentAssertions` - Assertions lisibles
- `Coverlet` - Code coverage

**Exemple de test** :
```csharp
[Fact]
public void Salle_SoftDelete_ShouldSetIsDeletedToTrue()
{
    // Arrange
    var salle = new SalleReunion { Nom = "Test" };
    
    // Act
    salle.IsDeleted = true;
    
    // Assert
    salle.IsDeleted.Should().BeTrue();
}
```

**Objectifs** :
- Domain : 90%+ de couverture (entités, logique métier)
- Infrastructure : 70%+ (repositories)
- WPF ViewModels : 60%+ (commands, logique)

---

#### 2. **Implémenter le Logging**
**Impact** : Débogage, monitoring, traçabilité  
**Effort** : 1 jour

**Actions** :
```powershell
# Ajouter Serilog
dotnet add WPF package Serilog
dotnet add WPF package Serilog.Sinks.File
dotnet add WPF package Serilog.Sinks.Console
dotnet add WPF package Serilog.Extensions.Hosting
```

**Configuration (App.xaml.cs)** :
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/onionwpf-.log", 
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

services.AddLogging(loggingBuilder =>
    loggingBuilder.AddSerilog(dispose: true));
```

**Utilisation dans ViewModels** :
```csharp
public class SalleListViewModel : BaseViewModel
{
    private readonly ILogger<SalleListViewModel> _logger;
    
    public SalleListViewModel(ILogger<SalleListViewModel> logger)
    {
        _logger = logger;
    }
    
    private async Task LoadSallesAsync()
    {
        try
        {
            _logger.LogInformation("Chargement des salles...");
            var salles = await _repository.GetAllAsync();
            _logger.LogInformation("Chargement terminé: {Count} salles", salles.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du chargement des salles");
            throw;
        }
    }
}
```

---

#### 3. **Ajouter FluentValidation**
**Impact** : Validation robuste, messages d'erreur clairs  
**Effort** : 1-2 jours

**Actions** :
```powershell
dotnet add Domain package FluentValidation
dotnet add WPF package FluentValidation
```

**Exemple de validateur** :
```csharp
public class SalleValidator : AbstractValidator<Salle>
{
    public SalleValidator()
    {
        RuleFor(s => s.Nom)
            .NotEmpty().WithMessage("Le nom est obligatoire")
            .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères");
            
        RuleFor(s => s.Numero)
            .GreaterThan(0).WithMessage("Le numéro doit être positif");
            
        RuleFor(s => s.EtageId)
            .GreaterThan(0).WithMessage("L'étage est obligatoire");
    }
}

public class SalleReunionValidator : AbstractValidator<SalleReunion>
{
    public SalleReunionValidator()
    {
        Include(new SalleValidator()); // Hérite de la validation de base
        
        RuleFor(s => s.NbPlaces)
            .GreaterThan(0).WithMessage("Le nombre de places doit être positif")
            .LessThanOrEqualTo(100).WithMessage("Maximum 100 places");
    }
}
```

**Intégration dans ViewModel** :
```csharp
private async Task SaveSalleAsync()
{
    var validator = new SalleReunionValidator();
    var result = await validator.ValidateAsync(_currentSalle);
    
    if (!result.IsValid)
    {
        var errors = string.Join("\n", result.Errors.Select(e => e.ErrorMessage));
        _dialogService.ShowError(errors);
        return;
    }
    
    await _repository.AddAsync(_currentSalle);
}
```

---

#### 4. **Gérer les Secrets avec User Secrets**
**Impact** : Sécurité, pas de credentials en clair  
**Effort** : 30 minutes

**Actions** :
```powershell
cd WPF
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\\mssqllocaldb;Database=WebAppMaps;Trusted_Connection=true;"
```

**Configuration (App.xaml.cs)** :
```csharp
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
    .AddUserSecrets<App>() // ✅ Secrets utilisateur
    .Build();
```

**Fichier .gitignore** : User Secrets ne sont JAMAIS commités !

---

### 🟡 Priorité MOYENNE (2-4 semaines)

#### 5. **Ajouter un Service de Cache**
**Impact** : Performance, réduction des requêtes DB  
**Effort** : 1-2 jours

**Implementation** :
```csharp
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan expiration);
    Task RemoveAsync(string key);
}

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    
    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }
    
    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }
    
    public Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        _cache.Set(key, value, expiration);
        return Task.CompletedTask;
    }
    
    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }
}
```

**Utilisation** :
```csharp
public class SalleListViewModel : BaseViewModel
{
    private readonly ICacheService _cache;
    private const string CACHE_KEY_SALLES = "all_salles";
    
    private async Task LoadSallesAsync()
    {
        // Essayer de charger depuis le cache
        var cachedSalles = await _cache.GetAsync<List<Salle>>(CACHE_KEY_SALLES);
        if (cachedSalles != null)
        {
            Salles = new ObservableCollection<Salle>(cachedSalles);
            return;
        }
        
        // Sinon, charger depuis la DB
        var salles = await _repository.GetAllAsync();
        await _cache.SetAsync(CACHE_KEY_SALLES, salles, TimeSpan.FromMinutes(5));
        Salles = new ObservableCollection<Salle>(salles);
    }
}
```

---

#### 6. **Implémenter un Service de Navigation Typé**
**Impact** : Navigation robuste, type-safe  
**Effort** : 1 jour

**Interface** :
```csharp
public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : BaseViewModel;
    void NavigateTo<TViewModel>(object parameter) where TViewModel : BaseViewModel;
    void GoBack();
    bool CanGoBack { get; }
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<BaseViewModel> _navigationStack = new();
    
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public void NavigateTo<TViewModel>() where TViewModel : BaseViewModel
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        _navigationStack.Push(viewModel);
        // Lever un événement ou notifier MainViewModel
        NavigationChanged?.Invoke(this, viewModel);
    }
    
    public void GoBack()
    {
        if (_navigationStack.Count > 1)
        {
            _navigationStack.Pop();
            var previous = _navigationStack.Peek();
            NavigationChanged?.Invoke(this, previous);
        }
    }
    
    public bool CanGoBack => _navigationStack.Count > 1;
    public event EventHandler<BaseViewModel>? NavigationChanged;
}
```

---

#### 7. **Ajouter des Annotations de Documentation XML**
**Impact** : IntelliSense, documentation auto  
**Effort** : 2 jours

**Configuration (Domain.csproj)** :
```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <DocumentationFile>bin\$(Configuration)\$(TargetFramework)\$(AssemblyName).xml</DocumentationFile>
</PropertyGroup>
```

**Exemple** :
```csharp
/// <summary>
/// Représente une salle de réunion avec équipements spécifiques.
/// </summary>
public class SalleReunion : Salle
{
    /// <summary>
    /// Nombre de places disponibles dans la salle.
    /// </summary>
    /// <remarks>
    /// Doit être supérieur à 0 et inférieur à 100.
    /// </remarks>
    public int NbPlaces { get; set; }
    
    /// <summary>
    /// Indique si la salle dispose d'équipement de visioconférence.
    /// </summary>
    public bool EquipementVideoconf { get; set; }
}
```

---

#### 8. **Créer un EventAggregator pour Communication Inter-ViewModels**
**Impact** : Découplage, communication flexible  
**Effort** : 1 jour

**Implementation** :
```csharp
public interface IEventAggregator
{
    void Subscribe<TEvent>(Action<TEvent> handler);
    void Publish<TEvent>(TEvent eventData);
}

public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
    
    public void Subscribe<TEvent>(Action<TEvent> handler)
    {
        var eventType = typeof(TEvent);
        if (!_subscribers.ContainsKey(eventType))
            _subscribers[eventType] = new List<Delegate>();
            
        _subscribers[eventType].Add(handler);
    }
    
    public void Publish<TEvent>(TEvent eventData)
    {
        var eventType = typeof(TEvent);
        if (_subscribers.ContainsKey(eventType))
        {
            foreach (var handler in _subscribers[eventType])
            {
                ((Action<TEvent>)handler)(eventData);
            }
        }
    }
}

// Événements
public class SalleCreatedEvent
{
    public Salle Salle { get; set; }
}

public class SalleUpdatedEvent
{
    public Salle Salle { get; set; }
}

// Utilisation
public class CreateSalleViewModel : BaseViewModel
{
    private readonly IEventAggregator _eventAggregator;
    
    private async Task SaveAsync()
    {
        await _repository.AddAsync(_salle);
        _eventAggregator.Publish(new SalleCreatedEvent { Salle = _salle });
    }
}

public class SalleListViewModel : BaseViewModel
{
    public SalleListViewModel(IEventAggregator eventAggregator)
    {
        eventAggregator.Subscribe<SalleCreatedEvent>(OnSalleCreated);
        eventAggregator.Subscribe<SalleUpdatedEvent>(OnSalleUpdated);
    }
    
    private void OnSalleCreated(SalleCreatedEvent e)
    {
        Salles.Add(e.Salle);
    }
}
```

---

### 🟢 Priorité BASSE (Long terme)

#### 9. **Migration vers CommunityToolkit.Mvvm**
**Impact** : Moins de code boilerplate  
**Effort** : 2-3 jours

**Packages** :
```powershell
dotnet add WPF package CommunityToolkit.Mvvm
```

**Avant** :
```csharp
private string _nom;
public string Nom
{
    get => _nom;
    set => SetProperty(ref _nom, value);
}
```

**Après** :
```csharp
[ObservableProperty]
private string _nom;
// Génère automatiquement la propriété Nom avec INotifyPropertyChanged!
```

**Commands** :
```csharp
[RelayCommand]
private async Task LoadSallesAsync()
{
    // Le command LoadSallesCommand est généré automatiquement!
}
```

---

#### 10. **Implémenter un Système de Plugins**
**Impact** : Extensibilité  
**Effort** : 1 semaine

Permettre d'ajouter des fonctionnalités sans recompiler (ex: export Excel, PDF).

---

#### 11. **Ajouter un Système de Notifications**
**Impact** : UX améliorée  
**Effort** : 2 jours

Toast notifications ou SnackBar au lieu de MessageBox systématiques.

---

#### 12. **Internationalisation (i18n)**
**Impact** : Multi-langue  
**Effort** : 3-4 jours

Support français/anglais avec fichiers de ressources.

---

## 📊 Refactoring Recommandés

### 1. **Extraire les Magic Strings**
❌ **Avant** :
```csharp
var connectionString = configuration.GetConnectionString("DefaultConnection");
```

✅ **Après** :
```csharp
public static class ConfigurationKeys
{
    public const string DefaultConnection = "ConnectionStrings:DefaultConnection";
}

var connectionString = configuration[ConfigurationKeys.DefaultConnection];
```

---

### 2. **Utiliser des Records pour DTOs**
❌ **Avant** :
```csharp
public class SalleViewModel
{
    public int Id { get; set; }
    public string Nom { get; set; }
}
```

✅ **Après** (C# 9+) :
```csharp
public record SalleDto(int Id, string Nom, int Numero, bool Favori);
```

---

### 3. **Pattern Specification pour Requêtes Complexes**
Au lieu de méthodes spécifiques dans Repository :
```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
}

public class SallesFavorisSpec : ISpecification<Salle>
{
    public Expression<Func<Salle, bool>> Criteria => s => s.Favori && !s.IsDeleted;
    public List<Expression<Func<Salle, object>>> Includes => new() { s => s.Etage };
}

// Utilisation
var spec = new SallesFavorisSpec();
var favoris = await _repository.GetAsync(spec);
```

---

### 4. **Result Pattern pour Gestion d'Erreurs**
Au lieu de exceptions pour logique métier :
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    
    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}

// Utilisation
public async Task<Result<Salle>> CreateSalleAsync(Salle salle)
{
    if (string.IsNullOrEmpty(salle.Nom))
        return Result<Salle>.Failure("Le nom est obligatoire");
        
    await _repository.AddAsync(salle);
    return Result<Salle>.Success(salle);
}
```

---

## 🔧 Outils Recommandés

### Développement
- **Visual Studio 2022** : IDE principal
- **ReSharper** : Refactoring, analyse de code
- **CodeMaid** : Nettoyage automatique
- **GitKraken** ou **SourceTree** : Git GUI

### Qualité
- **SonarLint** : Détection de bugs en temps réel
- **dotCover** : Code coverage
- **BenchmarkDotNet** : Performance testing

### Documentation
- **DocFX** : Génération de documentation statique
- **Mermaid** : Diagrammes dans Markdown

### CI/CD
- **GitHub Actions** : Workflows CI/CD gratuits
- **Azure DevOps** : Alternative complète

---

## 📈 Métriques de Qualité Cibles

| Métrique | Cible | Actuel | Priorité |
|----------|-------|--------|----------|
| **Code Coverage** | 70%+ | 0% | 🔴 Haute |
| **Cyclomatic Complexity** | < 10 par méthode | ? | 🟡 Moyenne |
| **Technical Debt** | < 5% | ? | 🟡 Moyenne |
| **Bugs SonarQube** | 0 | ? | 🔴 Haute |
| **Code Smells** | < 50 | ? | 🟢 Basse |
| **Duplication** | < 3% | ? | 🟢 Basse |

---

## 🎯 Plan d'Action sur 3 Mois

### Mois 1 : Qualité et Stabilité
- Semaine 1-2 : Tests unitaires (Domain + Infrastructure)
- Semaine 3 : Logging + User Secrets
- Semaine 4 : FluentValidation + Documentation

### Mois 2 : Performance et Architecture
- Semaine 1 : Caching
- Semaine 2 : Navigation Service
- Semaine 3 : EventAggregator
- Semaine 4 : Refactoring + Code Review

### Mois 3 : Fonctionnalités Avancées
- Semaine 1-2 : Plan d'étage interactif
- Semaine 3 : Export PDF/Excel
- Semaine 4 : CI/CD + Déploiement

---

## ✅ Checklist de Révision de Code

Avant chaque commit :
- [ ] Les tests passent (quand implémentés)
- [ ] Pas de warnings de compilation
- [ ] Code formaté (Ctrl+K, Ctrl+D)
- [ ] Commentaires XML sur méthodes publiques
- [ ] Pas de code commenté laissé
- [ ] Pas de `Console.WriteLine` ou `Debug.WriteLine` oubliés
- [ ] Variables et méthodes nommées clairement
- [ ] Pas de magic numbers (utiliser des constantes)
- [ ] Exceptions gérées correctement
- [ ] Logging ajouté pour opérations critiques

---

**Dernière révision** : Octobre 2025  
**Prochaine révision** : Janvier 2026
