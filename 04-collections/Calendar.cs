using System.Collections;

public class Activity
{
    public string Name { get; set; }

    public DateTime Timestamp { get; set; }
}

public class Calendar : IEnumerable
{
    private readonly Activity[] _activities;

    public Calendar(params Activity[] activities)
    {
        _activities = activities;
    }

    public IEnumerator GetEnumerator()
    {
        return new CalendarEnumerator(_activities);
    }
}

public class CalendarEnumerator : IEnumerator
{
    private readonly Activity[] _activities;
    private int _position = -1;

    public CalendarEnumerator(Activity[] activities)
    {
        _activities = activities
            .OrderBy(activity => activity.Timestamp)
            .ToArray();
    }

    public bool MoveNext()
    {
        if (_position < _activities.Length - 1)
        {
            _position++;
            return true;
        }
        return false;
    }

    public void Reset()
    {
        _position = -1;
    }

    public object Current
    {
        get
        {
            if (_position == -1 || _position >= _activities.Length)
                throw new ArgumentException();
            return _activities[_position];
        }
    }
}