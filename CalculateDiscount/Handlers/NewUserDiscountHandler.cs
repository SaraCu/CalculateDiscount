using CalculateDiscount.Model;
using System.Collections.Generic;
using System.Linq;

namespace CalculateDiscount.Handlers
{
    public class NewUserDiscountHandler : DiscountHandler
    {
        public override void Handle(Basket basket)
        {
            if (basket.User.IsRegistered && basket.User.ShoppingCount == 0)
            {
                if (basket.ItemReceipts.Count() >= 3)
                {
                    var cheapest = basket.ItemReceipts.OrderBy(i => i.Amount).First();
                    cheapest.Discount = 1;
                    cheapest.DiscountedAmount = 0;
                    var except = new List<Receipt> { cheapest };
                    AddDiscount(basket.ItemReceipts.Except(except), 0);
                }
                else
                {
                    AddDiscount(basket.ItemReceipts, 0);
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
