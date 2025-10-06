# WebAppMaps - Application WPF

## 🎯 Description

Application WPF (Windows Presentation Foundation) pour la gestion des salles et plans d'étages, transformée depuis l'application ASP.NET Core MVC originale.

## 🏗️ Architecture

L'application suit le pattern **MVVM (Model-View-ViewModel)** avec :
- **Domain** : Couche de logique métier (réutilisée)
- **Infrastructure** : Couche d'accès aux données (réutilisée)
- **WPF** : Couche de présentation avec MVVM

## 📦 Structure du Projet WPF

```
WPF/
├── App.xaml                    # Point d'entrée de l'application
├── App.xaml.cs                 # Configuration et DI
├── MainWindow.xaml             # Fenêtre principale
├── appsettings.json           # Configuration
│
├── ViewModels/                 # ViewModels MVVM
│   ├── BaseViewModel.cs       # Classe de base avec INotifyPropertyChanged
│   ├── MainViewModel.cs       # ViewModel principal avec navigation
│   ├── SalleListViewModel.cs # Liste et recherche de salles
│   ├── CreateSalleViewModel.cs# Création de salles
│   ├── CreateEtageViewModel.cs# Création d'étages
│   └── EtageViewModel.cs      # Gestion des étages
│
├── Views/                      # Vues XAML
│   ├── SalleListView.xaml     # Vue liste des salles
│   ├── CreateSalleView.xaml   # Vue création salle
│   ├── CreateEtageView.xaml   # Vue création étage
│   └── EtageView.xaml         # Vue gestion étages
│
├── Commands/                   # Commands MVVM
│   ├── RelayCommand.cs        # Command synchrone
│   └── AsyncRelayCommand.cs   # Command asynchrone
│
├── Services/                   # Services
│   ├── DialogService.cs       # Gestion des dialogues
│   └── ImageService.cs        # Gestion des images
│
├── Converters/                 # Value Converters
│   └── ValueConverters.cs     # Converters pour XAML
│
└── Resources/                  # Ressources
    └── Styles.xaml            # Styles globaux
```

## 🚀 Installation et Lancement

### Prérequis
- .NET 9.0 SDK
- Windows 10/11
- SQL Server
- Visual Studio 2022 (recommandé)

### Configuration

1. **Configurer la base de données**
   
   Modifier la chaîne de connexion dans `appsettings.json` :
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

2. **Créer la base de données** (si pas déjà fait)
   ```powershell
   cd Infrastructure
   dotnet ef database update
   ```

3. **Lancer l'application**
   ```powershell
   cd WPF
   dotnet run
   ```

   Ou depuis Visual Studio : définir WPF comme projet de démarrage et appuyer sur F5.

## ✨ Fonctionnalités

### 🔍 Recherche de Salles
- Recherche par nom ou numéro
- Filtrage par étage
- Filtrage par type de salle (Réunion, Pause, Bubble)
- Système de favoris
- Affichage en cartes avec informations détaillées

### ➕ Création de Salle
- Formulaire dynamique selon le type de salle
- Upload d'images
- Coordonnées pour positionnement sur plan
- Validation des données

### 🏢 Gestion des Étages
- Création d'étages avec plans
- Upload de plans d'étages
- Visualisation des étages

## 🎨 Interface Utilisateur

L'interface utilise :
- **Material Design** inspiré
- **Responsive Design**
- **Navigation intuitive** via menu supérieur
- **Thème moderne** avec palette de couleurs cohérente
- **Icônes emoji** pour une meilleure UX

## 🔧 Technologies Utilisées

- **WPF** (.NET 9.0) - Interface utilisateur
- **MVVM** - Pattern architectural
- **Entity Framework Core 9.0** - ORM
- **Dependency Injection** - Microsoft.Extensions.DependencyInjection
- **SQL Server** - Base de données
- **Async/Await** - Programmation asynchrone

## 📊 Patterns et Principes

### MVVM (Model-View-ViewModel)
- **Model** : Entités du domaine (Domain)
- **View** : Vues XAML
- **ViewModel** : Logique de présentation et binding

### Dependency Injection
Configuration dans `App.xaml.cs` avec :
- Scoped pour les repositories et DbContext
- Transient pour les ViewModels
- Singleton pour les services

### Commands
- `RelayCommand` : Commands synchrones
- `AsyncRelayCommand` : Commands asynchrones avec gestion de l'état

### Services
- `IDialogService` : Dialogues système
- `IImageService` : Gestion des images

## 🔄 Navigation

La navigation est gérée par le `MainViewModel` qui change dynamiquement le `CurrentViewModel` :
- Navigation via commands dans le menu
- DataTemplates pour mapper ViewModels → Views

## 💾 Gestion des Données

### Repositories
Utilisation des mêmes repositories que l'application MVC :
- Pattern Repository générique
- Support du Soft Delete
- Requêtes asynchrones

### Entity Framework Core
- DbContext partagé avec Infrastructure
- Migrations existantes
- Support TPH (Table Per Hierarchy) pour les types de salles

## 🎯 Améliorations Futures

- [ ] Visualisation interactive des plans d'étages
- [ ] Drag & Drop pour positionner les salles sur le plan
- [ ] Export/Import de données (Excel, CSV)
- [ ] Système d'authentification
- [ ] Historique des modifications
- [ ] Notifications Toast
- [ ] Mode sombre
- [ ] Multi-langue
- [ ] Statistiques et rapports

## 🐛 Problèmes Connus

- Les images ne sont pas encore affichées dans les cartes (binding à implémenter)
- Pas de prévisualisation du plan lors de la création d'étage

## 📝 Notes de Migration

### Différences avec la version MVC

| Aspect | MVC | WPF |
|--------|-----|-----|
| Interface | HTML/CSS/JS | XAML |
| Pattern | MVC | MVVM |
| Navigation | Routing | ViewModel switching |
| Validation | Data Annotations + JS | IDataErrorInfo/FluentValidation |
| Upload fichiers | IFormFile | OpenFileDialog |
| Dialogues | JavaScript | MessageBox/Custom Windows |

## 🤝 Contribution

Pour contribuer :
1. Respecter le pattern MVVM
2. Utiliser les Commands pour les actions
3. Binding en XAML (pas de code-behind)
4. Async/Await pour les opérations longues
5. Dependency Injection pour les dépendances

## 📄 Licence

Même licence que le projet principal WebAppMaps.

---

**Note** : Cette application WPF réutilise les couches Domain et Infrastructure du projet MVC original, garantissant la cohérence des données et de la logique métier.
