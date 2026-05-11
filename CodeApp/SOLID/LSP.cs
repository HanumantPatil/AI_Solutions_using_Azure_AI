using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp.SOLID
{
    // Liskov Substitution Principle (LSP) states that objects of a superclass should be replaceable with objects of a subclass
    // without affecting the correctness of the program.
    // In other words: If S is a subtype of T, then objects of type T may be replaced with objects of type S
    // without altering the correctness or expected behavior of the program.

    // Base class representing a rectangle with independent width and height
    class Rectangle
    {
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public int Area()
        {
            return Width * Height;
        }
    }

    // Square inherits from Rectangle but violates LSP
    // WHY IT VIOLATES LSP:
    // - Rectangle's contract expects Width and Height to be independently settable
    // - Square breaks this contract by coupling Width and Height together
    // - When Square is substituted for Rectangle, the expected behavior (independent dimensions) breaks
    class Square : Rectangle
    {
        public override int Width
        {
            get { return base.Width; }
            set
            {
                base.Width = value;
                base.Height = value;  // VIOLATION: Setting width also changes height
            }
        }
        public override int Height
        {
            get { return base.Height; }
            set
            {
                base.Height = value;
                base.Width = value;  // VIOLATION: Setting height also changes width
            }
        }
    }

    // EXAMPLE 1: LSP VIOLATION
    // Demonstrates how substituting Square for Rectangle breaks expected behavior
    class LSPViolationExample
    {
        public void Test()
        {
            // PROBLEM: Creating a Square but treating it as a Rectangle
            Rectangle rect = new Square();

            // EXPECTED BEHAVIOR: Setting width to 5 should only change width
            rect.Width = 5;

            // EXPECTED BEHAVIOR: Setting height to 10 should only change height
            rect.Height = 10;

            // EXPECTED RESULT: Area should be 5 * 10 = 50
            // ACTUAL RESULT: Area is 10 * 10 = 100 (because setting Height also set Width to 10)
            // REASON: Square's implementation violates the Rectangle's contract of independent dimensions
            Console.WriteLine(rect.Area()); // This will not work as expected, violating LSP
        }
    }

    // EXAMPLE 1: LSP ADHERENCE
    // Demonstrates the correct way to use Rectangle and Square without violating LSP
    class LSPAdherenceExample
    {
        public void Test()
        {
            // CORRECT APPROACH: Use Rectangle as Rectangle
            Rectangle rect = new Rectangle();
            rect.Width = 5;
            rect.Height = 10;
            Console.WriteLine(rect.Area()); // Output: 50 - Works as expected

            // CORRECT APPROACH: Use Square as Square (not substituting for Rectangle)
            Square square = new Square();
            square.Width = 5; // This will set both width and height to 5 (expected for a square)
            Console.WriteLine(square.Area()); // Output: 25 - Works as expected, adhering to LSP

            // KEY POINT: We don't substitute Square for Rectangle, so LSP is not violated
        }
    }


    // EXAMPLE 2: LSP VIOLATION (Collection scenario)
    // Demonstrates LSP violation when working with collections
    // NOTE: This example is conceptually a violation but the code doesn't fully demonstrate it
    class LSPViolationExample2
    {
        public void Test()
        {
            List<Rectangle> rectangles = new List<Rectangle>();

            // Add a proper Rectangle: Width=5, Height=10, Area=50
            rectangles.Add(new Rectangle { Width = 5, Height = 10 });

            // POTENTIAL PROBLEM: Adding Square to a Rectangle collection
            // When Width is set to 5, Height is also automatically set to 5 (not shown explicitly here)
            rectangles.Add(new Square { Width = 5 }); // Area will be 25 (5*5), not what might be expected

            // ISSUE: If we later try to modify dimensions expecting Rectangle behavior, it will fail
            // Example: If we did rectangles[1].Height = 10, we'd expect Area=50 but get Area=100
            foreach (var rect in rectangles)
            {
                Console.WriteLine(rect.Area()); // Output: 50, then 25
                // REASON FOR VIOLATION: The Square in the collection doesn't behave like a Rectangle
                // when its properties are modified - violating substitutability principle
            }
        }
    }

    // EXAMPLE 2: LSP ADHERENCE (Corrected approach)
    // NOTE: This example is identical to the violation example above, which indicates incomplete implementation
    // TO TRULY ADHERE TO LSP, we should:
    // 1. Not inherit Square from Rectangle, OR
    // 2. Create a common interface/base class that both implement appropriately, OR
    // 3. Keep them separate and not treat them polymorphically
    class LSPAdherenceExample2
    {
        public void Test()
        {
            List<Rectangle> rectangles = new List<Rectangle>();

            // Use only Rectangle objects in Rectangle collections
            rectangles.Add(new Rectangle { Width = 5, Height = 10 });
            rectangles.Add(new Square { Width = 5 }); // This still violates LSP if we modify properties later

            foreach (var rect in rectangles)
            {
                Console.WriteLine(rect.Area()); // Output: 50, then 25
            }

            // BETTER APPROACH TO ADHERE TO LSP:
            // Keep Square and Rectangle separate or redesign the hierarchy
            // For example, create an IShape interface that both implement independently
        }
    }


    // BETTER DESIGN TO ADHERE TO LSP:
    // Instead of inheritance, use composition or interface-based design

    interface IShape
    {
        int CalculateArea();
    }

    class BetterRectangle : IShape
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int CalculateArea() => Width * Height;
    }

    class BetterSquare : IShape
    {
        private int _side;
        public int Side
        {
            get => _side;
            set => _side = value;
        }
        public int CalculateArea() => Side * Side;
    }

    // Now Square and Rectangle don't have a parent-child relationship
    // They can't be substituted for each other, preventing LSP violations



    internal class LSP
    {
    }
}
