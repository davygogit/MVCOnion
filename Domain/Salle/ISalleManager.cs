using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface ISalleManager
    {
        Task<Salle> GetSalleByNumeroAsync(int numeroSalle);
        Task<Salle> GetSalleByNameAsync(string nomSalle);
        Task<List<Salle>> GetSalleByEtageidAsync(int IdEtage);

    }
}
