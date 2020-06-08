using CalculateDiscount.Model;
using System.Collections.Generic;
using System.Linq;

namespace CalculateDiscount.Handlers
{
    public class GoldenUserDiscountHandler : DiscountHandler
    {
        public override void Handle(Basket basket)
        {
            if (basket.User.Category == UserCategory.Golden)
            {
                if (basket.ItemReceipts.Count() >= 3)
                {
                    var cheapest = basket.ItemReceipts.OrderBy(i => i.Amount).First();
                    cheapest.Discount = 0.5m;
                    cheapest.DiscountedAmount = cheapest.Amount * (1 - cheapest.Discount);

                    var except = new List<Receipt> { cheapest };
                    AddDiscount(basket.ItemReceipts.Except(except), 0.05m);
                }
                else
                {
                    AddDiscount(basket.ItemReceipts, 0.05m);
                }

                var sum = basket.ItemReceipts.Sum(i => i.Amount);
                var discountedSum = basket.ItemReceipts.Sum(i => i.DiscountedAmount);
                basket.Receipt = new Receipt
                {
                    Amount = sum,
                    DiscountedAmount = discountedSum,
                    Discount = 1 - discountedSum / sum,
                };
            }
            else if (nextHandler != null)
            {
                nextHandler.Handle(basket);
            }
        }
    }
}
