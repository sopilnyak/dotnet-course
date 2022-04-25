namespace OOP
{
    public class Person : IComparable
    {
        public string Name { get; init; }
        public int Age { get; init; }
        public int CompareTo(object? o)
        {
            if (o is Person person) return Age.CompareTo(person.Age);
            else throw new ArgumentException("Некорректное значение параметра");
        }
    }
}
