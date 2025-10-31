using Newtonsoft.Json.Linq;
using RestSharp;

namespace ReqResTests.Tests
{
    public class BaseApiTest : IDisposable
    {
        protected RestClient Client { get; private set; } = null!;
        private string BaseUrl { get; set; } = string.Empty;
        private string EnvironmentName { get; set; } = "Dev";
        private string ApiKey { get; set; } = string.Empty;

        private const string DefaultBaseUrl = "https://reqres.in";
        private const string DefaultApiKey = "reqres-free-v1";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            LoadConfig();
            InitializeClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown() => Dispose();

        private void LoadConfig()
        {
            const string configPath = "Config/appsettings.json";

            if (!File.Exists(configPath))
            {
                TestContext.Out.WriteLine("[WARN] Config file not found. Using default settings.");
                BaseUrl = DefaultBaseUrl;
                ApiKey = DefaultApiKey;
                return;
            }

            try
            {
                var configText = File.ReadAllText(configPath);
                var config = JObject.Parse(configText);

                var cliEnv = TestContext.Parameters.Get("env", null);
                var envOverride = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT");

                EnvironmentName = (cliEnv ?? envOverride ?? "Dev").Trim();
                EnvironmentName = char.ToUpper(EnvironmentName[0]) + EnvironmentName[1..].ToLower();

                BaseUrl = config["Environments"]?[EnvironmentName]?.ToString()?.TrimEnd('/')
                          ?? config["ApiBaseUrl"]?.ToString()?.TrimEnd('/')
                          ?? DefaultBaseUrl;

                ApiKey = config["ApiKey"]?.ToString() ?? DefaultApiKey;
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"[WARN] Failed to parse config: {ex.Message}. Using defaults.");
                BaseUrl = DefaultBaseUrl;
                ApiKey = DefaultApiKey;
            }

            TestContext.Out.WriteLine($"[INFO] Selected Environment: {EnvironmentName}");
            TestContext.Out.WriteLine($"[INFO] Using Base URL: {BaseUrl}");
        }

        private void InitializeClient()
        {
            Client = new RestClient(BaseUrl);
            Client.AddDefaultHeader("x-api-key", ApiKey);
        }

        public void Dispose()
        {
            Client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
