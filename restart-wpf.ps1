# Script pour fermer l'application WPF et recompiler
# Usage: .\restart-wpf.ps1

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "   Redémarrage Application WPF      " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Tuer les processus WPF en cours
Write-Host "🔍 Recherche des processus WPF en cours..." -ForegroundColor Yellow
$wpfProcesses = Get-Process | Where-Object {$_.ProcessName -like "*WPF*" -or $_.ProcessName -eq "WPF"}

if ($wpfProcesses) {
    Write-Host "✅ Processus WPF trouvés:" -ForegroundColor Green
    $wpfProcesses | ForEach-Object {
        Write-Host "   - $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Gray
    }
    
    Write-Host "🛑 Arrêt des processus WPF..." -ForegroundColor Yellow
    $wpfProcesses | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 1
    Write-Host "✅ Processus arrêtés" -ForegroundColor Green
} else {
    Write-Host "✅ Aucun processus WPF en cours" -ForegroundColor Green
}
Write-Host ""

# Naviguer vers le dossier WPF
if (-not (Test-Path "WPF\WPF.csproj")) {
    Write-Host "📁 Navigation vers le répertoire WPF..." -ForegroundColor Yellow
    if (Test-Path "WPF.csproj") {
        # Déjà dans le dossier WPF
    } else {
        Set-Location WPF
    }
}

# Nettoyer
Write-Host "🧹 Nettoyage du projet..." -ForegroundColor Yellow
dotnet clean --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Nettoyage réussi" -ForegroundColor Green
} else {
    Write-Host "⚠️  Erreur lors du nettoyage" -ForegroundColor Yellow
}
Write-Host ""

# Compiler
Write-Host "🔨 Compilation du projet..." -ForegroundColor Yellow
dotnet build --verbosity minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "❌ Erreur lors de la compilation" -ForegroundColor Red
    Write-Host "   Consultez les erreurs ci-dessus" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "💡 Essayez:" -ForegroundColor Cyan
    Write-Host "   - Vérifiez qu'aucune fenêtre WPF n'est ouverte" -ForegroundColor Gray
    Write-Host "   - Fermez Visual Studio si ouvert" -ForegroundColor Gray
    Write-Host "   - Exécutez à nouveau ce script" -ForegroundColor Gray
    exit 1
}
Write-Host "✅ Compilation réussie" -ForegroundColor Green
Write-Host ""

# Proposer de lancer
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "✅ Prêt à démarrer!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$response = Read-Host "Voulez-vous lancer l'application maintenant? (O/N)"
if ($response -eq "O" -or $response -eq "o" -or $response -eq "Y" -or $response -eq "y") {
    Write-Host ""
    Write-Host "🚀 Lancement de l'application..." -ForegroundColor Green
    Write-Host "   (Appuyez sur Ctrl+C pour arrêter)" -ForegroundColor Gray
    Write-Host ""
    
    dotnet run
} else {
    Write-Host ""
    Write-Host "✅ Pour lancer plus tard, utilisez:" -ForegroundColor Cyan
    Write-Host "   cd WPF" -ForegroundColor Gray
    Write-Host "   dotnet run" -ForegroundColor Gray
    Write-Host ""
}
