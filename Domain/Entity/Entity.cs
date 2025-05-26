namespace Domain
{
    // Classe de base pour toutes les entités du domaine.
    public class Entity
    {
        // Identifiant unique de l'entité (clé primaire en base de données).
        // Id est par défaut l'Id en Base de donnée
        public int Id { get; set; }

        // Date de création de l'entité (utile pour l'audit et le suivi).
        public DateTime CreatedAt { get; set; }

        // Date de la dernière modification de l'entité.
        public DateTime UpdatedAt { get; set; }

        // Indique si l'entité est supprimée logiquement (soft delete).
        public bool IsDeleted { get; set; } = false;

        // Constructeur : initialise les dates à la création de l'entité.
        public Entity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Méthode à appeler lors d'une modification pour mettre à jour la date.
        public void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
