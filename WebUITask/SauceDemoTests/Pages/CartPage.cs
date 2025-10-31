using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SauceDemoTests.Pages
{
    public class CartPage(IWebDriver driver)
    {
        private readonly WebDriverWait _wait = new(driver, TimeSpan.FromSeconds(10));

        private IReadOnlyCollection<IWebElement> CartItemElements() =>
            _wait.Until(d => d.FindElements(By.ClassName("inventory_item_name")));

        private IWebElement BackToProductsButton() =>
            _wait.Until(d =>
                d.FindElements(By.Id("back-to-products")).FirstOrDefault() ??
                d.FindElements(By.Id("continue-shopping")).FirstOrDefault());

        private IWebElement CheckoutButton() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("checkout")));

        private IWebElement FirstNameField() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("first-name")));

        private IWebElement LastNameField() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("last-name")));

        private IWebElement PostalCodeField() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("postal-code")));

        private IWebElement ContinueButton() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("continue")));

        private IWebElement FinishButton() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("finish")));

        private void SetFieldValue(IWebElement field, string value)
        {
            field.Clear();
            field.SendKeys(value);
        }

        public List<string> GetCartItems() =>
            CartItemElements().Select(x => x.Text).ToList();

        public void BackToProducts() => BackToProductsButton().Click();

        public void Checkout(string firstName, string lastName, string postalCode)
        {
            CheckoutButton().Click();

            SetFieldValue(FirstNameField(), firstName);
            SetFieldValue(LastNameField(), lastName);
            SetFieldValue(PostalCodeField(), postalCode);

            ContinueButton().Click();
            FinishButton().Click();

            _wait.Until(ExpectedConditions.UrlContains("checkout-complete"));
        }
    }
}