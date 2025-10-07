using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    /// <summary>
    /// Implémentation du service métier pour la gestion des Salles
    /// </summary>
    public class SalleService : ISalleService
    {
        private readonly IRepository<Salle> _salleRepository;
        private readonly IRepository<Etage> _etageRepository;

        public SalleService(
            IRepository<Salle> salleRepository,
            IRepository<Etage> etageRepository)
        {
            _salleRepository = salleRepository ?? throw new ArgumentNullException(nameof(salleRepository));
            _etageRepository = etageRepository ?? throw new ArgumentNullException(nameof(etageRepository));
        }

        #region CREATE

        public async Task<Salle> CreateSalleAsync(CreateSalleDto dto)
        {
            // Validation du DTO
            ValidateCreateDto(dto);

            // Règle métier 1: Le numéro de salle doit être unique
            if (await SalleNumeroExistsAsync(dto.Numero))
            {
                throw new InvalidOperationException($"Une salle avec le numéro {dto.Numero} existe déjà.");
            }

            // Règle métier 2: L'étage doit exister
            if (!await EtageExistsAsync(dto.EtageId))
            {
                throw new InvalidOperationException($"L'étage avec l'ID {dto.EtageId} n'existe pas.");
            }

            // Règle métier 3: Validation spécifique selon le type de salle
            ValidateTypeSpecificRules(dto.TypeSalle, dto.NbPlaces, dto.NbTables);

            // Création de l'entité Salle selon le type
            Salle salle = dto.TypeSalle switch
            {
                TypeSalle.Reunion => new SalleReunion
                {
                    Nom = dto.Nom?.Trim(),
                    Numero = dto.Numero,
                    ImgSallePath = dto.ImgSallePath,
                    Favori = dto.Favori ?? false,
                    TypeSalle = dto.TypeSalle,
                    CoordonneeX = dto.CoordonneeX ?? "0",
                    CoordonneeY = dto.CoordonneeY ?? "0",
                    NbTables = dto.NbTables,
                    NbPlaces = dto.NbPlaces,
                    EtageId = dto.EtageId,
                    Ecran = dto.Ecran ?? false,
                    Camera = dto.Camera ?? false,
                    TableauBlanc = dto.TableauBlanc ?? false,
                    SystemeAudio = dto.SystemeAudio ?? false
                },
                TypeSalle.Pause => new SallePause
                {
                    Nom = dto.Nom?.Trim(),
                    Numero = dto.Numero,
                    ImgSallePath = dto.ImgSallePath,
                    Favori = dto.Favori ?? false,
                    TypeSalle = dto.TypeSalle,
                    CoordonneeX = dto.CoordonneeX ?? "0",
                    CoordonneeY = dto.CoordonneeY ?? "0",
                    NbTables = dto.NbTables,
                    NbPlaces = dto.NbPlaces,
                    EtageId = dto.EtageId,
                    MicroOndes = dto.MicroOndes ?? 0,
                    Frigo = dto.Frigo ?? false,
                    Evier = dto.Evier ?? 0,
                    Distributeur = dto.Distributeur ?? false
                },
                TypeSalle.Bubble => new SalleBubble
                {
                    Nom = dto.Nom?.Trim(),
                    Numero = dto.Numero,
                    ImgSallePath = dto.ImgSallePath,
                    Favori = dto.Favori ?? false,
                    TypeSalle = dto.TypeSalle,
                    CoordonneeX = dto.CoordonneeX ?? "0",
                    CoordonneeY = dto.CoordonneeY ?? "0",
                    NbTables = dto.NbTables,
                    NbPlaces = dto.NbPlaces,
                    EtageId = dto.EtageId,
                    PriseElectrique = dto.PriseElectrique ?? false
                },
                _ => new Salle
                {
                    Nom = dto.Nom?.Trim(),
                    Numero = dto.Numero,
                    ImgSallePath = dto.ImgSallePath,
                    Favori = dto.Favori ?? false,
                    TypeSalle = dto.TypeSalle,
                    CoordonneeX = dto.CoordonneeX ?? "0",
                    CoordonneeY = dto.CoordonneeY ?? "0",
                    NbTables = dto.NbTables,
                    NbPlaces = dto.NbPlaces,
                    EtageId = dto.EtageId
                }
            };

            await _salleRepository.AddAsync(salle);
            await _salleRepository.SaveChangesAsync();

            return salle;
        }

        #endregion

        #region READ

        public async Task<Salle?> GetSalleByIdAsync(int id, bool includeEtage = false, bool includeDeleted = false)
        {
            if (includeEtage)
            {
                return await _salleRepository.GetQueryable()
                    .Include(s => s.Etage)
                    .FirstOrDefaultAsync(s => s.Id == id && (includeDeleted || !s.IsDeleted));
            }

            return await _salleRepository.GetByIdAsync(id, includeDeleted);
        }

        public async Task<Salle?> GetSalleByNumeroAsync(int numero, bool includeEtage = true)
        {
            if (includeEtage)
            {
                return await _salleRepository.GetQueryable()
                    .Include(s => s.Etage)
                    .FirstOrDefaultAsync(s => s.Numero == numero && !s.IsDeleted);
            }

            return await _salleRepository.FindAsync(s => s.Numero == numero)
                .ContinueWith(task => task.Result.FirstOrDefault());
        }

        public async Task<Salle?> GetSalleByNameAsync(string nom, bool includeEtage = true)
        {
            if (string.IsNullOrWhiteSpace(nom))
            {
                throw new ArgumentException("Le nom ne peut pas être vide.", nameof(nom));
            }

            if (includeEtage)
            {
                return await _salleRepository.GetQueryable()
                    .Include(s => s.Etage)
                    .FirstOrDefaultAsync(s => s.Nom == nom && !s.IsDeleted);
            }

            return await _salleRepository.FindAsync(s => s.Nom == nom)
                .ContinueWith(task => task.Result.FirstOrDefault());
        }

        public async Task<List<Salle>> GetAllSallesAsync(bool includeEtage = false, bool includeDeleted = false)
        {
            var query = _salleRepository.GetQueryable();

            if (includeEtage)
            {
                query = query.Include(s => s.Etage);
            }

            if (!includeDeleted)
            {
                query = query.Where(s => !s.IsDeleted);
            }

            // Tri automatique par numéro de salle
            query = query.OrderBy(s => s.Numero);

            return await query.ToListAsync();
        }

        public async Task<List<Salle>> GetSallesByEtageIdAsync(int etageId, bool includeEtage = false)
        {
            var query = _salleRepository.GetQueryable()
                .Where(s => s.EtageId == etageId && !s.IsDeleted);

            if (includeEtage)
            {
                query = query.Include(s => s.Etage);
            }

            // Tri automatique par numéro de salle
            query = query.OrderBy(s => s.Numero);

            return await query.ToListAsync();
        }

        public async Task<List<Salle>> GetFavorisSallesAsync(bool includeEtage = true)
        {
            var query = _salleRepository.GetQueryable()
                .Where(s => s.Favori == true && !s.IsDeleted);

            if (includeEtage)
            {
                query = query.Include(s => s.Etage);
            }

            // Tri automatique par numéro de salle
            query = query.OrderBy(s => s.Numero);

            return await query.ToListAsync();
        }

        public async Task<List<Salle>> GetSallesByTypeAsync(TypeSalle typeSalle, bool includeEtage = false)
        {
            var query = _salleRepository.GetQueryable()
                .Where(s => s.TypeSalle == typeSalle && !s.IsDeleted);

            if (includeEtage)
            {
                query = query.Include(s => s.Etage);
            }

            // Tri automatique par numéro de salle
            query = query.OrderBy(s => s.Numero);

            return await query.ToListAsync();
        }

        #endregion

        #region UPDATE

        public async Task<Salle> UpdateSalleAsync(UpdateSalleDto dto)
        {
            // Validation du DTO
            ValidateUpdateDto(dto);

            // Récupération de la salle existante
            var salle = await _salleRepository.GetByIdAsync(dto.Id);
            if (salle == null)
            {
                throw new KeyNotFoundException($"La salle avec l'ID {dto.Id} n'existe pas.");
            }

            // Règle métier 1: Le numéro de salle doit être unique (sauf pour la salle actuelle)
            if (await SalleNumeroExistsAsync(dto.Numero, dto.Id))
            {
                throw new InvalidOperationException($"Une autre salle avec le numéro {dto.Numero} existe déjà.");
            }

            // Règle métier 2: L'étage doit exister
            if (!await EtageExistsAsync(dto.EtageId))
            {
                throw new InvalidOperationException($"L'étage avec l'ID {dto.EtageId} n'existe pas.");
            }

            // Règle métier 3: Validation spécifique selon le type de salle
            ValidateTypeSpecificRules(dto.TypeSalle, dto.NbPlaces, dto.NbTables);

            // Mise à jour des propriétés communes
            salle.Nom = dto.Nom?.Trim();
            salle.Numero = dto.Numero;
            salle.ImgSallePath = dto.ImgSallePath;
            salle.Favori = dto.Favori;
            salle.TypeSalle = dto.TypeSalle;
            salle.CoordonneeX = dto.CoordonneeX ?? "0";
            salle.CoordonneeY = dto.CoordonneeY ?? "0";
            salle.NbTables = dto.NbTables;
            salle.NbPlaces = dto.NbPlaces;
            salle.EtageId = dto.EtageId;

            // Mise à jour des propriétés spécifiques selon le type
            if (salle is SalleReunion reunion)
            {
                reunion.Ecran = dto.Ecran ?? false;
                reunion.Camera = dto.Camera ?? false;
                reunion.TableauBlanc = dto.TableauBlanc ?? false;
                reunion.SystemeAudio = dto.SystemeAudio ?? false;
            }
            else if (salle is SallePause pause)
            {
                pause.MicroOndes = dto.MicroOndes ?? 0;
                pause.Frigo = dto.Frigo ?? false;
                pause.Evier = dto.Evier ?? 0;
                pause.Distributeur = dto.Distributeur ?? false;
            }
            else if (salle is SalleBubble bubble)
            {
                bubble.PriseElectrique = dto.PriseElectrique ?? false;
            }

            await _salleRepository.UpdateAsync(salle);
            await _salleRepository.SaveChangesAsync();

            return salle;
        }

        public async Task<bool> ToggleFavoriAsync(int salleId)
        {
            var salle = await _salleRepository.GetByIdAsync(salleId);
            if (salle == null)
            {
                throw new KeyNotFoundException($"La salle avec l'ID {salleId} n'existe pas.");
            }

            salle.Favori = !salle.Favori;
            await _salleRepository.UpdateAsync(salle);
            await _salleRepository.SaveChangesAsync();

            return salle.Favori ?? false;
        }

        #endregion

        #region DELETE

        public async Task DeleteSalleAsync(int id, bool hardDelete = false)
        {
            var salle = await _salleRepository.GetByIdAsync(id, includeDeleted: true);
            if (salle == null)
            {
                throw new KeyNotFoundException($"La salle avec l'ID {id} n'existe pas.");
            }

            await _salleRepository.DeleteAsync(id, hardDelete);
            await _salleRepository.SaveChangesAsync();
        }

        #endregion

        #region VALIDATION & BUSINESS RULES

        public async Task<bool> SalleNumeroExistsAsync(int numero, int? excludeSalleId = null)
        {
            var query = _salleRepository.GetQueryable()
                .Where(s => s.Numero == numero && !s.IsDeleted);

            if (excludeSalleId.HasValue)
            {
                query = query.Where(s => s.Id != excludeSalleId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> EtageExistsAsync(int etageId)
        {
            var etage = await _etageRepository.GetByIdAsync(etageId);
            return etage != null;
        }

        public async Task<int> CountSallesInEtageAsync(int etageId)
        {
            return await _salleRepository.GetQueryable()
                .CountAsync(s => s.EtageId == etageId && !s.IsDeleted);
        }

        #endregion

        #region PRIVATE VALIDATION METHODS

        /// <summary>
        /// Valide les données de création d'une salle
        /// </summary>
        private void ValidateCreateDto(CreateSalleDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Les données de création ne peuvent pas être nulles.");
            }

            // Validation du nom (optionnel mais si présent, entre 2 et 100 caractères)
            if (!string.IsNullOrWhiteSpace(dto.Nom))
            {
                if (dto.Nom.Length < 2)
                {
                    throw new ArgumentException("Le nom de la salle doit contenir au moins 2 caractères.", nameof(dto.Nom));
                }
                if (dto.Nom.Length > 100)
                {
                    throw new ArgumentException("Le nom de la salle ne peut pas dépasser 100 caractères.", nameof(dto.Nom));
                }
            }

            // Validation du numéro (doit être positif)
            if (dto.Numero <= 0)
            {
                throw new ArgumentException("Le numéro de salle doit être positif.", nameof(dto.Numero));
            }

            // Validation des coordonnées
            ValidateCoordinates(dto.CoordonneeX, nameof(dto.CoordonneeX));
            ValidateCoordinates(dto.CoordonneeY, nameof(dto.CoordonneeY));

            // Validation du nombre de places (si présent, doit être positif)
            if (dto.NbPlaces.HasValue && dto.NbPlaces.Value < 0)
            {
                throw new ArgumentException("Le nombre de places ne peut pas être négatif.", nameof(dto.NbPlaces));
            }

            // Validation du nombre de tables (si présent, doit être positif)
            if (dto.NbTables.HasValue && dto.NbTables.Value < 0)
            {
                throw new ArgumentException("Le nombre de tables ne peut pas être négatif.", nameof(dto.NbTables));
            }
        }

        /// <summary>
        /// Valide les données de mise à jour d'une salle
        /// </summary>
        private void ValidateUpdateDto(UpdateSalleDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Les données de mise à jour ne peuvent pas être nulles.");
            }

            if (dto.Id <= 0)
            {
                throw new ArgumentException("L'ID de la salle doit être positif.", nameof(dto.Id));
            }

            // Même validation que pour la création
            if (!string.IsNullOrWhiteSpace(dto.Nom))
            {
                if (dto.Nom.Length < 2)
                {
                    throw new ArgumentException("Le nom de la salle doit contenir au moins 2 caractères.", nameof(dto.Nom));
                }
                if (dto.Nom.Length > 100)
                {
                    throw new ArgumentException("Le nom de la salle ne peut pas dépasser 100 caractères.", nameof(dto.Nom));
                }
            }

            if (dto.Numero <= 0)
            {
                throw new ArgumentException("Le numéro de salle doit être positif.", nameof(dto.Numero));
            }

            ValidateCoordinates(dto.CoordonneeX, nameof(dto.CoordonneeX));
            ValidateCoordinates(dto.CoordonneeY, nameof(dto.CoordonneeY));

            if (dto.NbPlaces.HasValue && dto.NbPlaces.Value < 0)
            {
                throw new ArgumentException("Le nombre de places ne peut pas être négatif.", nameof(dto.NbPlaces));
            }

            if (dto.NbTables.HasValue && dto.NbTables.Value < 0)
            {
                throw new ArgumentException("Le nombre de tables ne peut pas être négatif.", nameof(dto.NbTables));
            }
        }

        /// <summary>
        /// Valide une coordonnée (doit être un nombre valide)
        /// </summary>
        private void ValidateCoordinates(string? coordinate, string paramName)
        {
            if (string.IsNullOrWhiteSpace(coordinate))
            {
                return; // Coordonnée vide acceptée, sera définie à "0" par défaut
            }

            // Vérifie si c'est un nombre valide (int ou decimal)
            if (!int.TryParse(coordinate, out _) && !decimal.TryParse(coordinate, out _))
            {
                throw new ArgumentException($"La coordonnée '{coordinate}' n'est pas un nombre valide.", paramName);
            }
        }

        /// <summary>
        /// Valide les règles spécifiques selon le type de salle
        /// </summary>
        private void ValidateTypeSpecificRules(TypeSalle typeSalle, int? nbPlaces, int? nbTables)
        {
            switch (typeSalle)
            {
                case TypeSalle.Reunion:
                    // Une salle de réunion doit avoir au moins 2 places
                    if (nbPlaces.HasValue && nbPlaces.Value < 2)
                    {
                        throw new InvalidOperationException("Une salle de réunion doit avoir au moins 2 places.");
                    }
                    // Une salle de réunion doit avoir au moins 1 table
                    if (nbTables.HasValue && nbTables.Value < 1)
                    {
                        throw new InvalidOperationException("Une salle de réunion doit avoir au moins 1 table.");
                    }
                    break;

                case TypeSalle.Bubble:
                    // Une salle Bubble est typiquement pour 1 personne
                    if (nbPlaces.HasValue && nbPlaces.Value > 2)
                    {
                        throw new InvalidOperationException("Une salle Bubble est conçue pour 1-2 personnes maximum.");
                    }
                    break;

                case TypeSalle.Pause:
                    // Pas de règle spécifique stricte pour les salles de pause
                    break;

                default:
                    throw new ArgumentException($"Type de salle inconnu: {typeSalle}");
            }
        }

        #endregion
    }
}
