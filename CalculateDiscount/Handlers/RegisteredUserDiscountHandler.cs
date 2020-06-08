using CalculateDiscount.Model;
using System.Linq;

namespace CalculateDiscount.Handlers
{
    public class RegisteredUserDiscountHandler : DiscountHandler
    {
        public override void Handle(Basket basket)
        {
            if (basket.User.Category == UserCategory.Registered)
            {
                AddDiscount(basket.ItemReceipts, 0.05m);
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
