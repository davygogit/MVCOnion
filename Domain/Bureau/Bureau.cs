namespace Domain
{
    public class Bureau: Entity
    {
        public Bureau() { }
        public Bureau(string name, int number)
        {
            Name = name;
            Number = number;
        }
        public string Name { get; set; }
        public int Number { get; set; }
        public int? NombreDePlaces { get; set; }
        public int EtageId { get; set; }
        public Etage Etage { get; set; }
    }
}
