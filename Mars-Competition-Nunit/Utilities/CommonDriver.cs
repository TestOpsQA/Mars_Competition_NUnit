using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Mars_Competition_Nunit.Pages;
using Mars_Competition_Nunit.Tests;
using static Mars_Competition_Nunit.Utilities.WaitHelpers;
using static Mars_Competition_Nunit.Utilities.CommonMethods;
using static Mars_Competition_Nunit.Utilities.ConstantHelpers;

namespace Mars_Competition_Nunit.Utilities
{
    class CommonDriver
    {
        public static IWebDriver? driver;
        private LoginPage? loginPageObj;
        private CertificationsPage? certificationsPageObj;
        private EducationPage? educationPageObj;
        private CertificationsTests? certificationsTestsObj;
        public static List<string> EducationCleanUp { get; set; } = new List<string>();
        public static List<string> CertificatesCleanUp { get; set; } = new List<string>();
        public int Browser { get; set; } = 2;


        [OneTimeSetUp]  // Runs once before all tests
        public void GlobalSetup()
        {
            InitializeExtentReports();
        }

        [SetUp]  // Runs before each test
        public void Setup()
        {
            if (driver == null)
            {
                Initialize();
            }
            if (driver != null)
            {
                loginPageObj = new LoginPage();
                certificationsPageObj = new CertificationsPage();
                educationPageObj = new EducationPage();
                certificationsTestsObj = new CertificationsTests();
                SigningIn();
                TurnOnWait();
            }

            // Start test report (HTML Report)
            Test = Extent?.CreateTest(TestContext.CurrentContext.Test.Name);
        }

        public void Initialize()
        {
            try
            {
                if (Browser != 1 && Browser != 2)
                {
                    throw new ArgumentException("Invalid Browser value. Expected 1 for Firefox or 2 for Chrome.");
                }

                switch (Browser)
                {
                    case 1:
                        driver = new FirefoxDriver();
                        break;
                    case 2:
                        driver = new ChromeDriver();
                        driver.Manage().Window.Maximize();
                        break;
                }

                if (driver == null)
                {
                    throw new NullReferenceException("Driver initialization failed.");
                }
            }
            catch (TimeoutException e)
            {
                Assert.Ignore(e.Message);
            }

            TurnOnWait();
            NavigateUrl();
        }

        public void SigningIn()
        {
            loginPageObj?.Login();  // Login action before every test
        }

        public void NavigateUrl()
        {
            driver?.Navigate().GoToUrl(Url);
        }

        [TearDown]  // Runs after each test
        public void TearDown()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            string testName = TestContext.CurrentContext.Test.Name;

            try
            {
                if (driver != null)
                {
                    string screenshotPath;
                    if (status == TestStatus.Failed)
                    {
                        screenshotPath = SaveScreenshot(driver, $"Fail_Screenshot_{testName}");
                        Test?.Fail("Test Failed. Screenshot Attached:")
                            .AddScreenCaptureFromPath(screenshotPath);
                    }
                    else
                    {
                        screenshotPath = SaveScreenshot(driver, $"Pass_Screenshot_{testName}");
                        Test?.Pass("Test Passed. Screenshot Attached:")
                            .AddScreenCaptureFromPath(screenshotPath);
                    }

                    // Generate XML Report
                    CreateXmlReport(ReportXMLPath);

                    // Cleanup certifications
                    if (CertificatesCleanUp != null && CertificatesCleanUp.Any())
                    {
                        TestContext.Out.WriteLine("Starting cleanup for certifications...");
                        foreach (var certificate in CertificatesCleanUp)
                        {
                            try
                            {
                                certificationsPageObj?.DeleteCertificatesData(certificate);
                            }
                            catch (Exception ex)
                            {
                                TestContext.Out.WriteLine($"Exception while deleting certificate '{certificate}': {ex.Message}");
                            }
                        }
                    }

                    // Cleanup education (if implemented)
                    if (EducationCleanUp != null && EducationCleanUp.Any())
                    {
                        TestContext.Out.WriteLine("Starting cleanup for education...");
                        foreach (var degree in EducationCleanUp)
                        {
                            educationPageObj?.DeleteEducationData(degree);
                        }
                    }

                    TestContext.Out.WriteLine("Cleanup process executed.");
                }
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"Unexpected error in TearDown: {ex.Message}");
            }
            driver?.Dispose();
            driver?.Quit();
            driver = null;
        }


        [OneTimeTearDown]  // Runs once after all tests
        public void GlobalTeardown()
        {
            FinalizeExtentReports(); // Ensure HTML reports are properly saved
        }
    }
}

