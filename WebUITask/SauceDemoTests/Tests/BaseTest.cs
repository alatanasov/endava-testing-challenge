using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Newtonsoft.Json.Linq;
using AventStack.ExtentReports;
using SauceDemoTests.Config;
using SauceDemoTests.Pages;

namespace SauceDemoTests.Tests
{
    [Parallelizable(ParallelScope.Fixtures)]
    [TestFixture]
    public class BaseTest
    {
        protected IWebDriver? Driver { get; private set; }
        private string BaseUrl { get; set; } = string.Empty;
        private string Username { get; set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        protected string CheckoutFirstName { get; private set; } = string.Empty;
        protected string CheckoutLastName { get; private set; } = string.Empty;
        protected string CheckoutPostalCode { get; private set; } = string.Empty;

        private string Browser { get; set; } = string.Empty;
        private string EnvironmentName { get; set; } = "Dev";
        private string Resolution { get; set; } = "1920x1080";

        [SetUp]
        public void SetUp()
        {
            LoadConfig();
            InitializeDriver();
            ApplyResolution();

            Driver!.Navigate().GoToUrl(BaseUrl);

            if (ExtentReportManager.Extent == null)
                ExtentReportManager.InitReport(environment: EnvironmentName, browser: Browser, resolution: Resolution, tester: "Alex");

            ExtentReportManager.Test = ExtentReportManager.Extent?.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentReportManager.Test?.Log(Status.Info, $"Environment: {EnvironmentName} | Browser: {Browser} | Resolution: {Resolution}");

            TestContext.Out.WriteLine($"[INFO] Environment: {EnvironmentName}");
            TestContext.Out.WriteLine($"[INFO] Browser: {Browser}");
            TestContext.Out.WriteLine($"[INFO] Resolution: {Resolution}");
            TestContext.Out.WriteLine($"[INFO] Tester: Alex");

        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                var outcome = TestContext.CurrentContext.Result.Outcome.Status;
                var message = TestContext.CurrentContext.Result.Message;

                if (outcome == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    TestContext.Out.WriteLine($"[ERROR] Test failed: {message}");
                    ExtentReportManager.Test?.Log(Status.Fail, $"Test failed: {message}");
                }
                else if (outcome == NUnit.Framework.Interfaces.TestStatus.Passed)
                {
                    TestContext.Out.WriteLine($"[PASS] Test passed: {TestContext.CurrentContext.Test.Name}");
                    ExtentReportManager.Test?.Log(Status.Pass, "Test passed successfully.");
                }
                else
                {
                    TestContext.Out.WriteLine($"[WARN] Test finished with status: {outcome}");
                    ExtentReportManager.Test?.Log(Status.Warning, $"Test finished with status: {outcome}");
                }
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"[ERROR] TearDown failed: {ex.Message}");
                ExtentReportManager.Test?.Log(Status.Error, $"TearDown error: {ex.Message}");
            }
            finally
            {
                Driver?.Quit();
                Driver?.Dispose();
            }
        }

        [OneTimeTearDown]
        public void GlobalTeardownForAssembly()
        {
            ExtentReportManager.FlushReport();
        }

        private void LoadConfig()
        {
            var configText = File.ReadAllText("Config/appsettings.json");
            var config = JObject.Parse(configText);

            Username = config["Credentials"]?["StandardUser"]?.ToString() ?? "standard_user";
            Password = config["Credentials"]?["Password"]?.ToString() ?? "secret_sauce";

            CheckoutFirstName = config["DefaultCheckout"]?["FirstName"]?.ToString() ?? "Alex";
            CheckoutLastName = config["DefaultCheckout"]?["LastName"]?.ToString() ?? "Test";
            CheckoutPostalCode = config["DefaultCheckout"]?["PostalCode"]?.ToString() ?? "1000";

            var cliEnv = TestContext.Parameters.Get("env", null);
            var cliBrowser = TestContext.Parameters.Get("browser", null);
            var cliResolution = TestContext.Parameters.Get("resolution", null);

            var envOverride = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");
            var browserOverride = Environment.GetEnvironmentVariable("TEST_BROWSER");
            var resolutionOverride = Environment.GetEnvironmentVariable("TEST_RESOLUTION");

            Browser = (cliBrowser ?? browserOverride ?? config["Browsers"]?["Default"]?.ToString() ?? "Chrome").Trim();
            EnvironmentName = (cliEnv ?? envOverride ?? "Dev").Trim();
            Resolution = (cliResolution ?? resolutionOverride ?? "1920x1080").Trim();

            BaseUrl = config["Environments"]?[EnvironmentName]?.ToString()
                      ?? config["Environments"]?["Dev"]?.ToString()
                      ?? "https://www.saucedemo.com/";
        }

        private void InitializeDriver()
        {
            switch (Browser.ToLowerInvariant())
            {
                case "chrome":
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--guest");
                    chromeOptions.AddArgument("--disable-notifications");
                    chromeOptions.AddArgument("--no-first-run");
                    chromeOptions.AddArgument("--no-default-browser-check");
                    Driver = new ChromeDriver(chromeOptions);
                    break;

                case "firefox":
                    var firefoxOptions = new FirefoxOptions();
                    firefoxOptions.SetPreference("signon.rememberSignons", false);
                    Driver = new FirefoxDriver(firefoxOptions);
                    break;

                default:
                    throw new ArgumentException($"Unsupported browser: {Browser}. Supported: Chrome, Firefox.");
            }
        }

        private void ApplyResolution()
        {
            try
            {
                if (Resolution.Equals("maximize", StringComparison.OrdinalIgnoreCase))
                {
                    Driver!.Manage().Window.Maximize();
                    return;
                }

                var parts = Resolution.ToLower().Split('x');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0], out var width) &&
                    int.TryParse(parts[1], out var height))
                {
                    Driver!.Manage().Window.Size = new System.Drawing.Size(width, height);
                }
                else
                {
                    TestContext.Out.WriteLine($"[WARN] Invalid resolution '{Resolution}', using 1920x1080.");
                    Driver!.Manage().Window.Size = new System.Drawing.Size(1920, 1080);
                }
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"[WARN] Failed to apply resolution: {ex.Message}");
            }
        }

        protected void PerformLogin()
        {
            var loginPage = new LoginPage(Driver!);
            loginPage.Login(Username, Password);
            Assert.That(Driver!.Url, Does.Contain("inventory.html"), "Login failed.");
        }

        protected void PerformLogout()
        {
            var inventory = new InventoryPage(Driver!);
            inventory.Logout();
            var loginPage = new LoginPage(Driver!);
            Assert.That(loginPage.IsAtLoginPage(), Is.True, "Logout failed.");
        }

        protected void AssertUrlContains(string keyword)
        {
            Assert.That(Driver!.Url, Does.Contain(keyword),
                $"Expected URL to contain '{keyword}', but got '{Driver.Url}'.");
        }
    }
}
