using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    /// <summary>
    /// Implémentation du service métier pour la gestion des étages
    /// Contient toute la logique métier, validation et règles business
    /// </summary>
    public class EtageService : IEtageService
    {
        private readonly IRepository<Etage> _etageRepository;
        private readonly IRepository<Salle> _salleRepository;

        public EtageService(
            IRepository<Etage> etageRepository,
            IRepository<Salle> salleRepository)
        {
            _etageRepository = etageRepository ?? throw new ArgumentNullException(nameof(etageRepository));
            _salleRepository = salleRepository ?? throw new ArgumentNullException(nameof(salleRepository));
        }

        #region CREATE

        public async Task<Etage> CreateEtageAsync(CreateEtageDto dto)
        {
            // Validation des données
            ValidateCreateDto(dto);

            // Règle métier : Vérifier si un étage avec ce niveau existe déjà
            if (await EtageExistsAsync(dto.Niveau))
            {
                throw new InvalidOperationException(
                    $"Un étage de niveau {dto.Niveau} existe déjà. Veuillez choisir un autre niveau.");
            }

            // Règle métier : Le nom du RDC doit être cohérent
            if (dto.Niveau == 0 && !dto.Nom.Contains("RDC", StringComparison.OrdinalIgnoreCase) 
                              && !dto.Nom.Contains("Rez", StringComparison.OrdinalIgnoreCase))
            {
                // Avertissement mais pas bloquant
                dto.Nom = $"{dto.Nom} (RDC)";
            }

            // Création de l'entité
            var etage = new Etage
            {
                Niveau = dto.Niveau,
                Nom = dto.Nom.Trim(),
                ImgPlanEtagePath = dto.ImgPlanEtagePath
            };

            // Persistance
            await _etageRepository.AddAsync(etage);
            await _etageRepository.SaveChangesAsync();

            return etage;
        }

        #endregion

        #region UPDATE

        public async Task<Etage> UpdateEtageAsync(UpdateEtageDto dto)
        {
            // Validation des données
            ValidateUpdateDto(dto);

            // Récupération de l'étage existant
            var etage = await _etageRepository.GetByIdAsync(dto.Id);
            if (etage == null)
            {
                throw new KeyNotFoundException($"L'étage avec l'ID {dto.Id} est introuvable.");
            }

            // Règle métier : Vérifier si le nouveau niveau est déjà utilisé par un autre étage
            if (etage.Niveau != dto.Niveau)
            {
                var existingEtages = await _etageRepository
                    .FindAsync(e => e.Niveau == dto.Niveau && e.Id != dto.Id);

                if (existingEtages.Any())
                {
                    throw new InvalidOperationException(
                        $"Un autre étage utilise déjà le niveau {dto.Niveau}. " +
                        $"Impossible de modifier le niveau de cet étage.");
                }
            }

            // Mise à jour des propriétés
            etage.Niveau = dto.Niveau;
            etage.Nom = dto.Nom.Trim();
            etage.ImgPlanEtagePath = dto.ImgPlanEtagePath;

            await _etageRepository.UpdateAsync(etage);
            await _etageRepository.SaveChangesAsync();

            return etage;
        }

        #endregion

        #region DELETE

        public async Task DeleteEtageAsync(int id, bool hardDelete = false)
        {
            // Vérifier que l'étage existe
            var etage = await _etageRepository.GetByIdAsync(id, includeDeleted: true);
            if (etage == null)
            {
                throw new KeyNotFoundException($"L'étage avec l'ID {id} est introuvable.");
            }

            // Règle métier : Vérifier si l'étage contient des salles
            var sallesCount = await CountSallesInEtageAsync(id);

            if (sallesCount > 0)
            {
                if (hardDelete)
                {
                    throw new InvalidOperationException(
                        $"Impossible de supprimer définitivement l'étage '{etage.Nom}' " +
                        $"car il contient {sallesCount} salle(s). " +
                        $"Veuillez d'abord supprimer ou déplacer les salles.");
                }
                else
                {
                    // Soft delete autorisé même avec des salles
                    // Les salles seront marquées comme "orphelines" mais restent accessibles
                }
            }

            // Suppression (soft ou hard)
            await _etageRepository.DeleteAsync(id, hardDelete);
            await _etageRepository.SaveChangesAsync();
        }

        #endregion

        #region READ

        public async Task<Etage?> GetEtageByIdAsync(int id, bool includeDeleted = false)
        {
            return await _etageRepository.GetByIdAsync(id, includeDeleted);
        }

        public async Task<List<Etage>> GetAllEtagesAsync(bool includeDeleted = false)
        {
            var etages = await _etageRepository.GetAllAsync(includeDeleted);
            
            // Tri par niveau croissant
            return etages.OrderBy(e => e.Niveau).ToList();
        }

        public async Task<List<Etage>> GetEtagesWithSallesAsync(bool includeDeleted = false)
        {
            return await _etageRepository.GetQueryable(includeDeleted)
                .Include(e => e.Salles)
                .OrderBy(e => e.Niveau)
                .ToListAsync();
        }

        public async Task<bool> EtageExistsAsync(int niveau)
        {
            var etages = await _etageRepository.FindAsync(e => e.Niveau == niveau);
            return etages.Any();
        }

        public async Task<int> CountSallesInEtageAsync(int etageId)
        {
            var salles = await _salleRepository.FindAsync(s => s.EtageId == etageId);
            return salles.Count;
        }

        #endregion

        #region VALIDATION

        private void ValidateCreateDto(CreateEtageDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Nom))
                throw new ArgumentException("Le nom de l'étage est obligatoire.", nameof(dto.Nom));

            if (dto.Nom.Length < 2)
                throw new ArgumentException("Le nom de l'étage doit contenir au moins 2 caractères.", nameof(dto.Nom));

            if (dto.Nom.Length > 100)
                throw new ArgumentException("Le nom de l'étage ne peut pas dépasser 100 caractères.", nameof(dto.Nom));

            // Validation du niveau (généralement entre -2 et 50)
            if (dto.Niveau < -2)
                throw new ArgumentException("Le niveau ne peut pas être inférieur à -2 (sous-sols).", nameof(dto.Niveau));

            if (dto.Niveau > 50)
                throw new ArgumentException("Le niveau ne peut pas dépasser 50.", nameof(dto.Niveau));
        }

        private void ValidateUpdateDto(UpdateEtageDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Id <= 0)
                throw new ArgumentException("L'identifiant de l'étage est invalide.", nameof(dto.Id));

            if (string.IsNullOrWhiteSpace(dto.Nom))
                throw new ArgumentException("Le nom de l'étage est obligatoire.", nameof(dto.Nom));

            if (dto.Nom.Length < 2)
                throw new ArgumentException("Le nom de l'étage doit contenir au moins 2 caractères.", nameof(dto.Nom));

            if (dto.Nom.Length > 100)
                throw new ArgumentException("Le nom de l'étage ne peut pas dépasser 100 caractères.", nameof(dto.Nom));

            if (dto.Niveau < -2)
                throw new ArgumentException("Le niveau ne peut pas être inférieur à -2 (sous-sols).", nameof(dto.Niveau));

            if (dto.Niveau > 50)
                throw new ArgumentException("Le niveau ne peut pas dépasser 50.", nameof(dto.Niveau));
        }

        #endregion
    }
}
