using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Extensions;

public class InitialData
{
    private static readonly Guid CustomerId1 = Guid.NewGuid();
    private static readonly Guid CustomerId2 = Guid.NewGuid();
    private static readonly Guid CustomerId3 = Guid.NewGuid();

    private static readonly Guid ProductId1 = Guid.NewGuid();
    private static readonly Guid ProductId2 = Guid.NewGuid();
    private static readonly Guid ProductId3 = Guid.NewGuid();
    private static readonly Guid ProductId4 = Guid.NewGuid();

    public static IEnumerable<Customer> Customers => new List<Customer>
    {
        Customer.Create(CustomerId.Of(CustomerId1), "John", "john@gmail.com"),
        Customer.Create(CustomerId.Of(CustomerId2), "Bob", "bob@gmail.com"),
        Customer.Create(CustomerId.Of(CustomerId3), "Alex", "alex@gmail.com"),
    };

    public static IEnumerable<Product> Products => new List<Product>
    {
        Product.Create(ProductId.Of(ProductId1), "iPhone 14", 499),
        Product.Create(ProductId.Of(ProductId2), "iPhone 15 Pro Max", 629),
        Product.Create(ProductId.Of(ProductId3), "Samsung Galaxy S23 Ultra", 529),
        Product.Create(ProductId.Of(ProductId4), "Samsung Galaxy S24 Ultra", 619),
    };

    public static IEnumerable<Order> Orders
    {
        get
        {
            var address1 = Address.Of("tom", "hary", "tom@gmail.com", "HaNam", "VN", "12/23", "0000");
            var address2 = Address.Of("bruce", "wayne", "bruce@gmail.com", "HaiDuong", "VN", "32/53", "0001");

            var payment1 = Payment.Of("tom", "23423452345234", "05/28", "123", 1);
            var payment2 = Payment.Of("bruce", "5555456456", "06/29", "122", 2);

            var order1 = Order.Create(
                OrderId.Of(Guid.NewGuid()),
                CustomerId.Of(CustomerId1),
                OrderName.Of("OD001"),
                shippingAddress: address1,
                billingAddress: address1,
                payment1);
            order1.AddItem(ProductId.Of(ProductId1), 2, 499);
            order1.AddItem(ProductId.Of(ProductId2), 1, 629);

            var order2 = Order.Create(
                OrderId.Of(Guid.NewGuid()),
                CustomerId.Of(CustomerId2),
                OrderName.Of("OD002"),
                shippingAddress: address2,
                billingAddress: address2,
                payment2);
            order2.AddItem(ProductId.Of(ProductId1), 1, 499);
            order2.AddItem(ProductId.Of(ProductId2), 3, 529);
            order2.AddItem(ProductId.Of(ProductId4), 2, 619);

            return new List<Order> { order1, order2 };
        }   
    }
}
