using System;
using System.Collections.Generic;
using System.Linq;
using CalculateDiscount.Handlers;
using CalculateDiscount.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCalculateDiscount
{
    [TestClass]
    public class UserDiscountHandlerTest 
    {
        private readonly IDiscountHandler handler;

        public UserDiscountHandlerTest()
        {
            var unregisteredUserDiscountHandler = new UnregisteredUserDiscountHandler();
            var newUserDiscountHandler = new NewUserDiscountHandler();
            var registeredUserDiscountHandler = new RegisteredUserDiscountHandler();
            var goldenUserDiscountHandler = new GoldenUserDiscountHandler();
            unregisteredUserDiscountHandler.SetNextHandler(newUserDiscountHandler);
            newUserDiscountHandler.SetNextHandler(registeredUserDiscountHandler);
            registeredUserDiscountHandler.SetNextHandler(goldenUserDiscountHandler);
            handler = unregisteredUserDiscountHandler;
        }

        [TestMethod]
        public void HandlerAddsNoDiscountForUnregisteredUser()
        {
            var basket = new Basket
            {
                User = new User
                {
                    UserName = "name",
                    Category = UserCategory.Unregistered,
                },
                ItemReceipts = new List<Receipt>
                {
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 15,
                    },
                    new Receipt
                    {
                        Amount = 16,
                    },
                    new Receipt
                    {
                        Amount = 20,
                    },
                }
            };

            handler.Handle(basket);

            foreach (var item in basket.ItemReceipts)
            {
                Assert.AreEqual(0, item.Discount);
                Assert.IsTrue(item.Amount == item.DiscountedAmount);
            }

            Assert.AreEqual(0, basket.Receipt.Discount);
            Assert.IsTrue(basket.Receipt.Amount == basket.Receipt.DiscountedAmount);
        }

        [TestMethod]
        public void HandlerSetsCheapestItemDiscountTo100ForNewUserAnd3OrMoreItemsInBasket()
        {
            var basket = new Basket
            {
                User = new User
                {
                    UserName = "name",
                    Category = UserCategory.New,
                },
                ItemReceipts = new List<Receipt>
                {
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 15,
                    },
                    new Receipt
                    {
                        Amount = 16,
                    },
                    new Receipt
                    {
                        Amount = 20,
                    },
                }
            };

            handler.Handle(basket);

            var cheapest = basket.ItemReceipts.OrderBy(i => i.Amount).First();

            Assert.AreEqual(1, cheapest.Discount);
            Assert.AreEqual(0, cheapest.DiscountedAmount);

            var except = new List<Receipt> { cheapest };

            foreach (var item in basket.ItemReceipts.Except(except))
            {
                Assert.AreEqual(0, item.Discount);
                Assert.IsTrue(item.Amount == item.DiscountedAmount);
            }
        }

        [TestMethod]
        public void HandlerAddsNoDiscountTo1ForNewUserAndLessThen3ItemsInBasket()
        {
            var basket = new Basket
            {
                User = new User
                {
                    UserName = "name",
                    Category = UserCategory.New,
                },
                ItemReceipts = new List<Receipt>
                {
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 15,
                    },
                }
            };

            handler.Handle(basket);

            foreach (var item in basket.ItemReceipts)
            {
                Assert.AreEqual(0, item.Discount);
                Assert.IsTrue(item.Amount == item.DiscountedAmount);
            }

            Assert.AreEqual(0, basket.Receipt.Discount);
            Assert.IsTrue(basket.Receipt.Amount == basket.Receipt.DiscountedAmount);
        }

        [TestMethod]
        public void HandlerAddsDiscountForEachItemForRegisteredUser()
        {
            var basket = new Basket
            {
                User = new User
                {
                    UserName = "name",
                    Category = UserCategory.Registered,
                },
                ItemReceipts = new List<Receipt>
                {
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 15,
                    },
                    new Receipt
                    {
                        Amount = 16,
                    },
                    new Receipt
                    {
                        Amount = 20,
                    },
                }
            };

            handler.Handle(basket);

            var discountedSum = 0m;
            foreach (var item in basket.ItemReceipts)
            {
                Assert.AreEqual(0.05m, item.Discount);
                Assert.AreEqual(item.Amount * (1 - item.Discount), item.DiscountedAmount);
                discountedSum += item.DiscountedAmount;
            }

            Assert.IsTrue(basket.Receipt.DiscountedAmount == discountedSum);
        }

        [TestMethod]
        public void HandlerAddsDiscount50ForCheapestItemAnd5ForOtherItemsForGoldenUserAnd3OrMoreItems()
        {
            var basket = new Basket
            {
                User = new User
                {
                    UserName = "name",
                    Category = UserCategory.Golden,
                },
                ItemReceipts = new List<Receipt>
                {
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 15,
                    },
                    new Receipt
                    {
                        Amount = 16,
                    },
                    new Receipt
                    {
                        Amount = 20,
                    },
                }
            };

            handler.Handle(basket);

            var cheapest = basket.ItemReceipts.OrderBy(i => i.Amount).First();

            Assert.AreEqual(0.5m, cheapest.Discount);
            Assert.AreEqual(cheapest.Amount * (1 - 0.5m), cheapest.DiscountedAmount);

            var except = new List<Receipt> { cheapest };

            foreach (var item in basket.ItemReceipts.Except(except))
            {
                Assert.AreEqual(0.05m, item.Discount);
                Assert.AreEqual(item.Amount * (1 - 0.05m), item.DiscountedAmount);
            }
        }

        [TestMethod]
        public void HandlerAddsDiscount50ForOneOfCheapestItemsAnd5ForOtherItemsForGoldenUserAnd3OrMoreItems()
        {
            var basket = new Basket
            {
                User = new User
                {
                    UserName = "name",
                    Category = UserCategory.Golden,
                },
                ItemReceipts = new List<Receipt>
                {
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 16,
                    },
                    new Receipt
                    {
                        Amount = 20,
                    },
                }
            };

            handler.Handle(basket);

            var cheapest = basket.ItemReceipts.OrderBy(i => i.Amount).First();

            Assert.AreEqual(0.5m, cheapest.Discount);
            Assert.AreEqual(cheapest.Amount * (1 - 0.5m), cheapest.DiscountedAmount);

            var except = new List<Receipt> { cheapest };

            foreach (var item in basket.ItemReceipts.Except(except))
            {
                Assert.AreEqual(0.05m, item.Discount);
                Assert.AreEqual(item.Amount * (1 - 0.05m), item.DiscountedAmount);
            }
        }

        [TestMethod]
        public void HandlerAddsDiscount5ForEachItemForGoldenUserAndLessThen3Items()
        {
            var basket = new Basket
            {
                User = new User
                {
                    UserName = "name",
                    Category = UserCategory.Golden,
                },
                ItemReceipts = new List<Receipt>
                {
                    new Receipt
                    {
                        Amount = 10,
                    },
                    new Receipt
                    {
                        Amount = 13,
                    },
                }
            };

            handler.Handle(basket);

            var discountedSum = 0m;
            foreach (var item in basket.ItemReceipts)
            {
                Assert.AreEqual(0.05m, item.Discount);
                Assert.AreEqual(item.Amount * (1 - 0.05m), item.DiscountedAmount);
                discountedSum += item.DiscountedAmount;
            }

            Assert.AreEqual(discountedSum, basket.Receipt.DiscountedAmount);
        }
    }
}
