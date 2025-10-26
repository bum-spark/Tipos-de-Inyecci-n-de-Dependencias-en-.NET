namespace OrderSystem;

public class Order
{
    public string Articulo { get; set; }
    public int Cantidad { get; set; }
}

public interface IOrderServiceTransient
{
    public void AddOrder(Order order);
    public List<Order> GetOrders();
    public Guid GetInstanceId();
    public int GetOrdersCount();
}

public interface IOrderServiceScoped
{
    public void AddOrder(Order order);
    public List<Order> GetOrders();
    public Guid GetInstanceId();
    public int GetOrdersCount();
}

public interface IOrderServiceSingleton
{
    public void AddOrder(Order order);
    public List<Order> GetOrders();
    public Guid GetInstanceId();
    public int GetOrdersCount();
}
