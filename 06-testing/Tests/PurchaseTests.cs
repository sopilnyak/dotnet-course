using NUnit.Framework;

namespace Testing.Tests;

public class PurchaseTests
{
    [Test]
    public void ValidateOrders_NewOrder_CanBeShipped()
    {
        var stubOrder = new FakeOrder();
        var purchase = new Purchase(stubOrder);

        // Тестируемый метод
        purchase.ValidateOrders();

        // FakeOrder ведет себя как stub
        Assert.True(purchase.CanBeShipped);

        // FakeOrder ведет себя как mock
        Assert.True(stubOrder.IsValidated);
    }
}
