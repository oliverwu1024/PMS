namespace PMS.Methods;

public class OrderHistory
{
    public List<Order> Orders = new List<Order>();

    public void AddOrder(Order order)
    {
        Orders.Add(order);
    }
}
