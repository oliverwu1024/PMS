using PMS.Enums;

namespace PMS.Methods;

public class Payment
{
    public int PaymentId {get;set;}

    public int UserId {get;set;}

    public PaymentStatus PaymentStatus {get;set;}

    public decimal PaymentAmount {get; set;}

    public PaymentMethod PaymentMethod {get; set;}


    public Payment(int userId, int paymentId, decimal paymentAmount, PaymentMethod paymentMethod, PaymentHistory paymentHistory)
    {
        UserId = userId;
        PaymentId = paymentId;
        PaymentAmount=paymentAmount;
        PaymentMethod=paymentMethod;
        paymentHistory.AddPayment(this);
        PaymentStatus = PaymentStatus.Pending;  
    }





}
