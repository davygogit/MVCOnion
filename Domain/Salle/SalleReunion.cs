using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SalleReunion : Salle
    {

        public bool Ecran { get; set; }
        public bool Camera { get; set; }
        public bool TableauBlanc { get; set; }
        public bool SystemeAudio { get; set; }
    }
} 