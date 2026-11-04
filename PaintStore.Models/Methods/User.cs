using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PaintStore.Models.Methods;

public class User: IdentityUser<int>
{   
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    public int Age {get;set;}


    private User() {}

    public User(string userName, string userEmail, string userPhone,  int age, int userId)
    {
        UserName = userName;
        Email = userEmail;
        PhoneNumber = userPhone;
        Age = age;
        Id =userId;
        OrderHistory = new OrderHistory();
    }

    public OrderHistory OrderHistory {get; set;}

    public List <Order> OrderList {get; set;}

    public PaymentHistory PaymentHistory {get; set;}

    public List <Payment> PaymentList {get; set;}


    public List <Order> MostExpensiveOrder()
    {
        decimal mostExpensive = OrderHistory.Orders.Where(order=>order.UserId == Id).Max(order => order.GetTotalOrderPrice());
        return OrderHistory.Orders.Where(order=> order.UserId == Id && order.GetTotalOrderPrice() == mostExpensive).ToList();
    }

    public Order MostRecentOrder()
    {
        return OrderHistory.Orders.Last();
    }
    public List <Payment> LeastExpensivePayment()
    {
        decimal leastExpensive = PaymentHistory.Payments.Where(payment=>payment.UserId == Id).Min(payment => payment.PaymentAmount);
        return PaymentHistory.Payments.Where(payment=> payment.UserId == Id && payment.PaymentAmount == leastExpensive).ToList();
    }   

    public List <Payment> PaymentGreaterThanTen()
    {

        return PaymentHistory.Payments.Where(payment=> payment.UserId == Id && payment.PaymentAmount > 10).ToList();
    }   


    
}
