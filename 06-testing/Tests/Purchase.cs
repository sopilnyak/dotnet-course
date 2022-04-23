namespace Testing.Tests;

// Тестируемый класс
public class Purchase
{
    public bool CanBeShipped { get; private set; }

    private readonly IOrder _order;

    public Purchase(IOrder order)
    {
        _order = order;
    }

    public void ValidateOrders()
    {
        if (_order.IsValid())
        {
            CanBeShipped = true;
        }
    }
}
