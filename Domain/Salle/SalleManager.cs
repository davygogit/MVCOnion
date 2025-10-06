using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Domain
{
    public class SalleManager : ISalleManager
    {

        private readonly IRepository<Salle> _salleRepository;


        public SalleManager(IRepository<Salle> salleRepository)
        {
            _salleRepository = salleRepository;
        }

        public async Task<List<Salle>> GetSalleByEtageidAsync(int IdEtage)
        {
            // Utilise FindAsync avec un prédicat pour filtrer par EtageId
            return await _salleRepository.FindAsync(s => s.EtageId == IdEtage);
        }

        public async Task<Salle> GetSalleByNameAsync(string nomSalle)
        {
            // Utilise GetQueryable pour ajouter Include et faire une requête complexe
            return await _salleRepository.GetQueryable()
                            .Include(s => s.Etage) // ← charge aussi l'étage lié
                            .FirstOrDefaultAsync(s => s.Nom == nomSalle);
        }

        public async Task<Salle> GetSalleByNumeroAsync(int numeroSalle)
        {
            // Utilise GetQueryable pour ajouter Include et faire une requête complexe
            return await _salleRepository.GetQueryable()
                            .Include(s => s.Etage) // ← charge aussi l'étage lié
                            .FirstOrDefaultAsync(s => s.Numero == numeroSalle);
        }
    }
}
