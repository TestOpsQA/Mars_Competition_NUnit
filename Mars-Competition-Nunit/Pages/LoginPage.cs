using OpenQA.Selenium;
using AventStack.ExtentReports;
using OpenQA.Selenium.Support.UI;
using static Mars_Competition_Nunit.Utilities.WaitHelpers;
using Mars_Competition_Nunit.Data_Model;
using static Mars_Competition_Nunit.Utilities.CommonDriver;
using Mars_Competition_Nunit.Utilities;

namespace Mars_Competition_Nunit.Pages
{
    class LoginPage
    {
        IWebElement? SignInButton => driver?.FindElement(By.XPath("//*[@id=\"home\"]/div/div/div[1]/div/a"));
        IWebElement? UsernameField => driver?.FindElement(By.Name("email"));
        IWebElement? PasswordField => driver?.FindElement(By.Name("password"));
        IWebElement? LoginButton => driver?.FindElement(By.XPath("/html/body/div[2]/div/div/div[1]/div/div[4]/button"));
        private const string ProfileUrl = "http://localhost:5003/Account/Profile";
        // Class-level variables to hold login data
        private List<LoginModel> loginDataList;
        private string? email;
        private string? password;

        public LoginPage()
        {
            // Fetch login data once during object initialization
            loginDataList = JsonDataReader.GetLoginData();

            if (loginDataList.Count == 0)
            {
                throw new Exception("🚨 No login data found in the JSON file.");
            }

            // Initialize login credentials
            LoginModel loginData = loginDataList[0]; // Get first login data
            email = loginData?.Username;
            password = loginData?.Password;

        }
        public void Login()
        {
            var loginDataList = JsonDataReader.GetLoginData();

            if (loginDataList.Count == 0)
            {
                throw new Exception("🚨 No login data found in the JSON file.");
            }
            if (driver != null)
                try
                {
                    LoginModel loginData = loginDataList[0]; // Get first login data
                    string? email = loginData?.Username;
                    string? password = loginData?.Password;

                    SignInButton?.Click();
                    UsernameField?.SendKeys(email);
                    PasswordField?.SendKeys(password);
                    LoginButton?.Click();

                    TestContext.Out.WriteLine($"🔹 Logging in ");

                    WebDriverWait wait = new(driver, TimeSpan.FromSeconds(30));
                    wait.Until(d => d.Url.Contains(ProfileUrl));

                    // Log success
                    CommonMethods.LogTestStep("Successfully signed in", Status.Pass);
                }
                catch (Exception ex)
                {
                    CommonMethods.LogTestStep($"Sign-in failed: {ex.Message}", Status.Fail);
                    throw;
                }
                finally
                {
                    // Finalize reports
                    CommonMethods.FinalizeExtentReports();
                    TestContext.Out.WriteLine($"Reports Path: {ConstantHelpers.ReportsPath}");
                }
            TestContext.Out.WriteLine("Logged In successfully");
            Wait(5);
        }
    }
}
