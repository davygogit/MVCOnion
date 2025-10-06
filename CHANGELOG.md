# Changelog - OnionWPF

Tous les changements notables de ce projet seront documentés dans ce fichier.

Le format est basé sur [Keep a Changelog](https://keepachangelog.com/fr/1.0.0/),
et ce projet adhère au [Semantic Versioning](https://semver.org/lang/fr/).

## [Non publié]

### À Venir
- Tests unitaires (Domain, Infrastructure, WPF)
- Système de logging avec Serilog
- FluentValidation pour validation robuste
- User Secrets pour sécurité développement
- Système de cache pour performance
- Export PDF/Excel des listes de salles
- Plan d'étage interactif avec clic sur salles

---

## [2.0.0] - 2025-10-06

### 🎉 Transformation Majeure : ASP.NET MVC → WPF Desktop

#### Ajouté
- **Application WPF complète** avec architecture MVVM
  - 6 ViewModels (Base, Main, SalleList, CreateSalle, CreateEtage, Etage)
  - 4 Views (SalleListView, CreateSalleView, CreateEtageView, EtageView)
  - Commands (RelayCommand, AsyncRelayCommand) pour actions utilisateur
  - Services (DialogService, ImageService) pour fonctionnalités transversales
  - 6 Converters pour binding XAML
  - Styles.xaml avec thème Material Design

- **Infrastructure de Base de Données**
  - `WebAppMapsContextFactory.cs` pour design-time EF Core
  - Support complet SQL Server LocalDB
  - Migrations existantes réutilisées

- **Documentation Complète** (1500+ lignes)
  - `ARCHITECTURE.md` - Architecture détaillée avec diagrammes
  - `IMPROVEMENTS.md` - Roadmap technique sur 3 mois
  - `ARCHITECTURE_IMPROVEMENTS_SUMMARY.md` - Résumé des améliorations
  - `GETTING_STARTED.md` - Guide démarrage rapide (15 min)
  - `QUICK_START.md` - Lancement rapide
  - `TRANSFORMATION_GUIDE.md` - Processus de transformation MVC→WPF
  - `DATABASE_SETUP.md` - Configuration SQL Server
  - `FIXES_APPLIED.md` - Solutions aux problèmes rencontrés
  - `WPF/README.md` - Documentation technique WPF
  - `WPF/TROUBLESHOOTING.md` - Dépannage
  - `WPF/FIX_XAMLPARSE.md` - Solution XamlParseException

- **Fichiers de Configuration**
  - `Directory.Build.props` - Centralisation versions et configuration
  - `.editorconfig` amélioré (200+ lignes) - Standards de code
  - `appsettings.json` dans WPF avec ConnectionStrings
  - `WPF/.gitignore` - Exclusions Git pour WPF
  - Scripts PowerShell : `start-wpf.ps1`, `restart-wpf.ps1`

- **Dependency Injection**
  - Configuration complète dans `App.xaml.cs`
  - Lifetimes appropriés (Scoped pour DbContext, Singleton pour Services, Transient pour ViewModels)
  - Resolution automatique des dépendances

#### Modifié
- **Solution renommée** : `WebAppMaps.sln` → `OnionWPF.sln`
- **README.md** mis à jour avec focus sur WPF
- **Versions NuGet harmonisées** : Toutes à 9.0.9
  - Microsoft.EntityFrameworkCore.Design 9.0.5 → 9.0.9
  - Microsoft.Extensions.DependencyInjection 9.0.5 → 9.0.9
  - Microsoft.Extensions.Hosting 9.0.5 → 9.0.9
  - Microsoft.Extensions.Configuration.Json 9.0.5 → 9.0.9

- **Infrastructure**
  - Ajout de `Microsoft.Extensions.Configuration.Json` 9.0.9
  - Création du factory pour DbContext design-time

- **App.xaml** (WPF)
  - Suppression de `StartupUri` pour éviter conflit DI
  - Création manuelle de MainWindow avec injection ViewModel

- **Connection String** mise à jour
  - Ancien : `Server=localhost`
  - Nouveau : `Server=(localdb)\\mssqllocaldb` pour LocalDB

#### Corrigé
- **Erreur XamlParseException** : Conflit entre StartupUri XAML et constructeur DI
  - Solution : Suppression StartupUri + création manuelle dans OnStartup
  
- **Erreur "DefaultBinder not found"** : Application déjà en cours d'exécution
  - Solution : Script restart-wpf.ps1 pour kill processes
  
- **Erreur "Serveur n'est pas trouvé"** : Mauvaise connection string
  - Solution : Utilisation de LocalDB avec connection string correcte
  
- **Erreur DbContext design-time** : EF Tools ne pouvait pas créer DbContext
  - Solution : Implémentation IDesignTimeDbContextFactory
  
- **Erreur CS8116** : FavoriConverter nullable mal typé
  - Solution : Changement de `bool?` vers `bool`
  
- **Conflit version packages** : Microsoft.Extensions.Configuration.Json 9.0.5 vs 9.0.9
  - Solution : Mise à jour vers 9.0.9 dans WPF

#### Performance
- Async/Await utilisé partout (Repository, ViewModels, Commands)
- Pagination implémentée dans SalleListViewModel (10 items par page)
- ObservableCollection pour updates UI optimisées

#### Sécurité
- Connection string dans appsettings.json (pas hardcodé)
- TrustServerCertificate configuré pour LocalDB
- Soft delete sur toutes les entités (IsDeleted)

---

## [1.0.0] - 2025-06-27

### Initial Release - Application ASP.NET Core MVC

#### Ajouté
- **Projet Web ASP.NET Core MVC** (.NET 9.0)
  - Controllers (Home, Etages)
  - Views (Razor)
  - wwwroot (CSS, JS, Images)
  - ViewModels

- **Domaine (Domain)**
  - Entité de base `Entity` avec soft delete
  - Entités métier :
    - `Salle` (classe de base)
    - `SalleReunion` (hérite de Salle)
    - `SallePause` (hérite de Salle)
    - `SalleBubble` (hérite de Salle)
    - `Etage`
  - Enum `TypeSalle` (Reunion, Pause, Bubble)
  - Interfaces (`IRepository<T>`, `ISalleManager`)
  - `SalleManager` - Logique métier

- **Infrastructure**
  - `WebAppMapsContext` - DbContext EF Core
  - `Repository<T>` - Implémentation générique
  - Migration initiale `20250627151537_init_fin`
  - Configuration TPH (Table-Per-Hierarchy) pour Salles

- **Fonctionnalités Web**
  - Recherche de salles avec filtres
  - Création d'étages avec upload de plans
  - Création de salles avec types différents
  - Système de favoris
  - Affichage modal des détails
  - Plans d'étages interactifs (Canvas JS)

- **Base de Données**
  - SQL Server support
  - Migrations Entity Framework Core
  - Tables : Salles (TPH), Etages
  - Audit fields : CreatedAt, UpdatedAt, IsDeleted

#### Configuration
- Entity Framework Core 9.0.5
- ASP.NET Core 9.0
- Bootstrap 5 pour UI
- JavaScript vanilla pour interactivité

---

## Format des Versions

- **[Major].[Minor].[Patch]** (Semantic Versioning)
- **[Non publié]** - Modifications en cours de développement
- **Major** : Changements incompatibles avec versions précédentes
- **Minor** : Ajout de fonctionnalités rétro-compatibles
- **Patch** : Corrections de bugs rétro-compatibles

## Catégories de Changements

- **Ajouté** : Nouvelles fonctionnalités
- **Modifié** : Changements dans fonctionnalités existantes
- **Déprécié** : Fonctionnalités bientôt retirées
- **Retiré** : Fonctionnalités retirées
- **Corrigé** : Corrections de bugs
- **Sécurité** : Corrections de vulnérabilités

---

## Feuille de Route

### Version 2.1.0 (Q1 2026)
- [ ] Tests unitaires (60-70% couverture)
- [ ] Logging avec Serilog
- [ ] FluentValidation
- [ ] User Secrets
- [ ] CI/CD avec GitHub Actions

### Version 2.2.0 (Q2 2026)
- [ ] Système de cache (MemoryCache ou Redis)
- [ ] Navigation Service typé
- [ ] EventAggregator pour communication inter-ViewModels
- [ ] Amélioration UI (animations, transitions)

### Version 2.3.0 (Q3 2026)
- [ ] Plan d'étage interactif cliquable
- [ ] Export PDF/Excel
- [ ] Recherche avancée avec multiples filtres
- [ ] Historique des modifications

### Version 3.0.0 (Q4 2026)
- [ ] Multi-utilisateurs avec authentification
- [ ] API REST pour intégrations externes
- [ ] Notifications en temps réel
- [ ] Mode hors-ligne avec synchronisation

---

**Note** : Les dates sont indicatives et peuvent être ajustées selon les priorités du projet.

---

**Maintenu par** : Équipe OnionWPF  
**Contact** : [Email/Slack]  
**Repository** : [URL GitHub]
