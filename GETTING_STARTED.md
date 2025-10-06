# 🚀 Guide de Démarrage Rapide OnionWPF - Pour Nouveaux Développeurs

Bienvenue dans le projet OnionWPF ! Ce guide vous permettra d'être opérationnel en 15 minutes.

## ✅ Prérequis (5 min)

### Logiciels Requis

1. **Visual Studio 2022** (Community, Professional ou Enterprise)
   - Télécharger : https://visualstudio.microsoft.com/downloads/
   - Workloads requis :
     - ✅ Développement .NET Desktop
     - ✅ Développement ASP.NET et web
     - ✅ Stockage et traitement de données

2. **.NET 9.0 SDK**
   - Télécharger : https://dotnet.microsoft.com/download/dotnet/9.0
   - Vérifier l'installation :
     ```powershell
     dotnet --version
     # Devrait afficher 9.0.xxx
     ```

3. **SQL Server LocalDB** (inclus avec Visual Studio)
   - Vérifier l'installation :
     ```powershell
     sqllocaldb info
     # Devrait lister: MSSQLLocalDB
     ```

### Extensions VS Code Recommandées (si vous utilisez VS Code)
- C# Dev Kit
- .NET Extension Pack
- EditorConfig for VS Code

---

## 📦 Installation (5 min)

### 1. Cloner le Repository

```powershell
git clone [URL_DU_REPO]
cd MVCOnion
```

### 2. Restaurer les Packages NuGet

```powershell
dotnet restore OnionWPF.sln
```

### 3. Vérifier la Configuration de la Base de Données

Ouvrir `WPF/appsettings.json` et vérifier la connection string :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 4. Créer la Base de Données

```powershell
cd Infrastructure
dotnet ef database update
```

**Résultat attendu** :
```
Build started...
Build succeeded.
Done.
```

Si vous voyez "No migrations were applied. The database is already up to date." - parfait ! La base existe déjà.

### 5. Compiler le Projet

```powershell
cd ..
dotnet build OnionWPF.sln
```

**Résultat attendu** :
```
Générer a réussi
```

---

## 🎯 Premier Lancement (2 min)

### Option 1 : Via Visual Studio

1. Ouvrir `OnionWPF.sln` dans Visual Studio
2. Dans l'explorateur de solutions, clic-droit sur **WPF** → "Définir comme projet de démarrage"
3. Appuyer sur `F5` ou cliquer sur "Démarrer"

### Option 2 : Via Ligne de Commande

```powershell
cd WPF
dotnet run
```

**L'application devrait se lancer !** 🎉

---

## 🧪 Vérifier que Tout Fonctionne (3 min)

### Test 1 : Créer un Étage

1. Dans l'application, cliquer sur **"Créer Étage"**
2. Remplir :
   - Niveau : `1`
   - Nom : `Premier Étage`
3. Cliquer sur **"Parcourir"** pour ajouter une image (optionnel)
4. Cliquer sur **"Enregistrer"**

✅ **Succès** : Un message de confirmation apparaît

### Test 2 : Créer une Salle

1. Cliquer sur **"Créer Salle"**
2. Remplir :
   - Nom : `Salle de test`
   - Numéro : `101`
   - Étage : Sélectionner "Premier Étage"
   - Type : `Réunion`
3. Si type Réunion, remplir :
   - Nombre de places : `10`
4. Cliquer sur **"Enregistrer"**

✅ **Succès** : Un message de confirmation apparaît

### Test 3 : Rechercher une Salle

1. Cliquer sur **"Rechercher Salle"**
2. Taper `test` dans la barre de recherche
3. La salle "Salle de test" devrait apparaître

✅ **Succès** : La salle est affichée dans la liste

### Test 4 : Marquer une Salle en Favori

1. Dans la liste des salles, cliquer sur l'étoile ☆ de "Salle de test"
2. L'étoile devrait devenir ⭐

✅ **Succès** : L'étoile est remplie

---

## 📚 Explorer le Projet (en cours de travail)

### Structure des Dossiers

```
MVCOnion/
├── Domain/              👈 Entités et logique métier (COMMENCER ICI)
│   ├── Entity/
│   ├── Salle/
│   ├── Etage/
│   └── Repository/
│
├── Infrastructure/      👈 Accès aux données (EF Core)
│   ├── WebAppMapsContext.cs
│   ├── Repository/
│   └── Migrations/
│
├── WPF/                 👈 Interface utilisateur (MVVM)
│   ├── App.xaml.cs      # Point d'entrée, Dependency Injection
│   ├── MainWindow.xaml  # Fenêtre principale
│   ├── ViewModels/      # Logique de présentation
│   ├── Views/           # Interfaces XAML
│   ├── Commands/        # ICommand implémentations
│   └── Services/        # Services (Dialog, Image)
│
├── Web/                 # Application ASP.NET MVC (legacy)
│
├── ARCHITECTURE.md      👈 Lire ceci pour comprendre l'architecture
├── IMPROVEMENTS.md      👈 Roadmap technique
└── OnionWPF.sln        # Solution principale
```

### Fichiers Importants à Connaître

| Fichier | Description | Quand le modifier |
|---------|-------------|-------------------|
| `Domain/Salle/Salle.cs` | Entité Salle | Ajouter propriétés |
| `Infrastructure/WebAppMapsContext.cs` | Configuration DbContext | Ajouter DbSet, changer config |
| `WPF/App.xaml.cs` | Configuration DI | Ajouter services |
| `WPF/ViewModels/SalleListViewModel.cs` | Logique liste salles | Modifier recherche, filtres |
| `WPF/Views/SalleListView.xaml` | Interface liste salles | Modifier UI |
| `appsettings.json` | Configuration app | Changer connection string |

---

## 🔧 Commandes Essentielles

### Développement

```powershell
# Compiler
dotnet build

# Lancer l'application
cd WPF
dotnet run

# Nettoyer
dotnet clean

# Restaurer packages
dotnet restore
```

### Base de Données

```powershell
# Créer une migration
cd Infrastructure
dotnet ef migrations add NomDeLaMigration

# Appliquer migrations
dotnet ef database update

# Voir les migrations
dotnet ef migrations list

# Supprimer la dernière migration (non appliquée)
dotnet ef migrations remove

# Recréer la base complètement
dotnet ef database drop
dotnet ef database update
```

### Git

```powershell
# Voir le statut
git status

# Ajouter des fichiers
git add .

# Commit
git commit -m "Votre message"

# Push
git push

# Pull dernières modifications
git pull
```

---

## 🐛 Problèmes Fréquents et Solutions

### Problème 1 : "The type or namespace name 'Domain' could not be found"

**Solution** :
```powershell
dotnet restore
dotnet build
```

### Problème 2 : "Cannot connect to SQL Server"

**Solutions** :
1. Vérifier que LocalDB est installé :
   ```powershell
   sqllocaldb info
   ```

2. Démarrer l'instance :
   ```powershell
   sqllocaldb start MSSQLLocalDB
   ```

3. Vérifier la connection string dans `appsettings.json`

### Problème 3 : "XamlParseException: No matching constructor found"

**Solution** : Cette erreur a déjà été corrigée. Si elle réapparaît, vérifier `App.xaml` :
- ✅ **Pas** de `StartupUri="MainWindow.xaml"`
- ✅ `OnStartup` crée manuellement la fenêtre

### Problème 4 : "Build failed" avec erreurs sur fichiers .gz

**Solution** :
```powershell
dotnet clean
Remove-Item -Recurse -Force "Web\obj\Debug" -ErrorAction SilentlyContinue
dotnet build
```

### Problème 5 : Migrations échouent avec "Unable to create DbContext"

**Solution** : Vérifier que `WebAppMapsContextFactory.cs` existe dans Infrastructure.

---

## 📖 Prochaines Étapes

### Jour 1 : Comprendre l'Architecture
- [ ] Lire [ARCHITECTURE.md](ARCHITECTURE.md) (30 min)
- [ ] Explorer les dossiers Domain, Infrastructure, WPF
- [ ] Comprendre le pattern MVVM
- [ ] Tracer le flux : View → ViewModel → Repository → DbContext

### Jour 2 : Faire des Modifications Simples
- [ ] Ajouter une propriété à `Salle` (ex: `Capacité`)
- [ ] Créer une migration
- [ ] Mettre à jour le ViewModel et la View
- [ ] Tester les changements

### Jour 3 : Créer une Nouvelle Fonctionnalité
- [ ] Créer un nouveau type de salle (ex: `SalleLaboratoire`)
- [ ] Ajouter les champs spécifiques
- [ ] Mettre à jour le formulaire de création
- [ ] Tester end-to-end

### Semaine 1 : Contribuer au Projet
- [ ] Lire [IMPROVEMENTS.md](IMPROVEMENTS.md) pour voir la roadmap
- [ ] Choisir une tâche (ex: ajouter logging)
- [ ] Créer une branche Git
- [ ] Implémenter la fonctionnalité
- [ ] Faire une Pull Request

---

## 🎓 Ressources d'Apprentissage

### Architecture
- **Onion Architecture** : [Article original de Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/)
- **Clean Architecture** : [Article de Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

### MVVM Pattern
- [MVVM Pattern - Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm)
- [WPF MVVM Tutorial](https://www.youtube.com/watch?v=VxZtvHfbg2I)

### Entity Framework Core
- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

### WPF
- [WPF Tutorial - Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Data Binding](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/data/)

---

## 💬 Obtenir de l'Aide

### Documentation Interne
1. [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture détaillée
2. [IMPROVEMENTS.md](IMPROVEMENTS.md) - Roadmap et bonnes pratiques
3. [TROUBLESHOOTING.md](WPF/TROUBLESHOOTING.md) - Solutions aux problèmes
4. [DATABASE_SETUP.md](DATABASE_SETUP.md) - Configuration base de données

### Contacts
- **Lead Dev** : [Nom]
- **Architecture** : [Nom]
- **Slack Channel** : #onionwpf
- **Email** : [email]

### Ouvrir une Issue
Si vous rencontrez un bug ou avez une suggestion :
1. Aller sur GitHub Issues
2. Cliquer "New Issue"
3. Décrire le problème avec :
   - Ce que vous essayiez de faire
   - Ce qui s'est passé
   - Message d'erreur (si applicable)
   - Étapes pour reproduire

---

## ✅ Checklist de Configuration

Avant de commencer à coder, vérifier que :

- [ ] Visual Studio 2022 installé avec workloads .NET Desktop
- [ ] .NET 9.0 SDK installé (`dotnet --version`)
- [ ] SQL Server LocalDB installé (`sqllocaldb info`)
- [ ] Repository cloné
- [ ] Packages NuGet restaurés (`dotnet restore`)
- [ ] Base de données créée (`dotnet ef database update`)
- [ ] Solution compile (`dotnet build OnionWPF.sln`)
- [ ] Application se lance (`cd WPF && dotnet run`)
- [ ] Tests manuels réussis (créer étage, créer salle, rechercher)
- [ ] [ARCHITECTURE.md](ARCHITECTURE.md) lu
- [ ] Git configuré (nom, email)

**Si toutes les cases sont cochées, vous êtes prêt à contribuer ! 🎉**

---

## 🚀 Commencer Maintenant

```powershell
# Cloner et configurer
git clone [URL_DU_REPO]
cd MVCOnion
dotnet restore
cd Infrastructure
dotnet ef database update
cd ..
dotnet build OnionWPF.sln

# Lancer
cd WPF
dotnet run
```

**Temps total : ~15 minutes**

---

**Bon développement ! 👨‍💻👩‍💻**

Si vous avez des questions, n'hésitez pas à demander à l'équipe ou consulter la documentation.

---

**Dernière mise à jour** : Octobre 2025  
**Version** : 1.0  
**Auteur** : Équipe OnionWPF
