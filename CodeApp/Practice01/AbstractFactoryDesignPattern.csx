// AbstractFactoryDesignPattern.csx

interface IPayment
{
    void Pay();
}
interface IRefund
{
    void Refund();
}
interface IInvoice
{
    void GenerateInvoice();
}

class PaytmPayment : IPayment
{
    public void Pay()
    {
        Console.WriteLine("Paytm Payment executed");
    }
}

class PaytmRefund : IRefund
{
    public void Refund()
    {
        Console.WriteLine("Paytm Refund executed");
    }
}
class PaytmInvoice : IInvoice
{
    public void GenerateInvoice()
    {
        Console.WriteLine("Paytm Invoice generated");
    }
}

class RozarpayPayment : IPayment
{
    public void Pay()
    {
        Console.WriteLine("Rozarpay Payment executed");
    }
}

class RozarpayRefund : IRefund
{
    public void Refund()
    {
        Console.WriteLine("Rozarpay Refund executed");
    }
}

class RozarpayInvoice : IInvoice
{
    public void GenerateInvoice()
    {
        Console.WriteLine("Rozarpay Invoice generated");
    }
}

interface IPaymentFactory
{
    IPayment CreatePayment();
    IRefund CreateRefund();
    IInvoice CreateInvoice();
}

class PaytmFactory : IPaymentFactory
{
    public IPayment CreatePayment()
    {
        return new PaytmPayment();
    }
    public IRefund CreateRefund()
    {
        return new PaytmRefund();
    }
    public IInvoice CreateInvoice()
    {
        return new PaytmInvoice();
    }
}

class RozarpayFactory : IPaymentFactory
{
    public IPayment CreatePayment()
    {
        return new RozarpayPayment();
    }
    public IRefund CreateRefund()
    {
        return new RozarpayRefund();
    }
    public IInvoice CreateInvoice()
    {
        return new RozarpayInvoice();
    }
}

// Usage

IPaymentFactory paymentFactory = new PaytmFactory();
paymentFactory.CreatePayment().Pay();
paymentFactory.CreateRefund().Refund();
paymentFactory.CreateInvoice().GenerateInvoice();