using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeApp.SOLID
{
    // Open-Closed Principle (OCP) states that software entities (classes, modules, functions, etc.) should be open for extension but closed for modification.

    interface IDiscount
    {
        decimal ApplyDiscount(decimal price);
    }

    class SeasonalDiscount : IDiscount
    {
        public decimal ApplyDiscount(decimal price)
        {
            return price * 0.9m; // 10% discount
        }
    }

    class FestiveDiscount : IDiscount
    {
        public decimal ApplyDiscount(decimal price)
        {
            return price * 0.8m; // 20% discount
        }
    }

    internal class DiscountCalculator
    {
        private readonly IDiscount _discount;
        public DiscountCalculator(IDiscount discount)
        {
            _discount = discount;
        }
        public decimal CalculatePrice(decimal price)
        {
            return _discount.ApplyDiscount(price);
        }
    }


    internal class OpenClosedPrinciple
    {
        public void DemonstrateDiscounts()
        {
            // Call discount calculator with different discounts without modifying existing code
            DiscountCalculator discountCalculator = new DiscountCalculator(new SeasonalDiscount());
            discountCalculator.CalculatePrice(100); // Applies seasonal discount
        }
    }
}
