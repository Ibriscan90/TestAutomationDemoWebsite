using OpenQA.Selenium;
using Reqnroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAutomationDemoWebsite.Hooks
{
    [Binding]
    public class Hooks
    {

        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly IWebDriver _driver;

        public Hooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;

            // Access the driver from context if stored there
            if (_scenarioContext.TryGetValue("driver", out IWebDriver driver))
            {
                _driver = driver;
            }
        }

        [AfterScenario]
        public void TakeScreenshotOnFailure()
        {
            if (_scenarioContext.TestError != null && _driver != null)
            {
                var screenshotsDir = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots");
                Directory.CreateDirectory(screenshotsDir);

                var fileName = $"{_featureContext.FeatureInfo.Title}_{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var safeFileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

                var fullPath = Path.Combine(screenshotsDir, safeFileName);

                try
                {
                    ((ITakesScreenshot)_driver).GetScreenshot().SaveAsFile(fullPath);
                    Console.WriteLine($"Screenshot saved to: {fullPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to take screenshot: " + ex.Message);
                }
            }
        }

        [AfterScenario]

        public void CleanupWebDriver()
        {
            if (_scenarioContext.TryGetValue("driver", out IWebDriver driver))
            {
                try
                {
                    driver.Quit(); // Properly shuts down the driver and browser
                    driver.Dispose(); // Frees up resources
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to close WebDriver: " + ex.Message);
                }
            }
        }

    }
}
