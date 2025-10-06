# 🏗️ Architecture OnionWPF - Guide Complet

## 📋 Vue d'ensemble

OnionWPF est une application de gestion de salles et d'étages qui suit l'architecture en oignon (Onion Architecture) avec une interface WPF utilisant le pattern MVVM.

## 🧅 Architecture en Oignon (Onion Architecture)

```
┌─────────────────────────────────────────────────────────┐
│                    Présentation (WPF)                    │
│  ┌────────────────────────────────────────────────┐    │
│  │              Infrastructure                     │    │
│  │  ┌──────────────────────────────────────┐     │    │
│  │  │            Domain (Core)              │     │    │
│  │  │                                       │     │    │
│  │  │  • Entités (Entity, Salle, Etage)   │     │    │
│  │  │  • Interfaces (IRepository)          │     │    │
│  │  │  • Logique métier (SalleManager)    │     │    │
│  │  │                                       │     │    │
│  │  └──────────────────────────────────────┘     │    │
│  │                                                 │    │
│  │  • DbContext (WebAppMapsContext)              │    │
│  │  • Repositories (Repository<T>)               │    │
│  │  • Migrations EF Core                         │    │
│  │                                                 │    │
│  └────────────────────────────────────────────────┘    │
│                                                          │
│  • ViewModels (MVVM)                                   │
│  • Views (XAML)                                        │
│  • Services (DialogService, ImageService)              │
│  • Commands (RelayCommand, AsyncRelayCommand)          │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

## 📦 Structure des Projets

### 1️⃣ **Domain** (Cœur de l'application)
**Responsabilité** : Logique métier pure, indépendante de toute technologie

```
Domain/
├── Entity/
│   └── Entity.cs              # Classe de base avec soft delete
├── Salle/
│   ├── Salle.cs              # Entité Salle (classe de base)
│   ├── SalleReunion.cs       # Salle de réunion (héritage TPH)
│   ├── SallePause.cs         # Salle de pause
│   ├── SalleBubble.cs        # Salle Bubble
│   ├── TypeSalle.cs          # Enum des types
│   ├── ISalleManager.cs      # Interface logique métier
│   └── SalleManager.cs       # Implémentation logique métier
├── Etage/
│   └── Etage.cs              # Entité Étage
└── Repository/
    └── IRepository.cs        # Interface générique Repository
```

**Principes** :
- ✅ Aucune dépendance externe
- ✅ Entités riches avec logique métier
- ✅ Interfaces pour l'inversion de dépendance (DIP)
- ✅ Soft delete (IsDeleted) pour traçabilité

### 2️⃣ **Infrastructure** (Accès aux données)
**Responsabilité** : Implémentation de la persistance et accès aux données

```
Infrastructure/
├── WebAppMapsContext.cs           # DbContext EF Core
├── WebAppMapsContextFactory.cs    # Factory pour design-time
├── Repository/
│   └── Repository.cs              # Implémentation Repository<T>
└── Migrations/
    ├── 20250627151537_init_fin.cs
    └── WebAppMapsContextModelSnapshot.cs
```

**Stratégie EF Core** :
- **Table-Per-Hierarchy (TPH)** pour Salle et ses dérivées
- **Soft Delete** via IsDeleted
- **Audit automatique** : CreatedAt, UpdatedAt

**Configuration DbContext** :
```csharp
// TPH Configuration
modelBuilder.Entity<Salle>()
    .HasDiscriminator<TypeSalle>("TypeSalle")
    .HasValue<SalleReunion>(TypeSalle.Reunion)
    .HasValue<SallePause>(TypeSalle.Pause)
    .HasValue<SalleBubble>(TypeSalle.Bubble);
```

### 3️⃣ **WPF** (Présentation MVVM)
**Responsabilité** : Interface utilisateur avec pattern MVVM

```
WPF/
├── App.xaml / App.xaml.cs        # Application entry + DI
├── MainWindow.xaml/.cs           # Fenêtre principale
├── ViewModels/
│   ├── BaseViewModel.cs          # INotifyPropertyChanged
│   ├── MainViewModel.cs          # Navigation + Status
│   ├── SalleListViewModel.cs    # Liste et recherche
│   ├── CreateSalleViewModel.cs  # Création/édition salle
│   ├── CreateEtageViewModel.cs  # Création étage
│   └── EtageViewModel.cs        # Liste étages
├── Views/
│   ├── SalleListView.xaml       # Vue liste salles
│   ├── CreateSalleView.xaml     # Vue création salle
│   ├── CreateEtageView.xaml     # Vue création étage
│   └── EtageView.xaml           # Vue liste étages
├── Commands/
│   ├── RelayCommand.cs          # ICommand synchrone
│   └── AsyncRelayCommand.cs     # ICommand asynchrone
├── Services/
│   ├── DialogService.cs         # MessageBox wrapper
│   └── ImageService.cs          # Gestion images
├── Converters/
│   └── ValueConverters.cs       # Convertisseurs XAML
├── Resources/
│   └── Styles.xaml              # Styles Material Design
└── appsettings.json             # Configuration (ConnectionString)
```

**Pattern MVVM** :
- ✅ Séparation View/ViewModel
- ✅ Data Binding bidirectionnel
- ✅ Commands pour les actions
- ✅ Services injectés via DI

### 4️⃣ **Web** (Application MVC - Legacy)
**Responsabilité** : Application web ASP.NET Core MVC (conservée pour compatibilité)

```
Web/
├── Program.cs
├── Controllers/
│   ├── HomeController.cs
│   └── EtagesController.cs
├── Views/
│   ├── Home/
│   └── Etages/
└── wwwroot/
    ├── css/
    ├── js/
    └── assets/
```

## 🔄 Flux de Données (MVVM)

```
┌──────────┐         ┌─────────────┐         ┌────────────┐
│   View   │ ◄────── │  ViewModel  │ ◄────── │ Repository │
│  (XAML)  │ ──────► │   (Logic)   │ ──────► │   (Data)   │
└──────────┘         └─────────────┘         └────────────┘
     │                      │                        │
  Binding            INotifyPropertyChanged    DbContext
  Commands              Commands                EF Core
```

**Exemple de flux** :
1. **User clicks "Rechercher"** → Command dans View
2. **ViewModel.SearchCommand** → Appelle Repository
3. **Repository.GetAllAsync()** → Query EF Core
4. **DbContext** → SQL Server LocalDB
5. **Résultats** → Remontent vers ViewModel
6. **ObservableCollection** → Notifie View (INotifyPropertyChanged)
7. **View** → Affiche les données (Binding)

## 🧩 Dependency Injection

### Configuration (App.xaml.cs)

```csharp
services.AddDbContext<WebAppMapsContext>(options =>
    options.UseSqlServer(connectionString), 
    ServiceLifetime.Scoped); // ❌ Pas Singleton pour DbContext!

// Repositories (Scoped pour correspondre au DbContext)
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services (Singleton car stateless)
services.AddSingleton<DialogService>();
services.AddSingleton<ImageService>();

// ViewModels (Transient car nouvelle instance à chaque navigation)
services.AddTransient<MainViewModel>();
services.AddTransient<SalleListViewModel>();
services.AddTransient<CreateSalleViewModel>();
services.AddTransient<CreateEtageViewModel>();
services.AddTransient<EtageViewModel>();
```

### Lifetimes

| Service | Lifetime | Raison |
|---------|----------|--------|
| **DbContext** | Scoped | Une instance par "scope" (évite memory leaks) |
| **Repository** | Scoped | Doit correspondre au DbContext |
| **Services** | Singleton | Stateless, réutilisable |
| **ViewModels** | Transient | Nouvelle instance à chaque navigation |

## 🗄️ Base de Données

### Configuration SQL Server LocalDB

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### Schéma des Tables

#### Table `Salles` (TPH)
```sql
CREATE TABLE Salles (
    Id INT PRIMARY KEY IDENTITY,
    Discriminator NVARCHAR(MAX) NOT NULL, -- 'SalleReunion', 'SallePause', 'SalleBubble'
    Nom NVARCHAR(100) NOT NULL,
    Numero INT NOT NULL,
    ImgSallePath NVARCHAR(255),
    Favori BIT NOT NULL DEFAULT 0,
    TypeSalle INT NOT NULL,
    CoordonneeX FLOAT,
    CoordonneeY FLOAT,
    EtageId INT FOREIGN KEY REFERENCES Etages(Id),
    
    -- Champs spécifiques SalleReunion
    NbPlaces INT NULL,
    EquipementVideoconf BIT NULL,
    Tableau BIT NULL,
    
    -- Champs spécifiques SallePause
    Machine BIT NULL,
    CoinRepas BIT NULL,
    
    -- Champs spécifiques SalleBubble
    Isolation BIT NULL,
    Telephone BIT NULL,
    
    -- Audit
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
```

#### Table `Etages`
```sql
CREATE TABLE Etages (
    Id INT PRIMARY KEY IDENTITY,
    Niveau INT NOT NULL,
    Nom NVARCHAR(100) NOT NULL,
    ImgPlanEtagePath NVARCHAR(255),
    
    -- Audit
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0
);
```

## 🎨 Design Patterns Utilisés

### 1. **Onion Architecture**
- Dépendances vers l'intérieur uniquement
- Domain au centre (aucune dépendance)
- Infrastructure dépend de Domain
- Présentation dépend de Domain et Infrastructure

### 2. **MVVM (Model-View-ViewModel)**
- Séparation des responsabilités
- Testabilité (ViewModels testables sans UI)
- Data Binding déclaratif

### 3. **Repository Pattern**
- Abstraction de la couche d'accès aux données
- Interface générique `IRepository<T>`
- Implémentation avec EF Core

### 4. **Unit of Work** (implicite avec EF Core)
- `DbContext` agit comme Unit of Work
- `SaveChangesAsync()` = commit transaction

### 5. **Dependency Injection**
- Inversion de contrôle (IoC)
- Microsoft.Extensions.DependencyInjection
- Configuration centralisée dans App.xaml.cs

### 6. **Command Pattern**
- `ICommand` pour les actions utilisateur
- `RelayCommand` et `AsyncRelayCommand`
- Découplage View/ViewModel

### 7. **Factory Pattern**
- `WebAppMapsContextFactory` pour design-time
- Permet à EF Core Tools de créer le DbContext

### 8. **Observer Pattern**
- `INotifyPropertyChanged` pour notifier les changements
- `ObservableCollection<T>` pour les listes

## 📊 Diagramme de Classes Simplifié

```
┌─────────────────────┐
│      Entity         │ (abstract)
├─────────────────────┤
│ + Id: int           │
│ + CreatedAt: DateTime│
│ + UpdatedAt: DateTime│
│ + IsDeleted: bool   │
└──────────┬──────────┘
           │
    ┌──────┴──────┬─────────────┐
    │             │             │
┌───▼────┐   ┌───▼────┐   ┌───▼────┐
│ Salle  │   │ Etage  │   │  ...   │
├────────┤   ├────────┤   └────────┘
│ ...    │   │ Niveau │
└───┬────┘   │ Nom    │
    │        └────────┘
    │
┌───┴─────────┬──────────────┬─────────────┐
│             │              │             │
│ SalleReunion│  SallePause │ SalleBubble │
├─────────────┤──────────────┤─────────────┤
│ NbPlaces    │  Machine     │ Isolation   │
│ Equipement  │  CoinRepas   │ Telephone   │
└─────────────┘──────────────┘─────────────┘
```

## 🚀 Commandes Utiles

### Compilation
```powershell
# Compiler toute la solution
dotnet build OnionWPF.sln

# Compiler un projet spécifique
dotnet build WPF/WPF.csproj
```

### Lancer l'application
```powershell
# Application WPF
cd WPF
dotnet run

# Application Web (legacy)
cd Web
dotnet run
```

### Migrations EF Core
```powershell
# Créer une migration
cd Infrastructure
dotnet ef migrations add NomDeLaMigration

# Appliquer les migrations
dotnet ef database update

# Rollback à une migration spécifique
dotnet ef database update NomDeLaMigration

# Supprimer la dernière migration (non appliquée)
dotnet ef migrations remove
```

### Tests
```powershell
# Exécuter tous les tests (à créer)
dotnet test

# Avec couverture de code
dotnet test /p:CollectCoverage=true
```

## 🔒 Sécurité et Bonnes Pratiques

### 1. **Connection String**
- ✅ Stockée dans `appsettings.json`
- ⚠️ **À FAIRE** : User Secrets pour développement
- ⚠️ **À FAIRE** : Azure Key Vault pour production

### 2. **Validation**
- ✅ ViewModels valident les données avant sauvegarde
- ⚠️ **À FAIRE** : Annotations DataAnnotations sur entités
- ⚠️ **À FAIRE** : FluentValidation pour règles complexes

### 3. **Gestion des Erreurs**
- ✅ Try-catch dans Commands avec DialogService
- ⚠️ **À FAIRE** : Logging (Serilog recommandé)
- ⚠️ **À FAIRE** : Global exception handler

### 4. **Performance**
- ✅ Async/Await partout
- ✅ Pagination dans SalleListViewModel
- ⚠️ **À FAIRE** : Caching pour les données fréquemment lues
- ⚠️ **À FAIRE** : Lazy loading ou Eager loading stratégique

## 📈 Métriques de Code

| Projet | Lignes de Code (approx.) | Fichiers | Complexité |
|--------|--------------------------|----------|------------|
| **Domain** | ~500 | 12 | Faible |
| **Infrastructure** | ~300 | 5 | Moyenne |
| **WPF** | ~2500 | 35+ | Moyenne-Élevée |
| **Web** | ~1000 | 15+ | Moyenne |
| **TOTAL** | ~4300 | 67+ | - |

## 🔄 Roadmap / Améliorations Futures

### Phase 1 : Stabilisation (CURRENT)
- [x] Architecture Onion complète
- [x] MVVM pattern
- [x] Dependency Injection
- [x] Base de données LocalDB
- [ ] Tests unitaires (Domain, Infrastructure)
- [ ] Tests d'intégration (WPF ViewModels)

### Phase 2 : Qualité
- [ ] Logging (Serilog)
- [ ] User Secrets pour dev
- [ ] FluentValidation
- [ ] Code Coverage > 80%
- [ ] Documentation XML comments

### Phase 3 : Fonctionnalités
- [ ] Plan d'étage interactif (clic sur salle)
- [ ] Export PDF/Excel
- [ ] Recherche avancée (filtres multiples)
- [ ] Historique des modifications
- [ ] Multi-utilisateurs (authentification)

### Phase 4 : Performance
- [ ] Caching (Redis ou Memory Cache)
- [ ] Pagination côté serveur
- [ ] Optimisation queries EF Core
- [ ] Virtualisation UI (WPF)

### Phase 5 : Déploiement
- [ ] CI/CD (GitHub Actions ou Azure DevOps)
- [ ] Containerisation (Docker)
- [ ] Azure SQL Database
- [ ] Azure App Service (Web)
- [ ] ClickOnce ou MSIX (WPF)

## 📚 Ressources et Références

- [Clean Architecture - Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Onion Architecture - Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)
- [MVVM Pattern - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [WPF Guide](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)

## 👥 Contributeurs

Projet développé pour la gestion des salles et étages avec transformation de MVC vers WPF.

---

**Version** : 1.0  
**Dernière mise à jour** : Octobre 2025
