using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Etage : Entity
    {
        public int Niveau { get; set; }
        public string Nom { get; set; }
        public string? ImgPlanEtagePath { get; set; }
        public ICollection<Salle> Salles { get; set; } = new List<Salle>();

    }
}
