using CalculateDiscount.Model;

namespace CalculateDiscount.Handlers
{
    public class UnregisteredUserDiscountHandler : DiscountHandler
    {
        public override void Handle(Basket basket)
        {
            if (basket.User.Category == UserCategory.Unregistered)
            {
                var sum = 0m;
                foreach (var item in basket.ItemReceipts)
                {
                    item.Discount = 0;
                    item.DiscountedAmount = item.Amount;
                    sum += item.Amount;
                }
                
                basket.Receipt = new Receipt
                {
                    Amount = sum,
                    Discount = 0,
                    DiscountedAmount = sum,
                };
            }
            else if (nextHandler != null)
            {
                nextHandler.Handle(basket);
            }
        }
    }
}
