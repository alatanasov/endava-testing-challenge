using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;

namespace SauceDemoTests.Config
{
    public static class ExtentReportManager
    {
        public static ExtentReports? Extent;
        public static ExtentTest? Test;
        private static string? _reportPath;

        public static void InitReport(string environment = "Dev", string browser = "Chrome", string resolution = "1920x1080", string tester = "Alex")
        {
            if (Extent != null) return;

            var projectDir = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
            var reportsDir = Path.Combine(projectDir, "TestResults");
            Directory.CreateDirectory(reportsDir);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _reportPath = Path.Combine(reportsDir, $"SauceReport_{timestamp}.html");

            var spark = new ExtentSparkReporter(_reportPath);
            spark.Config.DocumentTitle = "SauceDemo Automated Test Report";
            spark.Config.ReportName = "Endava Testing Challenge";
            spark.Config.Theme = Theme.Standard;

            Extent = new ExtentReports();
            Extent.AttachReporter(spark);
            Extent.AddSystemInfo("Environment", environment);
            Extent.AddSystemInfo("Browser", browser);
            Extent.AddSystemInfo("Resolution", resolution);
            Extent.AddSystemInfo("Tester", tester);
        }



        public static void FlushReport()
        {
            try
            {
                Extent?.Flush();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!string.IsNullOrEmpty(_reportPath))
                    Console.WriteLine($"Report generated: {_reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to flush report: {ex.Message}");
            }
        }
    }
}
