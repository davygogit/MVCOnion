namespace Domain
{
    public class Etage : Entity
    {
        public Etage() { }
        public Etage(string name, int number)
        {
            Name = name;
            Number = number;
        }
        public string Name { get; set; }
        public int Number { get; set; }
        public ICollection<Bureau> bureaux { get; set; } = new List<Bureau>();
    }
}
