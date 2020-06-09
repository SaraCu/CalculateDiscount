namespace CalculateDiscount.Model
{
    public class User
    {
        public string UserName { get; set; }

        public bool IsRegistered { get; set; }

        public int ShoppingCount { get; set; }

        public decimal TotalAmountSpent { get; set; }
    }
}
