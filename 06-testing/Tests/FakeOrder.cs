namespace Testing.Tests;

public class FakeOrder : IOrder
{
    public bool IsValidated { get; private set; }

    public bool IsValid()
    {
        IsValidated = true;
        return true;
    }
}
