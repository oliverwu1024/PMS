namespace PMS.Methods;

public class PaymentHistory
{
    public List<Payment> Payments = new List<Payment>();

    public void AddPayment(Payment payment)
    {
        Payments.Add(payment);
    }
}
