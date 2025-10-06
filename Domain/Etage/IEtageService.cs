using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// DTO pour la création d'un étage
    /// </summary>
    public class CreateEtageDto
    {
        public int Niveau { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? ImgPlanEtagePath { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'un étage
    /// </summary>
    public class UpdateEtageDto
    {
        public int Id { get; set; }
        public int Niveau { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string? ImgPlanEtagePath { get; set; }
    }

    /// <summary>
    /// Service métier pour la gestion des étages
    /// Centralise la logique métier et les règles de validation
    /// </summary>
    public interface IEtageService
    {
        /// <summary>
        /// Crée un nouvel étage avec validation métier
        /// </summary>
        /// <exception cref="ArgumentException">Si les données sont invalides</exception>
        /// <exception cref="InvalidOperationException">Si un étage avec ce niveau existe déjà</exception>
        Task<Etage> CreateEtageAsync(CreateEtageDto dto);

        /// <summary>
        /// Met à jour un étage existant
        /// </summary>
        /// <exception cref="KeyNotFoundException">Si l'étage n'existe pas</exception>
        /// <exception cref="InvalidOperationException">Si le nouveau niveau est déjà utilisé</exception>
        Task<Etage> UpdateEtageAsync(UpdateEtageDto dto);

        /// <summary>
        /// Supprime un étage (soft delete par défaut)
        /// </summary>
        /// <param name="id">Identifiant de l'étage</param>
        /// <param name="hardDelete">True pour suppression définitive</param>
        /// <exception cref="KeyNotFoundException">Si l'étage n'existe pas</exception>
        /// <exception cref="InvalidOperationException">Si l'étage contient des salles et hardDelete=true</exception>
        Task DeleteEtageAsync(int id, bool hardDelete = false);

        /// <summary>
        /// Récupère un étage par son identifiant
        /// </summary>
        Task<Etage?> GetEtageByIdAsync(int id, bool includeDeleted = false);

        /// <summary>
        /// Récupère tous les étages
        /// </summary>
        Task<List<Etage>> GetAllEtagesAsync(bool includeDeleted = false);

        /// <summary>
        /// Récupère tous les étages avec leurs salles
        /// </summary>
        Task<List<Etage>> GetEtagesWithSallesAsync(bool includeDeleted = false);

        /// <summary>
        /// Vérifie si un étage avec ce niveau existe déjà
        /// </summary>
        Task<bool> EtageExistsAsync(int niveau);

        /// <summary>
        /// Compte le nombre de salles dans un étage
        /// </summary>
        Task<int> CountSallesInEtageAsync(int etageId);
    }
}
