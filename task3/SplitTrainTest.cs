/*
    Задача 2 (5 баллов)

    Пусть у нас есть класс, определяющий некоторый документ:

    public class Document
    {
        public string Title { get; init; }
        public DateTime CreatedUtc { get; init; }
        public string ClassName { get; init; }
    }

    Предлагается реализовать метод
    public (List<Document>, List<Document>) SplitTrainTest(List<Document> documents, double trainSize)
    который позволяет разбить выборку documents на две части — обучающую и тестовую — в пропорции, заданной параметром trainSize.
    Разбиение должно быть случайным и воспроизводимым независимо от порядка элементов в исходном массиве. Для воспроизводимости
    предлагается отсортировать массив по названию документа и дате его создания.
    В задаче предлагается максимально использовать методы LINQ.


    Мягкий дедлайн: 16.04.2022 23:59
    Жесткий дедлайн: 12.05.2022 23:59
*/

public class Document
{
    public string Title { get; }
    public DateTime CreatedUtc { get; }
    public string ClassName { get; }

    public Document(string title, DateTime time, string name)
    {
        this.Title = title;
        this.CreatedUtc = time;
        this.ClassName = name;
    }

    static public (List<Document> train, List<Document> test) SplitTrainTest(List<Document> documents, double trainSize)
    {
        var rnd = new Random(42);

        var marked =
        new List<(Document doc, bool is_train)>(from doc in documents
        orderby (doc.Title, doc.CreatedUtc)
        select (doc: doc, is_train: rnd.NextDouble() < trainSize));

        return (train: new List<Document>(from p in marked where p.is_train select p.doc),
        test: new List<Document>(from p in marked where !p.is_train select p.doc));
    }
}
