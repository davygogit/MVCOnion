using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Service métier pour la gestion des Salles.
    /// Centralise toute la logique métier liée aux salles (validation, règles métier, etc.)
    /// </summary>
    public interface ISalleService
    {
        #region CREATE

        /// <summary>
        /// Crée une nouvelle salle avec validation des règles métier
        /// </summary>
        /// <param name="dto">Données de création de la salle</param>
        /// <returns>La salle créée</returns>
        /// <exception cref="ArgumentException">Si les données sont invalides</exception>
        /// <exception cref="InvalidOperationException">Si une règle métier est violée (numéro déjà existant, étage inexistant, etc.)</exception>
        Task<Salle> CreateSalleAsync(CreateSalleDto dto);

        #endregion

        #region READ

        /// <summary>
        /// Récupère une salle par son ID
        /// </summary>
        /// <param name="id">ID de la salle</param>
        /// <param name="includeEtage">Inclure l'étage dans la requête</param>
        /// <param name="includeDeleted">Inclure les salles soft-deleted</param>
        /// <returns>La salle trouvée ou null</returns>
        Task<Salle?> GetSalleByIdAsync(int id, bool includeEtage = false, bool includeDeleted = false);

        /// <summary>
        /// Récupère une salle par son numéro
        /// </summary>
        /// <param name="numero">Numéro de la salle</param>
        /// <param name="includeEtage">Inclure l'étage dans la requête</param>
        /// <returns>La salle trouvée ou null</returns>
        Task<Salle?> GetSalleByNumeroAsync(int numero, bool includeEtage = true);

        /// <summary>
        /// Récupère une salle par son nom
        /// </summary>
        /// <param name="nom">Nom de la salle</param>
        /// <param name="includeEtage">Inclure l'étage dans la requête</param>
        /// <returns>La salle trouvée ou null</returns>
        Task<Salle?> GetSalleByNameAsync(string nom, bool includeEtage = true);

        /// <summary>
        /// Récupère toutes les salles
        /// </summary>
        /// <param name="includeEtage">Inclure l'étage dans la requête</param>
        /// <param name="includeDeleted">Inclure les salles soft-deleted</param>
        /// <returns>Liste de toutes les salles</returns>
        Task<List<Salle>> GetAllSallesAsync(bool includeEtage = false, bool includeDeleted = false);

        /// <summary>
        /// Récupère toutes les salles d'un étage donné
        /// </summary>
        /// <param name="etageId">ID de l'étage</param>
        /// <param name="includeEtage">Inclure l'étage dans la requête</param>
        /// <returns>Liste des salles de l'étage</returns>
        Task<List<Salle>> GetSallesByEtageIdAsync(int etageId, bool includeEtage = false);

        /// <summary>
        /// Récupère toutes les salles favorites
        /// </summary>
        /// <param name="includeEtage">Inclure l'étage dans la requête</param>
        /// <returns>Liste des salles favorites</returns>
        Task<List<Salle>> GetFavorisSallesAsync(bool includeEtage = true);

        /// <summary>
        /// Récupère toutes les salles d'un type donné
        /// </summary>
        /// <param name="typeSalle">Type de salle (Reunion, Pause, Bubble)</param>
        /// <param name="includeEtage">Inclure l'étage dans la requête</param>
        /// <returns>Liste des salles du type spécifié</returns>
        Task<List<Salle>> GetSallesByTypeAsync(TypeSalle typeSalle, bool includeEtage = false);

        #endregion

        #region UPDATE

        /// <summary>
        /// Met à jour une salle existante avec validation
        /// </summary>
        /// <param name="dto">Données de mise à jour</param>
        /// <returns>La salle mise à jour</returns>
        /// <exception cref="KeyNotFoundException">Si la salle n'existe pas</exception>
        /// <exception cref="ArgumentException">Si les données sont invalides</exception>
        /// <exception cref="InvalidOperationException">Si une règle métier est violée</exception>
        Task<Salle> UpdateSalleAsync(UpdateSalleDto dto);

        /// <summary>
        /// Bascule le statut favori d'une salle
        /// </summary>
        /// <param name="salleId">ID de la salle</param>
        /// <returns>Le nouveau statut favori</returns>
        /// <exception cref="KeyNotFoundException">Si la salle n'existe pas</exception>
        Task<bool> ToggleFavoriAsync(int salleId);

        #endregion

        #region DELETE

        /// <summary>
        /// Supprime une salle (soft delete par défaut)
        /// </summary>
        /// <param name="id">ID de la salle à supprimer</param>
        /// <param name="hardDelete">True pour supprimer définitivement, False pour soft delete (par défaut)</param>
        /// <exception cref="KeyNotFoundException">Si la salle n'existe pas</exception>
        Task DeleteSalleAsync(int id, bool hardDelete = false);

        #endregion

        #region VALIDATION & BUSINESS RULES

        /// <summary>
        /// Vérifie si une salle avec le numéro donné existe déjà
        /// </summary>
        /// <param name="numero">Numéro de la salle</param>
        /// <param name="excludeSalleId">ID de salle à exclure (pour mise à jour)</param>
        /// <returns>True si le numéro existe déjà, False sinon</returns>
        Task<bool> SalleNumeroExistsAsync(int numero, int? excludeSalleId = null);

        /// <summary>
        /// Vérifie si un étage existe
        /// </summary>
        /// <param name="etageId">ID de l'étage</param>
        /// <returns>True si l'étage existe, False sinon</returns>
        Task<bool> EtageExistsAsync(int etageId);

        /// <summary>
        /// Compte le nombre de salles dans un étage donné
        /// </summary>
        /// <param name="etageId">ID de l'étage</param>
        /// <returns>Nombre de salles dans l'étage</returns>
        Task<int> CountSallesInEtageAsync(int etageId);

        #endregion
    }

    #region DTOs

    /// <summary>
    /// DTO pour la création d'une salle
    /// </summary>
    public class CreateSalleDto
    {
        public string? Nom { get; set; }
        public int Numero { get; set; }
        public string? ImgSallePath { get; set; }
        public bool? Favori { get; set; }
        public TypeSalle TypeSalle { get; set; }
        public string CoordonneeX { get; set; } = "0";
        public string CoordonneeY { get; set; } = "0";
        public int? NbTables { get; set; }
        public int? NbPlaces { get; set; }
        public int EtageId { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'une salle
    /// </summary>
    public class UpdateSalleDto
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public int Numero { get; set; }
        public string? ImgSallePath { get; set; }
        public bool? Favori { get; set; }
        public TypeSalle TypeSalle { get; set; }
        public string CoordonneeX { get; set; } = "0";
        public string CoordonneeY { get; set; } = "0";
        public int? NbTables { get; set; }
        public int? NbPlaces { get; set; }
        public int EtageId { get; set; }
    }

    #endregion
}
