using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Mars_Competition_Nunit.Data_Model;
using Mars_Competition_Nunit.Pages;
using Mars_Competition_Nunit.Utilities;
using static Mars_Competition_Nunit.Utilities.WaitHelpers;

namespace Mars_Competition_Nunit.Tests
{
    class CertificationsTests : CommonDriver
    {

        CertificationsPage certificationsPage;


        public List<string>? addedCertificates = new List<string>();

        [SetUp]
        public void SetUp()
        {

            certificationsPage = new CertificationsPage();
            certificationsPage.GoToCertificateTab();
            certificationsPage.ClearCertificationsData();

        }

        [Test]
        public void AddValidCertificate()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddValidCertificateData.json");
                certificationsPage.AddCertificate();
                AddCertificateToCleanUp();
                string expectedMessage = certificationsPage.certificateName + certificationsPage.message; // Replace with real logic
                string? actualMessage = certificationsPage?.PopUpMessage;
                Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The Actual message is not equal to expected message");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("Unable to add valid certificate " + ex);
            }
        }
        private void AddCertificateToCleanUp()
        {
            string? addedCertificateName = certificationsPage.certificateName;

            CertificatesCleanUp.Add(addedCertificateName);
        }
        private void AddCertificateFromToCleanUp()
        {
            string? addedCertificateFrom = certificationsPage.certificateFrom;  // Use the actual certificate name logic here
            CertificatesCleanUp.Add(addedCertificateFrom);
        }


        [Test]
        public void AddMultipleValidCertificate()
        {
            if (driver != null)
            {
                try
                {
                    // Read the data and validate
                    List<CertificateModel> certificatesData = JsonDataReader.GetCertificateData("AddMultipleCertifiatesData.json");
                    Assert.That(certificatesData, Is.Not.Null.And.Not.Empty, "No certificates data found in the file.");

                    // Collect expected certificate names
                    List<string> expectedCertificateNames = certificatesData.Select(certificate => certificate.CertificateName).ToList();

                    // Add certificates
                    certificationsPage.AddMultipleValidCertificates();

                    //Ensure all certificates are loaded
                    WaitUntilAllRowsAreVisible();

                    // Check if certificates are loaded properly
                    if (certificationsPage?.Rows?.Count > 0)
                    {
                        TestContext.Out.WriteLine($"Number of certificate rows found: {certificationsPage.Rows.Count}");

                        // Capture actual certificate names
                        List<string> actualCertificateNames = new List<string>();

                        // Loop through certificate rows and collect their names
                        for (int i = 0; i < certificationsPage.Rows.Count; i++)
                        {
                            try
                            {
                                string certName = GetCertificateNameFromRow(i);
                                actualCertificateNames.Add(certName);
                                CertificatesCleanUp.Add(certName);
                                TestContext.Out.WriteLine($"Certificate {i + 1}: {certName}");
                            }
                            catch (Exception ex)
                            {
                                TestContext.Out.WriteLine($"Error retrieving certificate at index {i + 1}: {ex.Message}");
                            }
                        }

                        // Compare actual names with expected
                        Assert.That(actualCertificateNames, Is.EquivalentTo(expectedCertificateNames), "The certificate names do not match.");

                        // Check success message
                        VerifySuccessMessage();
                    }
                }
                catch (Exception ex)
                {
                    TestContext.Out.WriteLine("An error occurred while adding multiple certificates: " + ex.Message);
                    Assert.Fail("Test failed due to exception: " + ex.Message);
                }
            }
        }

        // Helper method to ensure all certificate rows are loaded
        private void WaitUntilAllRowsAreVisible()
        {
            // Adjust the wait time or use a condition that waits for the table to be fully populated with rows
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(d => certificationsPage.Rows.Count > 0 && certificationsPage.Rows.All(row => row.Displayed));
        }

        // Helper method to get certificate name from row
        private string GetCertificateNameFromRow(int rowIndex)
        {
            IWebElement? certificateNameField = driver?.FindElement(By.XPath($"//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[5]/div[1]/div[2]/div/table/tbody[{rowIndex + 1}]/tr/td[1]"));
            WaitUntilElementIsVisible(certificateNameField);
            if (certificateNameField != null)
            {
                return certificateNameField.Text.Trim();
            }
            else
            {
                throw new Exception($"Certificate name element not found at index {rowIndex + 1}");
            }
        }

        // Helper method to verify success message
        private void VerifySuccessMessage()
        {
            string? expectedMessage = certificationsPage?.certificateName + certificationsPage?.message;
            string? actualMessage = certificationsPage?.PopUpMessage;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The actual success message does not match the expected message.");
        }
        private void VerifyMessage()
        {
            string? expectedMessage = certificationsPage?.message;
            string? actualMessage = certificationsPage?.PopUpMessage;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The actual success message does not match the expected message.");
        }



        [Test]
        public void DeleteCertificate()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddValidCertificateData.json");
                certificationsPage.AddCertificate();
                certificationsPage.DeleteAddedCertificate();

                string? expectedMessage = certificationsPage?.certificateName + "has been deleted from your certification";
                string? actualMessage = certificationsPage?.PopUpMessage;

                Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The actual success message does not match the expected message.");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("Exception while deleting added certificate" + ex);
            }
        }

        [Test]
        public void AddCertificateWithoutCertificateName()
        {
            try
            {
                certificationsPage.LoadCertificateData("AddCertificateWithoutCertificateName.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate without certificate Name: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        [Test]
        public void AddCertificateWithoutCertifiedFrom()
        {
            try
            {
                certificationsPage.LoadCertificateData("AddCertificateWithoutCertifiedFrom.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate without certified from: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        [Test]
        public void AddCertificateWithoutCertificationYear()
        {
            try
            {
                certificationsPage.LoadCertificateData("AddCertificateWithoutCertificationYear.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate without certification year: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        [Test]
        public void AddCertificateWithEmptyFiels()
        {
            try
            {
                certificationsPage.LoadCertificateData("AddCertificateEmptyFields.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with all fields empty: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        [Test]
        public void AddCertificateWithSpecialCharactersCertificateName()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateSpecialCharactersCertificateName.json");
                AddCertificateSuccessTests();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with special characters: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }


        [Test]
        public void AddCertificateWithSpecialCharactersCertificateFrom()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateSpecialChractersCertifiedFrom.json");
                AddCertificateSuccessTests();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with special characters certification from: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }



        [Test]
        public void AddCertificateWithSpacesAsCertificateName()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateSpacesAsCertificateName.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with spaces: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }


        [Test]
        public void AddCertificateWithSpacesAsCertifiedFrom()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateSpacesAsCertificateFrom.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with special characters: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }



        [Test]
        public void AddCertificateWithMaliciousCertificateName()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateMaliciousCertificateName.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with malicious certificate Name: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }



        [Test]
        public void AddCertificateWithMaliciousCertificateFrom()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateMaliciousCertificateFrom.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with malicious certified from: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }


        [Test]
        public void AddCertificateWithLongCertificateName()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateLongCetificateName.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with long certificate name: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        [Test]
        public void AddCertificateWithLongCertificateFrom()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateLongCertificateFrom.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with long certificate from: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        [Test]
        public void AddCertificateWithVeryLongCertificateName()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateVeryLongCertificateName.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with very long certificate name: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }
        [Test]
        public void AddCertificateWithVeryLongCertificateFrom()
        {
            try
            {

                certificationsPage.LoadCertificateData("AddCertificateVeryLongCertificateFrom.json");
                certificationsPage.AddCertificateWithLongText();
                AddCertificateToCleanUp();
                VerifyMessage();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with very long certificate from: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }



        [Test]
        public void AddCertificateWithDuplicateCertificateData()
        {
            try
            {
                certificationsPage.LoadCertificateData("AddValidCertificateData.json");
                certificationsPage.AddCertificate();

                certificationsPage.LoadCertificateData("AddDuplicteCertificateData.json");
                AddCertificateTest();
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("An error occurred while adding certificate with duplicate certificate Date: " + ex.Message);
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        public void AddCertificateTest()
        {
            certificationsPage.AddCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();

        }
        public void AddCertificateSuccessTests()
        {
            certificationsPage.AddCertificate();
            AddCertificateToCleanUp();
            VerifySuccessMessage();

        }

        [Test]
        public void UpdateCertificateValidData()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateValidData.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifySuccessMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateEmptyFields()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateEmptyFields.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateLongCertificateFromName()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateLongCertificateFrom.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateLongCertificateName()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateLongCetificateName.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateMaliciuosCertificateFrom()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateMaliciousCertificateFrom.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateMaliciousCertificateName()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateMaliciousCertificateName.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateSpacesAsCertificateFrom()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateSpacesAsCertificateFrom.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateSpacesAsCertificateName()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateSpacesAsCertificateName.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateSpecialCharactersCertificateName()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateSpecialCharactersCertificateName.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifySuccessMessage();
            Wait(2);

        }
        [Test]
        public void UpdateWithSpecialCharactersCertificateFrom()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateSpecialChractersCertifiedFrom.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifySuccessMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateEmptyCertificateName()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateWithoutCertificateName.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateEmptyCertificationFrom()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateWithoutCertifiedFrom.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateEmptyCertificationYear()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateWithoutCertificationYear.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
        [Test]
        public void UpdateCertificateWithDuplicateData()
        {
            certificationsPage.LoadCertificateData("AddValidCertificateData.json");
            certificationsPage.AddCertificate();
            certificationsPage.LoadCertificateData("UpdateCertificateEmptyFields.json");
            certificationsPage.UpdateCertificate();
            AddCertificateToCleanUp();
            VerifyMessage();
            Wait(2);

        }
    }
}
