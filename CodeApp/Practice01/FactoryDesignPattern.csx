// FactoryDesignPattern.csx

interface INotification
{
    void Send(string message);
}

class EmailNotification : INotification
{
    public void Send(string message)
    {
        Console.WriteLine($"Email Notification: {message}");
    }
}

class SMSNotification : INotification
{
    public void Send(string message)
    {
        Console.WriteLine($"SMS Notification: {message}");
    }
}

interface INotificationFactory
{
    INotification CreateNotification();
}

class EmailNotificationFactory : INotificationFactory
{
    public INotification CreateNotification()
    {
        return new EmailNotification();
    }
}

class SMSNotificationFactory : INotificationFactory
{
    public INotification CreateNotification()
    {
        return new SMSNotification();
    }
}
// dotnet script Practice01\FactoryDesignPattern.csx
INotificationFactory notificationFactory = new EmailNotificationFactory();
notificationFactory.CreateNotification().Send("Hello via Email!");

INotificationFactory smsFactory= new SMSNotificationFactory();
smsFactory.CreateNotification().Send("Hello via SMS!");

