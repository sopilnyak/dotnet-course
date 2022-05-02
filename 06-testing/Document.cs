namespace Testing
{
    public class Document
    {
        public string Title { get; init; }
        public DateTime CreatedUtc { get; init; }
        public string ClassName { get; init; }

        public Document(string title, DateTime date, string name)
        {
            Title = title;
            CreatedUtc = date;
            ClassName = name;
        }
    }
}