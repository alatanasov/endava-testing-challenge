using SauceDemoTests.Pages;

namespace SauceDemoTests.Tests
{
    [TestFixture]
    public class SauceDemoWorkflowTests : BaseTest
    {
        [Test]
        [Category("CompletingOrder")]
        [Category("HighPriority")]
        public void VerifyCompleteOrderWithCartModification()
        {
            PerformLogin();

            var inventory = new InventoryPage(Driver!);
            var items = inventory.GetItemNames();
            Assert.That(items, Is.Not.Empty, "No items were found in inventory.");

            var firstItem = items.First();
            var lastItem = items.Last();
            var secondToLastItem = items[^2];

            inventory.AddItemToCart(firstItem);
            inventory.AddItemToCart(lastItem);

            inventory.GoToCart();
            var cart = new CartPage(Driver!);
            var cartItems = cart.GetCartItems();

            Assert.Multiple(() =>
            {
                Assert.That(cartItems, Does.Contain(firstItem), "First item missing from cart.");
                Assert.That(cartItems, Does.Contain(lastItem), "Last item missing from cart.");
            });

            cart.BackToProducts();
            inventory.RemoveItemFromCart(firstItem);
            inventory.AddItemToCart(secondToLastItem);

            inventory.GoToCart();
            cartItems = cart.GetCartItems();

            Assert.Multiple(() =>
            {
                Assert.That(cartItems, Does.Not.Contain(firstItem), "Removed item still present.");
                Assert.That(cartItems, Does.Contain(lastItem), "Last item missing after modification.");
                Assert.That(cartItems, Does.Contain(secondToLastItem), "Newly added item missing.");
            });

            cart.Checkout(CheckoutFirstName, CheckoutLastName, CheckoutPostalCode);

            AssertUrlContains("checkout-complete.html");

            cart.BackToProducts();
            inventory.GoToCart();
            cartItems = cart.GetCartItems();
            Assert.That(cartItems, Is.Empty, "Cart not empty after order completion.");

            PerformLogout();
        }

        [Test]
        [Category("ItemsSorting")]
        [Category("LowPriority")]
        public void VerifyItemsAreSortedByPriceHighToLow()
        {
            PerformLogin();

            var inventory = new InventoryPage(Driver!);
            Assert.That(inventory.GetItemNames(), Is.Not.Empty, "No items were found in inventory.");

            inventory.SortByPriceHighToLow();
            var prices = inventory.GetItemPrices();
            Assert.That(prices.Count, Is.GreaterThan(1), "Not enough items to verify sorting.");

            for (var i = 0; i < prices.Count - 1; i++)
            {
                Assert.That(prices[i], Is.GreaterThanOrEqualTo(prices[i + 1]),
                    $"Prices not sorted correctly: {prices[i]} < {prices[i + 1]}");
            }
            
            PerformLogout();
        }
    }
}
