using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Salle : Entity
    {
        public string? Nom { get; set; }
        public int Numero { get; set; }


        public string? ImgSallePath { get; set; }
        public bool? Favori { get; set; }
        public TypeSalle TypeSalle { get; set; }
        
        // Coordonnées sur le plan de l'étage
        public string CoordonneeX { get; set; }
        public string CoordonneeY { get; set; }
        public int?  NbTables { get; set; }
        public int? NbPlaces { get; set; }

        public Etage Etage { get; set; }
        public int EtageId { get; set; }


    }
}

