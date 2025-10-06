# 🔧 Guide de Dépannage - Application WPF

## ⚠️ Problème: "DefaultBinder est introuvable" ou Erreurs de Build

### Symptôme
- Message d'erreur: "DefaultBinder est introuvable"
- OU: "The process cannot access the file... because it is being used by another process"
- OU: "Impossible de copier... Le fichier est verrouillé"

### Cause
L'application WPF est **déjà en cours d'exécution** et verrouille les fichiers DLL.

---

## ✅ Solutions

### Solution 1: Fermer l'Application WPF
1. **Fermez complètement** la fenêtre de l'application WPF
2. Vérifiez qu'elle est bien fermée (regardez la barre des tâches)
3. Recompilez:
   ```powershell
   dotnet build
   ```

### Solution 2: Tuer le Processus WPF
```powershell
# Lister les processus WPF
Get-Process | Where-Object {$_.ProcessName -like "*WPF*"}

# Tuer tous les processus WPF
Get-Process | Where-Object {$_.ProcessName -like "*WPF*"} | Stop-Process -Force

# Ensuite recompiler
dotnet build
```

### Solution 3: Nettoyer et Recompiler
```powershell
# Fermer d'abord l'application WPF
# Ensuite:
dotnet clean
dotnet build
```

### Solution 4: Redémarrer Visual Studio
Si vous utilisez Visual Studio:
1. Fermez Visual Studio complètement
2. Rouvrez Visual Studio
3. Recompilez le projet

---

## 🔍 Vérification

Pour vérifier qu'aucun processus ne verrouille les fichiers:

```powershell
# Vérifier les processus WPF
Get-Process | Where-Object {$_.ProcessName -eq "WPF"}

# Si aucune sortie = OK, sinon fermez le processus
```

---

## 🚀 Relancer l'Application

Une fois les fichiers déverrouillés et la compilation réussie:

```powershell
# Méthode 1
dotnet run

# Méthode 2
.\bin\Debug\net9.0-windows\WPF.exe

# Méthode 3 (avec le script)
cd ..
.\start-wpf.ps1
```

---

## 📝 Bonnes Pratiques

### Pour Éviter ce Problème

1. **Toujours fermer** l'application avant de recompiler
2. **Ne pas lancer** plusieurs instances de l'application
3. **Utiliser Hot Reload** en développement (Visual Studio)
4. **Arrêter le débogage** avant de recompiler

### En Développement

Si vous développez et testez fréquemment:

**Option A - Visual Studio:**
- Utilisez **Hot Reload** (🔥 icône) pour appliquer les changements sans recompiler
- Utilisez **Shift+F5** pour arrêter le débogage
- Utilisez **F5** pour relancer

**Option B - Ligne de commande:**
```powershell
# Arrêtez l'app (Ctrl+C dans le terminal)
# Puis relancez:
dotnet run
```

---

## 🐛 Autres Erreurs Possibles

### "Cannot connect to database"
**Solution:** Vérifiez que SQL Server est démarré et que la chaîne de connexion est correcte dans `appsettings.json`

### "InitializeComponent does not exist"
**Solution:** C'est normal dans l'éditeur. Le projet compile correctement. Faites `dotnet build` pour vérifier.

### "Type... not found"
**Solution:** 
```powershell
dotnet restore
dotnet clean
dotnet build
```

---

## 💡 Astuce Rapide

**Commande tout-en-un** pour nettoyer et relancer:

```powershell
# Tuer les processus WPF, nettoyer et relancer
Get-Process | Where-Object {$_.ProcessName -like "*WPF*"} | Stop-Process -Force
dotnet clean
dotnet build
dotnet run
```

Copiez cette commande dans un fichier **restart-wpf.ps1** pour l'utiliser facilement !

---

## ✅ Résumé

L'erreur "DefaultBinder" ou les erreurs de verrouillage de fichiers signifient simplement que:
1. ✅ Votre application **fonctionne** (c'est pour ça qu'elle est en cours d'exécution!)
2. ✅ Il faut juste la **fermer** avant de recompiler
3. ✅ Ce n'est **pas une vraie erreur** dans votre code

**C'est un comportement normal de Windows!** 🎉
