namespace OrderSystem;


public class OrderService : IOrderServiceTransient, IOrderServiceScoped, IOrderServiceSingleton
{
    private readonly Guid _instanceId;
    private readonly List<Order> _orders;

    public OrderService()
    {
        _instanceId = Guid.NewGuid();
        _orders = new List<Order>();
    }

    public void AddOrder(Order order)
    {
        _orders.Add(order);
    }

    public List<Order> GetOrders()
    {
        return _orders;
    }

    public Guid GetInstanceId()
    {
        return _instanceId;
    }

    public int GetOrdersCount()
    {
        return _orders.Count;
    }
}
