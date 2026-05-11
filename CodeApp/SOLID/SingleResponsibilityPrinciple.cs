using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp.SOLID
{

    // Single Responsibility Principle (SRP) states that a class should have only one reason to change,
    // meaning it should have only one responsibility or job.
    // This principle helps to create more maintainable and flexible code by ensuring that each class has a clear and focused purpose.

    class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
    class UserRepository
    {
        public void Save(User user)
        {
            // Code to save user to database
        }
    }
    class UserPrinter
    {
        public void Print(User user)
        {
            Console.WriteLine($"Name: {user.Name}, Email: {user.Email}");
        }
    }

    class EmailService
    {
        public void SendEmail(User user, string message)
        {
            // Code to send email to user
        }
    }

    internal class SingleResponsibilityPrinciple
    {
    }
}
