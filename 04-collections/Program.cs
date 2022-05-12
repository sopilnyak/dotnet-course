var calendar = new Calendar(
    new Activity { Name = "Sport", Timestamp = new DateTime(year: 2022, month: 3, day: 3) },
    new Activity { Name = "Music", Timestamp = new DateTime(year: 2022, month: 3, day: 1) },
    new Activity { Name = "Science", Timestamp = new DateTime(year: 2022, month: 3, day: 2) }
);

foreach (var item in calendar)
{
    Activity activity = (Activity)item;
    Console.WriteLine(activity.Name);
}