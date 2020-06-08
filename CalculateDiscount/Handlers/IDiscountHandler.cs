using CalculateDiscount.Model;

namespace CalculateDiscount.Handlers
{
    public interface IDiscountHandler
    {
        void Handle(Basket basket);

        void SetNextHandler(IDiscountHandler handler);
    }
}
