using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Globalization;

namespace SauceDemoTests.Pages
{
    public class InventoryPage(IWebDriver driver)
    {
        private readonly WebDriverWait _wait = new(driver, TimeSpan.FromSeconds(5));

        private IReadOnlyCollection<IWebElement> ItemElements() =>
            _wait.Until(d => d.FindElements(By.ClassName("inventory_item_name")));

        private IWebElement CartLink() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("shopping_cart_link")));

        private IWebElement MenuButton() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("react-burger-menu-btn")));

        private IWebElement LogoutLink() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("logout_sidebar_link")));

        private IWebElement SortDropdown() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("product_sort_container")));

        private IReadOnlyCollection<IWebElement> PriceElements() =>
            _wait.Until(d => d.FindElements(By.ClassName("inventory_item_price")));

        private static decimal ParsePrice(string priceText) =>
            decimal.Parse(priceText.Replace("$", "").Trim(), CultureInfo.InvariantCulture);

        public List<string> GetItemNames() =>
            ItemElements().Select(i => i.Text).ToList();

        public void AddItemToCart(string itemName)
        {
            string dataTestValue = $"add-to-cart-{itemName.ToLower().Replace(" ", "-")}";
            var addButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                By.CssSelector($"button[data-test='{dataTestValue}']")));
            addButton.Click();
        }

        public void RemoveItemFromCart(string itemName)
        {
            string dataTestValue = $"remove-{itemName.ToLower().Replace(" ", "-")}";
            var removeButton = _wait.Until(ExpectedConditions.ElementToBeClickable(
                By.CssSelector($"button[data-test='{dataTestValue}']")));
            removeButton.Click();
        }

        public void GoToCart() => CartLink().Click();

        public void Logout()
        {
            MenuButton().Click();
            LogoutLink().Click();
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("login-button")));
        }

        public void SortByPriceHighToLow()
        {
            const string sortValue = "hilo";
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    var dropdown = new SelectElement(SortDropdown());
                    dropdown.SelectByValue(sortValue);

                    _wait.Until(d =>
                        new SelectElement(SortDropdown()).SelectedOption.GetAttribute("value") == sortValue);
                    return;
                }
                catch (StaleElementReferenceException)
                {
                    _wait.Until(ExpectedConditions.ElementExists(By.ClassName("product_sort_container")));
                }
            }

            throw new WebDriverException("Unable to sort items by price (High → Low)");
        }

        public List<decimal> GetItemPrices() =>
            PriceElements().Select(p => ParsePrice(p.Text)).ToList();
    }
}