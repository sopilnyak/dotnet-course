using NUnit.Framework;

namespace Testing.Tests;

public class DateTimeTests
{
    [Test]
    public void Add_AddWeek_CorrectDate()
    {
        // Arrange: подготовка объектов
        var date = new DateTime(2022, 3, 24);
        var weekSpan = new TimeSpan(7, 0, 0, 0);

        // Act: действие над объектами
        var datePlusWeek = date.Add(weekSpan);

        // Assert: проверка результата
        Assert.AreEqual(new DateTime(2022, 3, 30), datePlusWeek);
    }

    [TestCase(2022, 3, 24, "2022-03-24")]
    [TestCase(1970, 1, 1, "1970-01-01")]
    public void ToString_ArbitraryDate_CorrectString(int year, int month, int day, string expected)
    {
        var date = new DateTime(year, month, day);
        var str = date.ToString("yyyy-MM-dd");

        Assert.AreEqual(expected, str);
    }

    [TestCase("2022-02-31")]
    public void Parse_InvalidDate_FormatException(string dateString)
    {
        Assert.Throws<FormatException>(() => DateTime.Parse(dateString));
    }
}
