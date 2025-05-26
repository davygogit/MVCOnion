using Microsoft.EntityFrameworkCore;
using Domain;

namespace Infrastructure
{
    // Le contexte de base de données principal de l'application.
    // Hérite de DbContext (Entity Framework Core) pour gérer l'accès aux données.
    public class MVCOnionContext : DbContext
    {
        // Constructeur recevant les options de configuration du contexte (ex : chaîne de connexion).
        // Utilisé par l'injection de dépendances dans ASP.NET Core.
        public MVCOnionContext(DbContextOptions<MVCOnionContext> options) : base(options)
        {
        }

        // Représente la table des étages dans la base de données.
        // Permet de faire des requêtes, ajouts, modifications et suppressions sur les entités Etage.
        public DbSet<Etage> Etages { get; set; }

        // Représente la table des bureaux dans la base de données.
        // Permet de manipuler les entités Bureau de façon similaire.
        public DbSet<Bureau> Bureaux { get; set; }
    }
}
