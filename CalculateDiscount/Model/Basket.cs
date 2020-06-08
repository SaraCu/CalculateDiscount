using System.Collections.Generic;

namespace CalculateDiscount.Model
{
    public class Basket
    {
        public User User { get; set; }

        public ICollection<Receipt> ItemReceipts { get; set; }

        public Receipt Receipt { get; set; }
    }
}
