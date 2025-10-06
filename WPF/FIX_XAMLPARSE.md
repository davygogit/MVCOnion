# 🔧 Correctif - Erreur XamlParseException

## ❌ Erreur Rencontrée

```
System.Windows.Markup.XamlParseException : 
'Aucun constructeur correspondant n'a été trouvé sur le type 'WPF.MainWindow'. 
Vous pouvez utiliser les directives Arguments ou FactoryMethod pour construire ce type.'
```

## 🔍 Cause du Problème

Le problème venait du **conflit entre XAML et Dependency Injection** :

1. Dans `App.xaml`, nous avions `StartupUri="MainWindow.xaml"`
2. XAML essayait de créer `MainWindow` automatiquement avec un constructeur **sans paramètres**
3. Mais notre `MainWindow` avait un constructeur qui nécessitait `MainViewModel` (DI)
4. ❌ Résultat : Exception au démarrage

## ✅ Solution Appliquée

### 1. Modification de `App.xaml`

**AVANT :**
```xml
<Application x:Class="WPF.App"
             StartupUri="MainWindow.xaml">
```

**APRÈS :**
```xml
<Application x:Class="WPF.App">
```

➡️ **Suppression de `StartupUri`** car nous créons la fenêtre manuellement

---

### 2. Modification de `App.xaml.cs`

**AVANT :**
```csharp
// MainWindow enregistré dans DI
services.AddSingleton<MainWindow>();

// Dans OnStartup
var mainWindow = _host.Services.GetRequiredService<MainWindow>();
```

**APRÈS :**
```csharp
// MainWindow PAS enregistré dans DI (supprimé)

// Dans OnStartup - création manuelle avec DI
var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
var mainWindow = new MainWindow(mainViewModel);
mainWindow.Show();
```

➡️ **Création manuelle** de MainWindow avec injection du ViewModel

---

## 🎯 Pourquoi Cette Approche ?

### Pattern Utilisé
C'est le pattern **"Manual DI for Windows"** :

1. ✅ XAML ne tente pas de créer MainWindow
2. ✅ Nous contrôlons la création dans le code
3. ✅ Nous injectons les dépendances manuellement
4. ✅ ViewModels sont toujours résolus via DI

### Avantages
- ✅ Contrôle total sur la création de la fenêtre
- ✅ Gestion correcte du cycle de vie
- ✅ Pas de conflit XAML/DI
- ✅ Pattern standard en WPF avec DI

---

## 📝 Fichiers Modifiés

1. ✅ `App.xaml` - Suppression du StartupUri
2. ✅ `App.xaml.cs` - Création manuelle de MainWindow

---

## 🚀 Résultat

L'application compile maintenant **sans erreur** et peut démarrer correctement !

```powershell
# Lancer l'application
dotnet run

# Ou utiliser le script
cd ..
.\start-wpf.ps1
```

---

## 💡 Alternatives (Non utilisées)

Il existe d'autres approches, mais celle choisie est la plus simple :

### Alternative 1 : Arguments XAML
```xml
<Window xmlns:sys="clr-namespace:System;assembly=mscorlib">
    <Window.Arguments>
        <!-- Complexe et peu maintenable -->
    </Window.Arguments>
</Window>
```
❌ Trop complexe

### Alternative 2 : Constructeur sans paramètres
```csharp
public MainWindow()
{
    var viewModel = App.GetService<MainViewModel>();
    DataContext = viewModel;
}
```
❌ Service Locator anti-pattern

### Alternative 3 : Notre Solution ✅
```csharp
protected override async void OnStartup(StartupEventArgs e)
{
    var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
    var mainWindow = new MainWindow(mainViewModel);
    mainWindow.Show();
}
```
✅ **Simple, propre, maintenable**

---

## 📚 Pour en Savoir Plus

### Ressources
- [WPF Dependency Injection](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/data/data-binding-overview)
- [MVVM with DI in WPF](https://docs.microsoft.com/en-us/dotnet/architecture/modernize-desktop/example-migration-core)

### Pattern Similaire
C'est le même pattern utilisé dans :
- ASP.NET Core (Program.cs crée les controllers)
- MAUI (App.xaml.cs crée les pages)
- Avalonia UI (App.axaml.cs)

---

## ✅ Validation

Pour vérifier que tout fonctionne :

```powershell
# 1. Compiler
dotnet build

# 2. Pas d'erreur de compilation ✅

# 3. Lancer
dotnet run

# 4. La fenêtre s'ouvre ✅
```

---

**Le problème est maintenant résolu ! 🎉**
