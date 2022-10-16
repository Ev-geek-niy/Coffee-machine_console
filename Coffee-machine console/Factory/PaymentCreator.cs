using Coffee_machine_console.PaymentMethods;

namespace Coffee_machine_console.Factory;

public class PaymentCreator
{
    public Payment CreatePaymentMethod(string method)
    {
        switch (method)
        {
            case "cash":
                return new CashPayment();
            case "cashless":
                return new CashlessPayment();
        }

        return null;
    }
}