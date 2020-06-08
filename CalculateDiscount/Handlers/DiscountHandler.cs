using CalculateDiscount.Model;
using System.Collections.Generic;

namespace CalculateDiscount.Handlers
{
    public abstract class DiscountHandler : IDiscountHandler
    {
        protected IDiscountHandler nextHandler;

        public abstract void Handle(Basket basket);

        public void SetNextHandler(IDiscountHandler handler)
        {
            nextHandler = handler;
        }

        protected void AddDiscount(IEnumerable<Receipt> items, decimal discount)
        {
            foreach (var item in items)
            {
                item.Discount = discount;
                item.DiscountedAmount = item.Amount * (1 - item.Discount);
            }
        }
    }
}
