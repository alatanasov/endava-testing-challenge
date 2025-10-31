using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace SauceDemoTests.Pages
{
    public class LoginPage(IWebDriver driver)
    {
        private readonly WebDriverWait _wait = new(driver, TimeSpan.FromSeconds(5));

        private IWebElement UsernameField() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("user-name")));

        private IWebElement PasswordField() =>
            _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("password")));

        private IWebElement LoginButton() =>
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("login-button")));

        private void SetFieldValue(IWebElement field, string value)
        {
            field.Clear();
            field.SendKeys(value);
        }

        public void Login(string username, string password)
        {
            SetFieldValue(UsernameField(), username);
            SetFieldValue(PasswordField(), password);
            LoginButton().Click();

            _wait.Until(ExpectedConditions.UrlContains("inventory.html"));
        }

        public bool IsAtLoginPage()
        {
            try
            {
                return _wait.Until(d =>
                    d.FindElement(By.Id("login-button")).Displayed &&
                    d.FindElement(By.Id("user-name")).Displayed);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}