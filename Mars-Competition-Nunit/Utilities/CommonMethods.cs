using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Xml;
namespace Mars_Competition_Nunit.Utilities
{
    class CommonMethods
    {
        #region Screenshots
        public static string SaveScreenshot(IWebDriver driver, string screenShotFileName)
        {
            var folderLocation = ConstantHelpers.ScreenshotPath;

            // Ensure the Screenshots folder exists
            if (!Directory.Exists(folderLocation))
            {
                Directory.CreateDirectory(folderLocation);
            }

            // Construct the full file path with timestamp
            var fileName = Path.Combine(folderLocation, $"{screenShotFileName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.jpeg");

            // Save the screenshot
            var screenShot = ((ITakesScreenshot)driver).GetScreenshot();
            screenShot.SaveAsFile(fileName);

            return fileName;
        }
        #endregion

        #region Reports
        public static ExtentReports? Extent;
        public static ExtentTest? Test;

        public static void InitializeExtentReports()
        {
            var reportFolder = ConstantHelpers.ReportsPath;
            if (!Directory.Exists(reportFolder))
            {
                Directory.CreateDirectory(reportFolder);
            }

            // Initialize ExtentReports with HTML Reporter
            var htmlReportFilePath = Path.Combine(reportFolder, $"TestReport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.html");
            var sparkReporter = new ExtentSparkReporter(htmlReportFilePath)
            {
                Config =
                {
                    Theme = AventStack.ExtentReports.Reporter.Config.Theme.Standard,
                    ReportName = "Automation Test Results",
                    DocumentTitle = "Test Execution Report"
                }
            };

            Extent = new ExtentReports();
            Extent.AttachReporter(sparkReporter);
        }

        public static void FinalizeExtentReports()
        {
            Extent?.Flush();
        }
        public static void CreateXmlReport(string xmlReportFilePath)
        {
            var reportXMLFolder = ConstantHelpers.ReportXMLPath;

            // Ensure the folder exists
            if (!Directory.Exists(reportXMLFolder))
            {
                Directory.CreateDirectory(reportXMLFolder);
            }

            // Ensure xmlReportFilePath includes the correct directory
            xmlReportFilePath = Path.Combine(reportXMLFolder, $"TestReport_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xml");

            using (var writer = XmlWriter.Create(xmlReportFilePath, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("TestReports");

                writer.WriteStartElement("Test");
                writer.WriteElementString("TestName", TestContext.CurrentContext.Test.FullName);

                // Get test result
                string status = TestContext.CurrentContext.Result.Outcome.Status.ToString();
                writer.WriteElementString("Status", status);

                if (status == "Failed")
                {
                    writer.WriteElementString("ErrorMessage", TestContext.CurrentContext.Result.Message);
                }

                writer.WriteElementString("StartTime", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                writer.WriteElementString("EndTime", DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd_HH-mm-ss"));
                writer.WriteEndElement(); // Close "Test"

                writer.WriteEndElement(); // Close "TestReports"
                writer.WriteEndDocument();
            }
        }

        public static IEnumerable<string> GetFileNames()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo? parent = Directory.GetParent(currentDirectory)?.Parent?.Parent;

            if (parent == null)
            {
                throw new Exception("🚨 Unable to locate the project root directory.");
            }

            string testDataDirectory = Path.Combine(parent.FullName, "TestData");

            if (!Directory.Exists(testDataDirectory))
            {
                throw new Exception("🚨 TestData directory not found.");
            }

            return Directory.GetFiles(testDataDirectory, "*.json")
                            .Select(file => Path.GetFileName(file));
        }


        public static void LogTestStep(string stepName, Status status)
        {
            Test?.Log(status, stepName);
        }
        #endregion
    }
}
