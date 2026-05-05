namespace CodeApp;

/// <summary>
/// Demonstrates the Builder Design Pattern in .NET 8.
/// Key Benefits:
/// - Handles multiple parameters elegantly
/// - Optional parameters without constructor overloading
/// - Fluent interface with method chaining
/// - Order-independent method calls
/// - Immutable product objects
/// </summary>
internal class BuilderDesignPattern
{
    /// <summary>
    /// Immutable Student class representing the complex object being built.
    /// </summary>
    public sealed class Student
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public int Age { get; init; }
        public string? Address { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Email { get; init; }

        // Private constructor - only the builder can create instances
        private Student() { }

        /// <summary>
        /// Creates a new StudentBuilder for constructing Student instances.
        /// </summary>
        /// <param name="id">Required unique identifier for the student</param>
        public static StudentBuilder CreateBuilder(int id) => new(id);

        public override string ToString() =>
            $"Student {{ Id: {Id}, Name: {Name ?? "N/A"}, Age: {Age}, " +
            $"Address: {Address ?? "N/A"}, Phone: {PhoneNumber ?? "N/A"}, Email: {Email ?? "N/A"} }}";

        /// <summary>
        /// Nested Builder class for constructing Student instances with fluent API.
        /// Validates required fields and provides flexible construction.
        /// </summary>
        public sealed class StudentBuilder
        {
            private readonly int _id;
            private string? _name;
            private int _age;
            private string? _address;
            private string? _phoneNumber;
            private string? _email;

            /// <summary>
            /// Initializes a new StudentBuilder with required ID.
            /// </summary>
            /// <param name="id">Required unique identifier</param>
            public StudentBuilder(int id)
            {
                if (id <= 0)
                    throw new ArgumentException("Student ID must be greater than zero.", nameof(id));

                _id = id;
            }

            /// <summary>
            /// Sets the student's name.
            /// </summary>
            public StudentBuilder WithName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Name cannot be null or empty.", nameof(name));

                _name = name;
                return this;
            }

            /// <summary>
            /// Sets the student's age.
            /// </summary>
            public StudentBuilder WithAge(int age)
            {
                if (age < 0 || age > 150)
                    throw new ArgumentException("Age must be between 0 and 150.", nameof(age));

                _age = age;
                return this;
            }

            /// <summary>
            /// Sets the student's address.
            /// </summary>
            public StudentBuilder WithAddress(string address)
            {
                _address = address;
                return this;
            }

            /// <summary>
            /// Sets the student's phone number.
            /// </summary>
            public StudentBuilder WithPhoneNumber(string phoneNumber)
            {
                _phoneNumber = phoneNumber;
                return this;
            }

            /// <summary>
            /// Sets the student's email address.
            /// </summary>
            public StudentBuilder WithEmail(string email)
            {
                if (!string.IsNullOrWhiteSpace(email) && !email.Contains('@'))
                    throw new ArgumentException("Invalid email format.", nameof(email));

                _email = email;
                return this;
            }

            /// <summary>
            /// Builds and returns the immutable Student instance.
            /// </summary>
            public Student Build()
            {
                return new Student
                {
                    Id = _id,
                    Name = _name,
                    Age = _age,
                    Address = _address,
                    PhoneNumber = _phoneNumber,
                    Email = _email
                };
            }
        }
    }

    /// <summary>
    /// Example usage demonstrating the Builder pattern.
    /// </summary>
    public static void Demo()
    {
        // Example 1: Full student with all properties
        var student1 = Student.CreateBuilder(1)
            .WithName("John Doe")
            .WithAge(20)
            .WithAddress("123 Main St, City, State")
            .WithPhoneNumber("+1-234-567-8900")
            .WithEmail("john.doe@example.com")
            .Build();

        Console.WriteLine(student1);

        // Example 2: Minimal student (only required ID)
        var student2 = Student.CreateBuilder(2)
            .WithName("Jane Smith")
            .WithAge(22)
            .Build();

        Console.WriteLine(student2);

        // Example 3: Order doesn't matter in method chaining
        var student3 = Student.CreateBuilder(3)
            .WithEmail("alice@example.com")
            .WithName("Alice Johnson")
            .WithPhoneNumber("+1-555-123-4567")
            .WithAge(19)
            .WithAddress("456 Oak Ave")
            .Build();

        Console.WriteLine(student3);
    }
}
