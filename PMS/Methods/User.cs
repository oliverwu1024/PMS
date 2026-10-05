namespace PMS.Methods;

public class User
{
    public string UserName {get;set;}

    public int Age {get;set;}

    public int UserId {get;set;}

    public User(string userName, int age, int userId)
    {
        UserName = userName;
        Age = age;
        UserId =userId;
        OrderHistory = new OrderHistory();
    }

    public OrderHistory OrderHistory {get; set;}

    public List <Order> OrderList {get; set;}

    public PaymentHistory PaymentHistory {get; set;}

    public List <Payment> PaymentList {get; set;}


    public List <Order> MostExpensiveOrder()
    {
        decimal mostExpensive = OrderHistory.Orders.Where(order=>order.UserId == UserId).Max(order => order.GetTotalOrderPrice());
        return OrderHistory.Orders.Where(order=> order.UserId == UserId && order.GetTotalOrderPrice() == mostExpensive).ToList();
    }

    public Order MostRecentOrder()
    {
        return OrderHistory.Orders.Last();
    }
    public List <Payment> LeastExpensivePayment()
    {
        decimal leastExpensive = PaymentHistory.Payments.Where(payment=>payment.UserId == UserId).Min(payment => payment.PaymentAmount);
        return PaymentHistory.Payments.Where(payment=> payment.UserId == UserId && payment.PaymentAmount == leastExpensive).ToList();
    }   

    public List <Payment> PaymentGreaterThanTen()
    {

        return PaymentHistory.Payments.Where(payment=> payment.UserId == UserId && payment.PaymentAmount > 10).ToList();
    }   


    
}
