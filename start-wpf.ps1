# Script de lancement de l'application WPF WebAppMaps
# Usage: .\start-wpf.ps1

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "    WebAppMaps - Application WPF    " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Vérifier que nous sommes dans le bon répertoire
if (-not (Test-Path "WPF\WPF.csproj")) {
    Write-Host "❌ Erreur: Ce script doit être exécuté depuis le répertoire racine du projet MVCOnion" -ForegroundColor Red
    Write-Host "   Répertoire actuel: $PWD" -ForegroundColor Yellow
    exit 1
}

Write-Host "📁 Répertoire du projet: $PWD" -ForegroundColor Green
Write-Host ""

# Vérifier que .NET 9.0 est installé
Write-Host "🔍 Vérification de .NET..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET n'est pas installé ou n'est pas dans le PATH" -ForegroundColor Red
    exit 1
}
Write-Host "✅ .NET version: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Vérifier la connexion à la base de données
Write-Host "🗄️  Vérification de la configuration de la base de données..." -ForegroundColor Yellow
$appsettingsPath = "WPF\appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettings = Get-Content $appsettingsPath | ConvertFrom-Json
    $connectionString = $appsettings.ConnectionStrings.DefaultConnection
    Write-Host "✅ Chaîne de connexion trouvée" -ForegroundColor Green
    Write-Host "   $connectionString" -ForegroundColor Gray
} else {
    Write-Host "⚠️  Fichier appsettings.json non trouvé" -ForegroundColor Yellow
}
Write-Host ""

# Restaurer les packages si nécessaire
Write-Host "📦 Restauration des packages NuGet..." -ForegroundColor Yellow
Set-Location WPF
dotnet restore --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erreur lors de la restauration des packages" -ForegroundColor Red
    Set-Location ..
    exit 1
}
Write-Host "✅ Packages restaurés avec succès" -ForegroundColor Green
Write-Host ""

# Compiler le projet
Write-Host "🔨 Compilation du projet WPF..." -ForegroundColor Yellow
dotnet build --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erreur lors de la compilation" -ForegroundColor Red
    Write-Host "   Essayez: dotnet build (sans --verbosity quiet) pour voir les erreurs" -ForegroundColor Yellow
    Set-Location ..
    exit 1
}
Write-Host "✅ Compilation réussie" -ForegroundColor Green
Write-Host ""

# Lancer l'application
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "🚀 Lancement de l'application..." -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Astuce: Appuyez sur Ctrl+C pour arrêter l'application" -ForegroundColor Yellow
Write-Host ""

dotnet run

Set-Location ..

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "❌ L'application s'est terminée avec des erreurs" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Application fermée normalement" -ForegroundColor Green
