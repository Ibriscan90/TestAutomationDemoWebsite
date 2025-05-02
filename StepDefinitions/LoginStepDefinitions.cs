using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Reqnroll;
using SeleniumExtras.WaitHelpers;
using TestAutomationDemoWebsite.Helpers;

namespace TestAutomationDemoWebsite.StepDefinitions
{
    [Binding]
    public class LoginStepDefinitions
    {
        private IWebDriver _driver;
        private readonly ScenarioContext _scenarioContext;

        public LoginStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given("the user is on the login page")]
        public void GivenTheUserIsOnTheLoginPage()
        {
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            _scenarioContext["driver"] = _driver;

            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            _driver.Navigate().GoToUrl(Config.BaseUrl);


            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            try
            {
                var hackButton = wait.Until(driver =>
                {
                    var button = driver.FindElements(By.XPath("//button[text()='Let me hack!']")).FirstOrDefault();
                    return (button != null && button.Displayed) ? button : null;
                });

                if (hackButton != null)
                {
                    // Scroll the button into view if needed
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", hackButton);

                    // Save screenshot before clicking
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                    screenshot.SaveAsFile("screenshot_before_click.png");

                    hackButton.Click();
                }
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Let me hack button not found or not needed — continuing...");
            }

            // Now navigate to Admin panel safely
            var adminPanelLink = wait.Until(ExpectedConditions.ElementToBeClickable(By.LinkText("Admin")));
            adminPanelLink.Click();

            // Confirm URL changed
            wait.Until(driver => driver.Url.Contains("/admin"));


            // Wait until the username field appears
            wait.Until(drv => drv.FindElement(By.Id("username")));
            wait.Until(drv => drv.FindElement(By.Id("password")));
        }
        [When("the user logs in with username and password")]
        public void WhenTheUserLogsInWithUsernameAndPassword()
        {
            _driver.FindElement(By.Id("username")).SendKeys(Config.Username);
            _driver.FindElement(By.Id("password")).SendKeys(Config.Password);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
            loginButton.Click();
        }

        [When("the user logs in with an invalid {string} and {string}")]
        public void WhenTheUserLogsInWithAnInvalidAnd(string admin, string password)
        {
            _driver.FindElement(By.Id("username")).SendKeys(admin);
            _driver.FindElement(By.Id("password")).SendKeys(password);
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            Assert.IsTrue(_driver.Url.Contains("/admin"));
        }


        [Then("the user should see the admin panel")]
        public void ThenTheUserShouldSeeTheAdminPanel()
        {
            _driver = (IWebDriver)_scenarioContext["driver"];
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            //  Wait for URL change
            wait.Until(driver => driver.Url.Contains("/admin/rooms"));
            Assert.IsTrue(_driver.Url.Contains("/admin/rooms"), $"Expected URL to contain '/admin' but got {_driver.Url}");

            // Wait for admin element
            var roomsHeader = wait.Until(driver =>
            {
                var element = driver.FindElement(By.XPath("//a[text()='Rooms']"));
                return element.Displayed ? element : null;
            });

            Assert.IsTrue(roomsHeader.Displayed, "Admin panel (Rooms heading) was not visible.");
            Assert.IsTrue(_driver.Url.Contains("/admin/rooms"), $"Expected URL to contain '/admin/rooms' but got {_driver.Url}");

        }

        [Then("the user should see an error message")]
        public void ThenTheUserShouldSeeAnErrorMessage()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
            var error = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(text(), 'Invalid')]")));
            Assert.IsTrue(error.Displayed);
            Assert.IsTrue(error.Text.Contains("Invalid"));
        }

        [Then("the user logs out of the admin account")]
        public void ThenTheUserLogsOutOfTheAdminAccount()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));

            _driver = (IWebDriver)_scenarioContext["driver"];

            var logoutButton = _driver.FindElement(By.XPath("//button[text() = 'Logout']"));
            Assert.IsTrue(logoutButton.Displayed);
            logoutButton.Click();

            wait.Until(driver => driver.Url.Equals(Config.BaseUrl));

        }


    }
}
