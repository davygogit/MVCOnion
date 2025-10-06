# 🚀 Démarrage Rapide - Application WPF

## ⚡ Lancer l'Application en 3 Étapes

### 1️⃣ Vérifier la Base de Données

```powershell
# Assurez-vous que SQL Server est démarré et accessible
# La chaîne de connexion est dans : WPF\appsettings.json
```

Si vous n'avez pas encore créé la base de données :
```powershell
cd Infrastructure
dotnet ef database update
```

### 2️⃣ Lancer l'Application

**Option A - Ligne de commande :**
```powershell
cd WPF
dotnet run
```

**Option B - Visual Studio :**
1. Ouvrir `WebAppMaps.sln`
2. Clic droit sur **WPF** → **Définir comme projet de démarrage**
3. Appuyer sur **F5**

### 3️⃣ Utiliser l'Application

L'interface s'ouvre avec 3 boutons principaux :

#### 🔍 Rechercher Salles
- Tapez un nom ou numéro dans la barre de recherche
- Filtrez par étage ou type de salle
- Cliquez sur ⭐ pour marquer en favori
- Cliquez sur ℹ️ pour voir les détails
- Cliquez sur 🗑️ pour supprimer

#### ➕ Créer Salle
- Remplissez le nom et numéro
- Sélectionnez l'étage
- Choisissez le type (Réunion/Pause/Bubble)
- Ajoutez une image (optionnel)
- Remplissez les équipements selon le type
- Cliquez sur "Créer la salle"

#### 🏢 Créer Étage
- Entrez le nom (ex: "Rez-de-chaussée")
- Entrez le niveau (ex: 0)
- Ajoutez un plan d'étage (optionnel)
- Cliquez sur "Créer l'étage"

---

## 🎯 Fonctionnalités Disponibles

### ✅ Recherche et Filtres
- 🔍 Recherche par nom ou numéro
- 📊 Filtre par étage
- 🏷️ Filtre par type
- ⭐ Système de favoris
- 🔄 Actualisation en temps réel

### ✅ Gestion des Salles
- ➕ Créer des salles
- 📝 3 types : Réunion, Pause, Bubble
- 🖼️ Upload d'images
- 📍 Coordonnées sur plan
- 🗑️ Suppression

### ✅ Gestion des Étages
- ➕ Créer des étages
- 🗺️ Upload de plans
- 📊 Vue d'ensemble

---

## ⚙️ Configuration

### Modifier la Connexion à la Base de Données

Éditez `WPF\appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=VOTRE_SERVEUR;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**Exemples de chaînes de connexion :**

```json
// SQL Server local avec Windows Authentication
"Server=localhost;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"

// SQL Server avec utilisateur/mot de passe
"Server=localhost;Database=WebAppMaps;User Id=sa;Password=VotreMotDePasse;TrustServerCertificate=true;"

// SQL Server Express
"Server=localhost\\SQLEXPRESS;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
```

---

## 🐛 Résolution de Problèmes

### ❌ Erreur XamlParseException (Constructeur introuvable)

**Problème :** "Aucun constructeur correspondant n'a été trouvé sur le type 'WPF.MainWindow'"

**Solution :** ✅ **CORRIGÉ !** Ce problème a été résolu dans le code. Si vous le rencontrez :
```powershell
dotnet clean
dotnet build
```

Consultez `WPF/FIX_XAMLPARSE.md` pour les détails.

### ❌ Erreur de connexion à la base de données

**Problème :** "Cannot open database..."

**Solution :**
1. Vérifiez que SQL Server est démarré
2. Vérifiez la chaîne de connexion dans `appsettings.json`
3. Créez la base de données :
   ```powershell
   cd Infrastructure
   dotnet ef database update
   ```

### ❌ L'application ne démarre pas

**Problème :** Erreur au démarrage

**Solution :**
1. Vérifiez que vous avez .NET 9.0 installé :
   ```powershell
   dotnet --version
   ```
2. Restaurez les packages :
   ```powershell
   cd WPF
   dotnet restore
   ```
3. Recompilez :
   ```powershell
   dotnet build
   ```

### ❌ Les images ne s'affichent pas

**Problème :** Pas d'images dans les cartes

**Solution :**
- Les dossiers `assets/Salles/` et `assets/PlansEtages/` doivent exister
- Les images doivent être copiées dans le dossier de sortie
- Le binding des images sera implémenté dans une future version

### ❌ Erreurs de compilation dans l'IDE

**Problème :** "InitializeComponent does not exist"

**Solution :**
- Ces erreurs sont normales dans l'éditeur
- Le projet compile correctement avec `dotnet build`
- Visual Studio régénère ces méthodes à la compilation

---

## 📚 Documentation Complète

Pour plus d'informations, consultez :

- 📖 **TRANSFORMATION_GUIDE.md** - Guide complet de la transformation
- 📖 **WPF/README.md** - Documentation détaillée du projet WPF
- 📖 **README.md** - Documentation du projet global

---

## 💡 Astuces

### Raccourcis Utiles
- **F5** - Démarrer avec débogage (Visual Studio)
- **Ctrl+F5** - Démarrer sans débogage (Visual Studio)
- **Ctrl+Shift+B** - Compiler (Visual Studio)

### Développement
1. Utilisez le **XAML Designer** pour prévisualiser l'UI
2. Utilisez le **Live Visual Tree** pour déboguer le binding
3. Activez **Just My Code** pour un débogage plus facile

### Performance
- L'application utilise des requêtes asynchrones
- Les filtres s'appliquent en temps réel
- Le rechargement des données est optimisé

---

## 🎉 C'est Tout !

Votre application WPF est prête à l'emploi !

**Commande rapide :**
```powershell
cd c:\WorkSpacesGitHub\MVCOnion\WPF
dotnet run
```

Profitez de votre nouvelle application desktop ! 🚀✨
