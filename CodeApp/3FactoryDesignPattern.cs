namespace CodeApp;

/// <summary>
/// Demonstrates the Factory Design Pattern in .NET 8.
/// 
/// Problem: We want to create different types of notifications (Email, SMS, Push)
/// without coupling the client code to specific classes.
/// 
/// Solution: Factory Pattern provides an interface for creating objects,
/// allowing subclasses to decide which class to instantiate.
/// 
/// Benefits:
/// - Loose coupling between client and concrete classes
/// - Single Responsibility: Object creation logic is centralized
/// - Open/Closed Principle: Easy to add new notification types
/// - Client doesn't need to know implementation details
/// </summary>
internal class FactoryDesignPattern
{
    /// <summary>
    /// Common interface for all notification types.
    /// </summary>
    public interface INotification
    {
        void NotifyUser(string message);
    }

    /// <summary>
    /// Concrete implementation for Email notifications.
    /// </summary>
    public sealed class EmailNotification : INotification
    {
        public void NotifyUser(string message)
        {
            Console.WriteLine($"📧 Email Notification: {message}");
            // In real implementation: Send email via SMTP, SendGrid, etc.
        }
    }

    /// <summary>
    /// Concrete implementation for SMS notifications.
    /// </summary>
    public sealed class SMSNotification : INotification
    {
        public void NotifyUser(string message)
        {
            Console.WriteLine($"📱 SMS Notification: {message}");
            // In real implementation: Send SMS via Twilio, AWS SNS, etc.
        }
    }

    /// <summary>
    /// Concrete implementation for Push notifications.
    /// </summary>
    public sealed class PushNotification : INotification
    {
        public void NotifyUser(string message)
        {
            Console.WriteLine($"🔔 Push Notification: {message}");
            // In real implementation: Send via Firebase, Azure Notification Hubs, etc.
        }
    }

    /// <summary>
    /// Factory interface defining the contract for creating notifications.
    /// </summary>
    public interface INotificationFactory
    {
        INotification CreateNotification();
    }

    /// <summary>
    /// Concrete factory for creating Email notifications.
    /// </summary>
    public sealed class EmailNotificationFactory : INotificationFactory
    {
        public INotification CreateNotification()
        {
            return new EmailNotification();
        }
    }

    /// <summary>
    /// Concrete factory for creating SMS notifications.
    /// </summary>
    public sealed class SMSNotificationFactory : INotificationFactory
    {
        public INotification CreateNotification()
        {
            return new SMSNotification();
        }
    }

    /// <summary>
    /// Concrete factory for creating Push notifications.
    /// </summary>
    public sealed class PushNotificationFactory : INotificationFactory
    {
        public INotification CreateNotification()
        {
            return new PushNotification();
        }
    }

    /// <summary>
    /// Notification type enumeration for simple factory pattern.
    /// </summary>
    public enum NotificationType
    {
        Email,
        SMS,
        Push
    }

    /// <summary>
    /// Simple Factory (alternative approach) that creates notifications based on type.
    /// This is a simpler variant when you don't need the full Factory Method pattern.
    /// </summary>
    public static class NotificationFactory
    {
        /// <summary>
        /// Creates a notification instance based on the specified type.
        /// </summary>
        /// <param name="type">The type of notification to create</param>
        /// <returns>An instance of the requested notification type</returns>
        /// <exception cref="ArgumentException">Thrown when an unsupported notification type is provided</exception>
        public static INotification CreateNotification(NotificationType type)
        {
            return type switch
            {
                NotificationType.Email => new EmailNotification(),
                NotificationType.SMS => new SMSNotification(),
                NotificationType.Push => new PushNotification(),
                _ => throw new ArgumentException($"Unsupported notification type: {type}", nameof(type))
            };
        }

        /// <summary>
        /// Creates a notification instance based on string type.
        /// </summary>
        public static INotification CreateNotification(string type)
        {
            return type.ToLowerInvariant() switch
            {
                "email" => new EmailNotification(),
                "sms" => new SMSNotification(),
                "push" => new PushNotification(),
                _ => throw new ArgumentException($"Unsupported notification type: {type}", nameof(type))
            };
        }
    }

    /// <summary>
    /// Client code that uses the factory pattern.
    /// Notice how the client doesn't depend on concrete notification classes.
    /// </summary>
    public static class NotificationService
    {
        /// <summary>
        /// Sends a notification using the Factory Method pattern.
        /// </summary>
        public static void SendNotification(INotificationFactory factory, string message)
        {
            // Client doesn't know which concrete notification is being created
            INotification notification = factory.CreateNotification();
            notification.NotifyUser(message);
        }

        /// <summary>
        /// Sends a notification using the Simple Factory pattern.
        /// </summary>
        public static void SendNotification(NotificationType type, string message)
        {
            INotification notification = NotificationFactory.CreateNotification(type);
            notification.NotifyUser(message);
        }
    }

    /// <summary>
    /// Demonstration of the Factory Design Pattern.
    /// </summary>
    public static void Demo()
    {
        Console.WriteLine("=== Factory Method Pattern Demo ===\n");

        // Approach 1: Using Factory Method Pattern with concrete factories
        Console.WriteLine("1. Factory Method Pattern:");
        INotificationFactory emailFactory = new EmailNotificationFactory();
        INotificationFactory smsFactory = new SMSNotificationFactory();
        INotificationFactory pushFactory = new PushNotificationFactory();

        NotificationService.SendNotification(emailFactory, "Your order has been shipped!");
        NotificationService.SendNotification(smsFactory, "Your OTP is 123456");
        NotificationService.SendNotification(pushFactory, "New message received");

        Console.WriteLine("\n2. Simple Factory Pattern:");
        // Approach 2: Using Simple Factory with enum
        NotificationService.SendNotification(NotificationType.Email, "Welcome to our service!");
        NotificationService.SendNotification(NotificationType.SMS, "Your appointment is tomorrow");
        NotificationService.SendNotification(NotificationType.Push, "Someone liked your post");

        Console.WriteLine("\n3. Simple Factory with String Type:");
        // Approach 3: Using Simple Factory with string type
        var emailNotification = NotificationFactory.CreateNotification("email");
        emailNotification.NotifyUser("Password reset successful");

        var smsNotification = NotificationFactory.CreateNotification("sms");
        smsNotification.NotifyUser("Your package will arrive today");

        var pushNotification = NotificationFactory.CreateNotification("push");
        pushNotification.NotifyUser("New follower!");

        Console.WriteLine("\n=== Demo Complete ===");
    }
}
