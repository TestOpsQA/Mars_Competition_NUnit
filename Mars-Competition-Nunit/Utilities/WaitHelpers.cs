using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using static Mars_Competition_Nunit.Utilities.CommonDriver;

namespace Mars_Competition_Nunit.Utilities
{
   internal class WaitHelpers
    {
        private const int DefaultTimeoutInSeconds = 10;

        //Implicit Wait
        //Waits until the specified WebElement is visible on the page.

        public static void Wait(int second)
        {
            if (driver != null) // Check if the driver is initialized
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(second);
            }
            else
            {
                throw new InvalidOperationException("driver is not initialized.");
            }
        }
        public static void TurnOnWait()
        {
            if (driver != null)
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            }
        }

        public static void WaitUntilElementIsVisible(IWebElement element)
        {
            if (driver == null)
            {
                throw new InvalidOperationException("Webdriver is not initialized. Call InitializeDriver first.");
            }

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeoutInSeconds));

            // Wait until the element is visible (i.e., displayed)
            wait.Until(d => element != null && element.Displayed);
        }

        public static void WaitUntilElementIsClickable(IWebElement? element)
        {
            // EnsuredriverIsInitialized();
            if (driver == null)
            {
                throw new InvalidOperationException("Webdriver is not initialized. Call Initializedriver first.");
            }
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeoutInSeconds));
            wait.Until(driver => element != null && element.Enabled && element.Displayed);
        }

        public static void WaitUntilElementIsPresent(IWebElement? element)
        {
            //EnsuredriverIsInitialized();
            if (driver == null)
            {
                throw new InvalidOperationException("Webdriver is not initialized. Call Initializedriver first.");
            }
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeoutInSeconds));
            wait.Until(driver =>
            {
                // Explicitly check if element is not null and if it's displayed
                return element != null && element.Displayed;
            });
        }
        public static void WaitUntilElementIsInteractable(IWebElement element)
        {
            // EnsuredriverIsInitialized();
            if (driver == null)
            {
                throw new InvalidOperationException("Webdriver is not initialized. Call Initializedriver first.");
            }
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeoutInSeconds));
            wait.Until(driver =>
                element.Displayed &&    // Element is visible
                element.Enabled &&      // Element is enabled
                IsElementReadyForInteraction(driver, element) // Element is stable for interaction
            );
        }

        // Helper method to ensure the element is stable for interaction
        private static bool IsElementReadyForInteraction(IWebDriver driver, IWebElement element)
        {
            try
            {
                // Check if the element is visible and enabled
                return element.Displayed && element.Enabled;
            }
            catch (StaleElementReferenceException)
            {

                return false;
            }
        }

        public static void WaitUntilAlertIsPresent()
        {
            //EnsuredriverIsInitialized();
            if (driver == null)
            {
                throw new InvalidOperationException("Webdriver is not initialized. Call Initializedriver first.");
            }
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeoutInSeconds));
            wait.Until(driver =>
            {
                try
                {
                    IAlert alert = driver.SwitchTo().Alert();
                    return alert != null;
                }
                catch (NoAlertPresentException)
                {
                    return false; // Alert is not present
                }
            });
        }

        public static void EnsuredriverIsInitialized()
        {
            if (driver == null)
            {
                throw new InvalidOperationException("Webdriver is not initialized. Call Initializedriver first.");
            }
        }
    }
}
