using Domain;

namespace Web.ViewModel
{
    public class SalleViewModel
    {
        public ICollection<Salle> salles { get; set; } = new List<Salle>();


        public ICollection<Etage> etages { get; set; }

    }
}
