# ✨ Fonctionnalité: Modification de Salle avec Fenêtre Modale

## 📅 Date d'Ajout
**6 Octobre 2025**

---

## 🎯 Description

Ajout d'une fonctionnalité complète permettant de **modifier une salle existante** via une **fenêtre modale dédiée** dans l'application WPF.

---

## 📦 Fichiers Créés

### 1. **WPF/ViewModels/EditSalleViewModel.cs** ✨ NOUVEAU
**Rôle** : ViewModel pour la modification de salle (MVVM pattern)

**Propriétés** :
```csharp
// Informations de base
public int SalleId { get; set; }
public string Nom { get; set; }
public int Numero { get; set; }
public int? NbPlaces { get; set; }
public int? NbTables { get; set; }
public string CoordonneeX { get; set; }
public string CoordonneeY { get; set; }
public string? ImagePath { get; set; }
public Etage? SelectedEtage { get; set; }
public string SelectedTypeSalle { get; set; } // Lecture seule en édition

// Salle Réunion
public bool Ecran { get; set; }
public bool Camera { get; set; }
public bool TableauBlanc { get; set; }
public bool SystemeAudio { get; set; }

// Salle Pause
public int MicroOndes { get; set; }
public int Evier { get; set; }
public bool Frigo { get; set; }
public bool Distributeur { get; set; }

// Salle Bubble
public bool PriseElectrique { get; set; }
```

**Commandes** :
- `SaveCommand` - Enregistre les modifications
- `CancelCommand` - Annule et ferme la fenêtre
- `SelectImageCommand` - Sélectionne une nouvelle image

**Événements** :
- `SaveCompleted` - Déclenché après sauvegarde réussie
- `CancelRequested` - Déclenché lors de l'annulation

**Logique métier** :
- ✅ Chargement des données de la salle existante
- ✅ Chargement de la liste des étages
- ✅ Détection automatique du type de salle (Reunion/Pause/Bubble)
- ✅ Affichage conditionnel des équipements selon le type
- ✅ Validation des champs obligatoires (Nom, Numéro, Étage)
- ✅ Mise à jour via `Repository.UpdateAsync()`
- ✅ Gestion d'erreurs avec DialogService

**Statistiques** : ~354 lignes

---

### 2. **WPF/Windows/EditSalleWindow.xaml** ✨ NOUVEAU
**Rôle** : Fenêtre modale pour l'édition de salle

**Caractéristiques** :
- 🪟 Fenêtre modale (`WindowStartupLocation="CenterOwner"`)
- 📏 Dimensions: 900x750 pixels
- 📜 ScrollViewer pour contenu long
- 🎨 Utilise les styles de `Resources/Styles.xaml`

**Sections** :
1. **Informations de base** (GroupBox)
   - Nom de la salle *
   - Numéro *
   - Étage *
   - Type de salle * (désactivé - non modifiable)
   - Nombre de places
   - Nombre de tables
   - Sélection d'image

2. **Coordonnées sur le plan** (GroupBox)
   - Coordonnée X
   - Coordonnée Y

3. **Équipements Salle Réunion** (GroupBox - conditionnel)
   - CheckBox: Écran, Caméra, Tableau blanc, Système audio

4. **Équipements Salle Pause** (GroupBox - conditionnel)
   - TextBox: Nombre micro-ondes, Nombre éviers
   - CheckBox: Frigo, Distributeur automatique

5. **Équipements Salle Bubble** (GroupBox - conditionnel)
   - CheckBox: Prises électriques

6. **Boutons d'action**
   - ❌ Annuler (SecondaryButtonStyle)
   - ✅ Enregistrer (PrimaryButtonStyle, désactivé pendant sauvegarde)

**Indicateurs visuels** :
- ⚠️ Message "Le type ne peut pas être modifié"
- ⏳ Indicateur "Enregistrement en cours..." (visible pendant sauvegarde)

**Statistiques** : ~180 lignes XAML

---

### 3. **WPF/Windows/EditSalleWindow.xaml.cs** ✨ NOUVEAU
**Rôle** : Code-behind pour la fenêtre modale

```csharp
public partial class EditSalleWindow : Window
{
    public EditSalleWindow(EditSalleViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Gestion des événements ViewModel → Fenêtre
        viewModel.SaveCompleted += (s, e) => { DialogResult = true; Close(); };
        viewModel.CancelRequested += (s, e) => { DialogResult = false; Close(); };
    }
}
```

**Logique** :
- ✅ Injection du ViewModel via constructeur
- ✅ DialogResult = true si sauvegarde réussie
- ✅ DialogResult = false si annulation
- ✅ Fermeture automatique de la fenêtre

**Statistiques** : ~20 lignes

---

## 📝 Fichiers Modifiés

### 1. **WPF/ViewModels/SalleListViewModel.cs** ✏️ MODIFIÉ
**Ajouts** :

#### Nouvelle Commande
```csharp
public ICommand EditSalleCommand { get; }
```

#### Initialisation
```csharp
EditSalleCommand = new RelayCommand<Salle>(EditSalle);
```

#### Méthode EditSalle
```csharp
private void EditSalle(Salle? salle)
{
    if (salle == null) return;

    try
    {
        // Obtenir IImageService via le service provider
        var imageService = App.GetService<IImageService>();

        // Créer le ViewModel pour l'édition
        var editViewModel = new EditSalleViewModel(
            _salleRepository,
            _etageRepository,
            _dialogService,
            imageService,
            salle
        );

        // Créer et afficher la fenêtre modale
        var editWindow = new Windows.EditSalleWindow(editViewModel)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        // Recharger les données si modification réussie
        if (editWindow.ShowDialog() == true)
        {
            _ = LoadDataAsync();
        }
    }
    catch (Exception ex)
    {
        _dialogService.ShowError("Erreur", $"Erreur lors de l'ouverture de l'éditeur: {ex.Message}");
    }
}
```

**Impact** : +30 lignes

---

### 2. **WPF/Views/SalleListView.xaml** ✏️ MODIFIÉ
**Ajout du bouton Modifier** :

```xaml
<!-- Actions -->
<StackPanel Grid.Row="2" Orientation="Horizontal" HorizontalAlignment="Right">
    <!-- ⭐ Favori -->
    <Button Content="⭐" Command="{Binding DataContext.ToggleFavoriCommand, ...}" />
    
    <!-- ✏️ NOUVEAU: Modifier -->
    <Button Content="✏️"
           Command="{Binding DataContext.EditSalleCommand, 
                    RelativeSource={RelativeSource AncestorType=UserControl}}"
           CommandParameter="{Binding}"
           Style="{StaticResource IconButtonStyle}"
           FontSize="18"
           ToolTip="Modifier"/>
    
    <!-- ℹ️ Détails -->
    <Button Content="ℹ️" Command="{Binding DataContext.ShowDetailsCommand, ...}" />
    
    <!-- 🗑️ Supprimer -->
    <Button Content="🗑️" Command="{Binding DataContext.DeleteSalleCommand, ...}" />
</StackPanel>
```

**Position** : Entre le bouton Favori et le bouton Détails

**Impact** : +10 lignes XAML

---

## 🎨 Fonctionnalités UX

### 1. **Fenêtre Modale Centrée**
- Ouvre au centre de la fenêtre principale
- Bloque l'interaction avec la fenêtre parente
- Taille adaptative avec ScrollViewer

### 2. **Validation en Temps Réel**
- Bouton "Enregistrer" désactivé si champs obligatoires vides
- Indication visuelle des champs requis (*)

### 3. **Type de Salle Non Modifiable**
- ComboBox désactivée (`IsEnabled="False"`)
- Message explicatif: ⚠️ "Le type ne peut pas être modifié"
- **Raison** : Changement de type = changement de classe (SalleReunion ↔ SallePause ↔ SalleBubble)

### 4. **Équipements Conditionnels**
- Affichage dynamique selon le type existant
- GroupBox Réunion visible si type = Reunion
- GroupBox Pause visible si type = Pause
- GroupBox Bubble visible si type = Bubble

### 5. **Indicateurs Visuels**
- ⏳ "Enregistrement en cours..." pendant la sauvegarde
- Bouton "Enregistrer" désactivé pendant l'opération
- Messages de succès/erreur via DialogService

### 6. **Sélection d'Image**
- Dialog natif Windows (`Microsoft.Win32.OpenFileDialog`)
- Filtre: *.png, *.jpg, *.jpeg
- Chemin affiché dans TextBox (lecture seule)

---

## 🔄 Flux d'Utilisation

### Scénario Nominal

1. **Ouverture**
   ```
   Utilisateur clique sur ✏️ dans la carte d'une salle
   → EditSalleViewModel créé avec la salle sélectionnée
   → EditSalleWindow s'ouvre en modal
   → Chargement des données existantes
   ```

2. **Édition**
   ```
   Utilisateur modifie le nom: "Salle A" → "Salle Alpha"
   Utilisateur change nombre de places: 8 → 12
   Utilisateur coche "Écran" pour une salle de réunion
   ```

3. **Sauvegarde**
   ```
   Utilisateur clique sur "✅ Enregistrer"
   → Validation des champs
   → Repository.UpdateAsync(salle)
   → Repository.SaveChangesAsync()
   → DialogService.ShowInformation("Succès", ...)
   → SaveCompleted event déclenché
   → Fenêtre fermée avec DialogResult = true
   → SalleListViewModel.LoadDataAsync() rechargé
   → Liste mise à jour avec nouvelles données
   ```

### Scénario Annulation

```
Utilisateur clique sur "❌ Annuler"
→ CancelRequested event déclenché
→ Fenêtre fermée avec DialogResult = false
→ Aucune modification enregistrée
```

### Scénario Erreur

```
Erreur lors de UpdateAsync (ex: violation contrainte BDD)
→ DialogService.ShowError("Erreur", message)
→ Fenêtre reste ouverte
→ Utilisateur peut corriger ou annuler
```

---

## 🛠️ Architecture Technique

### Pattern MVVM
```
┌─────────────────┐
│   View (XAML)   │  EditSalleWindow.xaml
│                 │  - Bindings bidirectionnels
│                 │  - Commandes
└────────┬────────┘
         │
         │ DataContext
         ▼
┌─────────────────┐
│   ViewModel     │  EditSalleViewModel.cs
│                 │  - Propriétés (Nom, Numero, etc.)
│                 │  - Commandes (Save, Cancel, SelectImage)
│                 │  - Événements (SaveCompleted, CancelRequested)
└────────┬────────┘
         │
         │ Dépendances injectées
         ▼
┌─────────────────┐
│   Services      │
│                 │  - IRepository<Salle>
│                 │  - IRepository<Etage>
│                 │  - IDialogService
│                 │  - IImageService
└─────────────────┘
```

### Injection de Dépendances
```csharp
// Dans SalleListViewModel.EditSalle()
var imageService = App.GetService<IImageService>();

var editViewModel = new EditSalleViewModel(
    _salleRepository,    // Réutilise le repo de SalleListViewModel
    _etageRepository,    // Réutilise le repo de SalleListViewModel
    _dialogService,      // Réutilise le service de SalleListViewModel
    imageService,        // Obtenu via DI
    salle                // Salle à modifier
);
```

---

## ✅ Tests de Compilation

### Build Réussi
```powershell
PS C:\WorkSpacesGitHub\MVCOnion> dotnet build WPF/WPF.csproj

Restauration terminée (0.3s)
  Domain a réussi (0.2s) → Domain\bin\Debug\net9.0\Domain.dll
  Infrastructure a réussi (0.0s) → Infrastructure\bin\Debug\net9.0\Infrastructure.dll
  WPF a réussi (0.1s) → WPF\bin\Debug\net9.0-windows\WPF.dll

Générer a réussi dans 1.0s
```

**Résultat** : ✅ **0 erreurs, 0 avertissements**

---

## 📊 Statistiques

| Métrique | Valeur |
|----------|--------|
| **Fichiers créés** | 3 |
| **Fichiers modifiés** | 2 |
| **Lignes ajoutées** | ~600 lignes |
| **Lignes ViewModel** | 354 |
| **Lignes XAML** | 180 |
| **Lignes Code-behind** | 20 |
| **Commandes ajoutées** | 3 (Save, Cancel, SelectImage) |
| **Propriétés ViewModel** | 18 |
| **Événements** | 2 |
| **Temps de compilation** | 1.0s |

---

## 🚀 Fonctionnalités Techniques

### 1. **Chargement Async des Données**
```csharp
private async Task LoadDataAsync()
{
    // Charger étages avec tri
    var etages = await _etageRepository.GetQueryable()
        .OrderBy(e => e.Niveau)
        .ToListAsync();
    
    // Charger salle avec relations
    var salle = await _salleRepository.GetQueryable()
        .Include(s => s.Etage)
        .FirstOrDefaultAsync(s => s.Id == _originalSalle.Id);
    
    LoadSalleData(salle);
}
```

### 2. **Mise à Jour Polymorphique**
```csharp
// Mise à jour selon le type de salle
if (salle is SalleReunion reunion)
{
    reunion.Ecran = Ecran;
    reunion.Camera = Camera;
    // ...
}
else if (salle is SallePause pause)
{
    pause.MicroOndes = MicroOndes;
    pause.Evier = Evier;
    // ...
}
else if (salle is SalleBubble bubble)
{
    bubble.PriseElectrique = PriseElectrique;
}

await _salleRepository.UpdateAsync(salle);
```

### 3. **Validation CanSave**
```csharp
private bool CanSave()
{
    return !string.IsNullOrWhiteSpace(Nom) &&
           Numero > 0 &&
           SelectedEtage != null &&
           !IsSaving;
}
```

### 4. **DialogResult Pattern**
```csharp
// Dans EditSalleWindow.xaml.cs
viewModel.SaveCompleted += (s, e) =>
{
    DialogResult = true;  // Succès
    Close();
};

// Dans SalleListViewModel.cs
if (editWindow.ShowDialog() == true)
{
    _ = LoadDataAsync();  // Recharger si succès
}
```

---

## 🎯 Cas d'Usage

### Cas 1: Modifier le nom d'une salle
**Scénario** : "Salle Réunion 3" → "Salle Alpha"
1. Clic sur ✏️
2. Modification du champ Nom
3. Clic sur Enregistrer
4. Confirmation "Salle modifiée avec succès"

### Cas 2: Changer l'étage d'une salle
**Scénario** : Déplacer une salle de l'étage 1 à l'étage 2
1. Clic sur ✏️
2. Sélection "Étage 2" dans ComboBox
3. Clic sur Enregistrer
4. Liste mise à jour avec nouvelle appartenance

### Cas 3: Ajouter des équipements
**Scénario** : Ajouter un écran à une salle de réunion
1. Clic sur ✏️
2. Cocher "Écran" dans section Équipements
3. Clic sur Enregistrer
4. Équipement persisté en BDD

### Cas 4: Modifier les coordonnées
**Scénario** : Repositionner une salle sur le plan d'étage
1. Clic sur ✏️
2. Modification CoordonneeX et CoordonneeY
3. Clic sur Enregistrer
4. Nouvelle position enregistrée

### Cas 5: Changer l'image
**Scénario** : Remplacer la photo d'une salle
1. Clic sur ✏️
2. Clic sur "📁 Parcourir"
3. Sélection nouvelle image
4. Clic sur Enregistrer
5. Nouveau chemin stocké dans ImgSallePath

---

## ⚠️ Limitations et Contraintes

### 1. **Type de Salle Non Modifiable**
**Raison** : Changement de type = changement de table BDD (TPH/TPT strategy)
**Solution actuelle** : ComboBox désactivée
**Solution future** : Implémentation d'une migration de type (DELETE + INSERT)

### 2. **Image Non Prévisualisée**
**État actuel** : Chemin affiché mais pas de preview
**Solution future** : Ajouter un contrôle Image avec binding sur ImagePath

### 3. **Validation Basique**
**État actuel** : Validation côté client uniquement
**Solution future** : Ajouter FluentValidation avec règles métier

### 4. **Pas de Confirmation de Fermeture**
**État actuel** : Fermeture immédiate sans confirmation si modifications non sauvegardées
**Solution future** : Tracker IsDirty et demander confirmation

---

## 📚 Documentation Associée

- [ARCHITECTURE.md](ARCHITECTURE.md) - Architecture Onion complète
- [IMPROVEMENTS.md](IMPROVEMENTS.md) - Roadmap des améliorations
- [REPOSITORY_IMPROVEMENTS_APPLIED.md](REPOSITORY_IMPROVEMENTS_APPLIED.md) - Pattern Repository v2.0

---

## 🎉 Conclusion

### ✅ Fonctionnalité Complète
- Fenêtre modale professionnelle
- Pattern MVVM respecté
- Validation en temps réel
- Gestion d'erreurs robuste
- Interface intuitive

### 📈 Qualité du Code
**Avant** : Pas de fonctionnalité d'édition
**Après** : Édition complète avec modal, validation, async/await

### 🚀 Prochaines Étapes
1. Ajouter preview d'image
2. Implémenter confirmation fermeture
3. Ajouter FluentValidation
4. Tests unitaires du ViewModel
5. Tests d'intégration

---

**Date** : 6 Octobre 2025  
**Version** : EditSalle v1.0  
**Statut** : ✅ IMPLÉMENTÉ et COMPILÉ  
**Auteur** : Équipe OnionWPF

**La modification de salles est maintenant disponible avec une interface professionnelle ! 🎨✨**
