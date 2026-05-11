namespace CodeApp;

/// <summary>
/// Demonstrates the Decorator Design Pattern in .NET 8.
/// 
/// Problem: We want to add additional responsibilities to objects dynamically
/// without modifying the original class or using inheritance for every combination.
/// 
/// Solution: Decorator Pattern allows behavior to be added to individual objects,
/// either statically or dynamically, without affecting other objects.
/// 
/// Benefits:
/// - Add responsibilities to objects dynamically
/// - Flexible alternative to subclassing
/// - Combine decorators for multiple behaviors
/// - Open/Closed Principle: Open for extension, closed for modification
/// </summary>
internal class DecoratorDesignPattern
{
    /// <summary>
    /// Base interface for all loggers and decorators.
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
    }

    /// <summary>
    /// Concrete component - basic logger implementation.
    /// </summary>
    public sealed class Logger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG] {message}");
        }
    }

    /// <summary>
    /// Base decorator class that wraps an ILogger and delegates calls to it.
    /// This allows decorators to add behavior before/after the wrapped logger.
    /// </summary>
    public abstract class LoggerDecorator : ILogger
    {
        protected readonly ILogger _logger;

        protected LoggerDecorator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Virtual method allows derived decorators to override the logging behavior.
        /// Default implementation delegates to the wrapped logger.
        /// </summary>
        public virtual void Log(string message)
        {
            _logger.Log(message);
        }
    }

    /// <summary>
    /// Decorator that adds timestamp functionality to logging.
    /// </summary>
    public sealed class TimestampDecorator : LoggerDecorator
    {
        public TimestampDecorator(ILogger logger) : base(logger)
        {
        }

        public override void Log(string message)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff UTC");
            Console.WriteLine($"[TIMESTAMP] {timestamp}");
            _logger.Log(message);
        }
    }

    /// <summary>
    /// Decorator that formats log messages as JSON.
    /// </summary>
    public sealed class JsonDecorator : LoggerDecorator
    {
        public JsonDecorator(ILogger logger) : base(logger)
        {
        }

        public override void Log(string message)
        {
            string jsonMessage = System.Text.Json.JsonSerializer.Serialize(new
            {
                level = "INFO",
                message = message,
                timestamp = DateTime.UtcNow
            });

            Console.WriteLine("[JSON] " + jsonMessage);
            _logger.Log(message);
        }
    }

    /// <summary>
    /// Decorator that adds log level prefix.
    /// </summary>
    public sealed class LogLevelDecorator : LoggerDecorator
    {
        private readonly string _logLevel;

        public LogLevelDecorator(ILogger logger, string logLevel = "INFO") : base(logger)
        {
            _logLevel = logLevel;
        }

        public override void Log(string message)
        {
            Console.WriteLine($"[LEVEL] {_logLevel}");
            _logger.Log(message);
        }
    }

    /// <summary>
    /// Decorator that adds color to console output.
    /// </summary>
    public sealed class ColorDecorator : LoggerDecorator
    {
        private readonly ConsoleColor _color;

        public ColorDecorator(ILogger logger, ConsoleColor color = ConsoleColor.Green) : base(logger)
        {
            _color = color;
        }

        public override void Log(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = _color;
            Console.WriteLine("[COLOR] Applying color...");
            _logger.Log(message);
            Console.ForegroundColor = originalColor;
        }
    }

    /// <summary>
    /// Demonstrates various combinations of decorators.
    /// </summary>
    public static void Demo()
    {
        string message = "Hanumant is learning Low-Level Design (LLD)";

        Console.WriteLine("=== Decorator Design Pattern Demo ===\n");

        // Example 1: Basic logger (no decorators)
        Console.WriteLine("1. Basic Logger:");
        ILogger basicLogger = new Logger();
        basicLogger.Log(message);

        Console.WriteLine("\n2. Logger with Timestamp:");
        // Example 2: Logger with timestamp decorator
        ILogger timestampLogger = new TimestampDecorator(new Logger());
        timestampLogger.Log(message);

        Console.WriteLine("\n3. Logger with JSON formatting:");
        // Example 3: Logger with JSON decorator
        ILogger jsonLogger = new JsonDecorator(new Logger());
        jsonLogger.Log(message);

        Console.WriteLine("\n4. Logger with Multiple Decorators (Timestamp + JSON):");
        // Example 4: Combining multiple decorators - Order matters!
        // Wrapping: Timestamp -> JSON -> Logger
        ILogger multiLogger = new TimestampDecorator(new JsonDecorator(new Logger()));
        multiLogger.Log(message);

        Console.WriteLine("\n5. Logger with All Decorators:");
        // Example 5: Complex decoration chain
        // Wrapping: Color -> LogLevel -> Timestamp -> JSON -> Logger
        ILogger fullLogger = new ColorDecorator(
            new LogLevelDecorator(
                new TimestampDecorator(
                    new JsonDecorator(
                        new Logger()
                    )
                ),
                "DEBUG"
            ),
            ConsoleColor.Cyan
        );
        fullLogger.Log(message);

        Console.WriteLine("\n6. Different Decorator Order:");
        // Example 6: Different order produces different output
        // Wrapping: JSON -> Timestamp -> Logger
        ILogger reversedLogger = new JsonDecorator(new TimestampDecorator(new Logger()));
        reversedLogger.Log(message);

        Console.WriteLine("\n=== Real-World Use Cases ===");
        Console.WriteLine("• Add encryption to file writers");
        Console.WriteLine("• Add compression to data streams");
        Console.WriteLine("• Add authentication to HTTP clients");
        Console.WriteLine("• Add caching to data repositories");
        Console.WriteLine("• Add retry logic to network calls");

        Console.WriteLine("\n=== Demo Complete ===");
    }
}
