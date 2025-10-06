# 🗄️ Configuration de la Base de Données - Guide Complet

## ❌ Erreur: "Le serveur n'est pas trouvé"

Cette erreur signifie que l'application ne peut pas se connecter à SQL Server.

---

## 🔍 Diagnostic Rapide

### Étape 1: Vérifier SQL Server

Exécutez cette commande pour voir quelles instances SQL Server sont installées :

```powershell
# Option 1: Via les services
Get-Service -Name "MSSQL*" -ErrorAction SilentlyContinue

# Option 2: Via sqlcmd (si disponible)
sqlcmd -L
```

**Résultats possibles :**
- ✅ `MSSQLSERVER` (running) = Instance par défaut installée
- ✅ `MSSQL$SQLEXPRESS` (running) = SQL Server Express installé
- ❌ Aucun résultat = SQL Server n'est pas installé

---

## 🛠️ Solutions par Cas

### 📌 CAS 1: Vous avez SQL Server LocalDB (Recommandé pour le développement)

**LocalDB** est inclus avec Visual Studio et est idéal pour le développement.

#### Configuration
Le fichier `appsettings.json` est déjà configuré pour LocalDB :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### Créer la base de données
```powershell
cd c:\WorkSpacesGitHub\MVCOnion\Infrastructure
dotnet ef database update
```

#### Tester
```powershell
cd ..\WPF
dotnet run
```

✅ **C'est la solution la plus simple !**

---

### 📌 CAS 2: Vous avez SQL Server Express

SQL Server Express utilise une instance nommée `SQLEXPRESS`.

#### Configuration
Modifiez `appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**OU** (syntaxe alternative) :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=WebAppMaps;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

#### Vérifier que le service est démarré
```powershell
# Vérifier le statut
Get-Service -Name "MSSQL`$SQLEXPRESS"

# Démarrer si arrêté
Start-Service -Name "MSSQL`$SQLEXPRESS"
```

#### Créer la base de données
```powershell
cd c:\WorkSpacesGitHub\MVCOnion\Infrastructure
dotnet ef database update
```

---

### 📌 CAS 3: Vous avez SQL Server (Instance par défaut)

Si vous avez SQL Server installé avec l'instance par défaut.

#### Configuration
Modifiez `appsettings.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### Vérifier que le service est démarré
```powershell
# Vérifier le statut
Get-Service -Name "MSSQLSERVER"

# Démarrer si arrêté
Start-Service -Name "MSSQLSERVER"
```

---

### 📌 CAS 4: Vous n'avez PAS SQL Server

Vous devez installer une version de SQL Server.

#### Option A: Installer SQL Server LocalDB (RECOMMANDÉ)

**Plus simple et léger, parfait pour le développement.**

1. **Télécharger :**
   - Avec Visual Studio : Déjà inclus
   - Sinon : https://go.microsoft.com/fwlink/?linkid=866658

2. **Installer :**
   - Double-cliquez sur le fichier téléchargé
   - Suivez l'assistant d'installation

3. **Vérifier l'installation :**
   ```powershell
   sqllocaldb info
   ```

4. **Configurer l'application :**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

#### Option B: Installer SQL Server Express

**Plus complet, avec SQL Server Management Studio.**

1. **Télécharger :**
   - https://www.microsoft.com/sql-server/sql-server-downloads
   - Choisir "Express" (gratuit)

2. **Installer :**
   - Choisir "Installation de base"
   - Suivez l'assistant

3. **Configurer l'application :**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
     }
   }
   ```

---

## 🔧 Configurations Avancées

### Avec Identifiants (Login SQL Server)

Si vous utilisez un login SQL Server au lieu de Windows Authentication :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WebAppMaps;User Id=sa;Password=VotreMotDePasse;TrustServerCertificate=true;"
  }
}
```

### Serveur Distant

Si SQL Server est sur une autre machine :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=192.168.1.100;Database=WebAppMaps;User Id=utilisateur;Password=motdepasse;TrustServerCertificate=true;"
  }
}
```

### Instance Nommée Personnalisée

Si vous avez une instance avec un nom spécifique :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\NomInstance;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

---

## 📋 Procédure Complète - Démarrage Depuis Zéro

### 1. Installer SQL Server LocalDB (si nécessaire)

```powershell
# Vérifier si LocalDB est installé
sqllocaldb info

# Si erreur, installer LocalDB
# Télécharger depuis: https://go.microsoft.com/fwlink/?linkid=866658
```

### 2. Configurer appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 3. Créer la base de données

```powershell
cd c:\WorkSpacesGitHub\MVCOnion\Infrastructure
dotnet ef database update
```

**Résultat attendu :**
```
Build started...
Build succeeded.
Applying migration '20250627151537_init_fin'.
Done.
```

### 4. Lancer l'application

```powershell
cd ..\WPF
dotnet run
```

✅ **L'application devrait démarrer sans erreur !**

---

## 🐛 Dépannage

### Erreur: "A network-related or instance-specific error"

**Cause :** SQL Server n'est pas démarré ou pas accessible.

**Solutions :**
1. Vérifier que le service est démarré :
   ```powershell
   Get-Service -Name "MSSQL*"
   ```

2. Démarrer le service si nécessaire :
   ```powershell
   Start-Service -Name "MSSQLSERVER"  # ou MSSQL$SQLEXPRESS
   ```

3. Vérifier le nom de l'instance dans appsettings.json

### Erreur: "Login failed for user"

**Cause :** Problème d'authentification.

**Solutions :**
1. Si vous utilisez Windows Authentication, vérifiez que `Trusted_Connection=true` ou `Integrated Security=true` est dans la chaîne
2. Si vous utilisez SQL Authentication, vérifiez le login et mot de passe
3. Vérifiez que l'utilisateur a les droits sur la base de données

### Erreur: "Cannot open database 'WebAppMaps'"

**Cause :** La base de données n'existe pas.

**Solution :**
```powershell
cd c:\WorkSpacesGitHub\MVCOnion\Infrastructure
dotnet ef database update
```

---

## 💡 Recommandations

### Pour le Développement
✅ **SQL Server LocalDB** - Léger, simple, intégré à Visual Studio

### Pour la Production (interne)
✅ **SQL Server Express** - Gratuit, complet, avec management tools

### Pour la Production (critique)
✅ **SQL Server Standard/Enterprise** - Performances maximales

---

## 🎯 Configuration Rapide (TL;DR)

**Solution la plus simple pour commencer :**

```powershell
# 1. Modifier appsettings.json pour utiliser LocalDB
# (déjà fait par défaut)

# 2. Créer la base de données
cd c:\WorkSpacesGitHub\MVCOnion\Infrastructure
dotnet ef database update

# 3. Lancer l'application
cd ..\WPF
dotnet run
```

**Si LocalDB n'est pas installé :**
- Télécharger : https://go.microsoft.com/fwlink/?linkid=866658
- Installer
- Relancer les commandes ci-dessus

---

## 📞 Besoin d'Aide ?

### Vérifier votre configuration actuelle

```powershell
# Voir les instances SQL Server
sqllocaldb info

# Tester la connexion avec sqlcmd (si installé)
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT @@VERSION"
```

### Logs de l'application

L'application affiche des messages d'erreur détaillés dans la console.
Regardez les messages pour identifier le problème exact.

---

**Votre base de données devrait maintenant être configurée correctement ! 🎉**
