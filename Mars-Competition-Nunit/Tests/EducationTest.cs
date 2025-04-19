using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using Mars_Competition_Nunit.Pages;
using Mars_Competition_Nunit.Utilities;
using static Mars_Competition_Nunit.Utilities.WaitHelpers;
using Mars_Competition_Nunit.Data_Model;

namespace Mars_Competition_Nunit.Tests
{
    class EducationTest : CommonDriver
    {
        EducationPage educationPage;

        [SetUp]
        public void SetUp()
        {
            educationPage = new EducationPage();
            educationPage.GoToEducationPage();
            educationPage.ClearEducationData();

        }

        [Test]
        public void AddValidEducation()
        {
            try
            {
                educationPage.LoadEducationData("AddValidEducationData.json");
                educationPage.AddEducation();
                AddToEducationCleanUp();
                string? expectedMessage = educationPage?.Message; // Replace with real logic
                string? actualMessage = educationPage?.PopUpMessage;
                Assert.That(actualMessage, Is.EqualTo(expectedMessage));
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine("Unable to add valid education entry" + ex);
            }
        }
        public void AddToEducationCleanUp()
        {
            string degreeName = GetDegreeName();

            if (!string.IsNullOrEmpty(degreeName))
            {
                EducationCleanUp.Add(degreeName);
            }
            else
            {
                Console.WriteLine("Skipping cleanup as no degree name was found.");
            }
        }


        [Test]
        public void AddMultipleValidEducationEntries()
        {
            if (driver != null)
            {
                try
                {
                    // Read the data and validate
                    List<EducationModel> educationData = JsonDataReader.GetEducationData("AddMultipleEducationData.json");
                    Assert.That(educationData, Is.Not.Null.And.Not.Empty, "No educations data found in the file.");

                    // Collect expected education names
                    List<string> expectedDegreesNames = educationData.Select(edu => edu.degree).ToList();

                    // Add educations
                    educationPage.AddMultipleValidEducation();

                    // Wait for educations to load - Ensure all educations are loaded
                    WaitUntilAllRowsAreVisible();

                    // Check if educations are loaded properly
                    if (educationPage?.EducationRows?.Count > 0)
                    {
                        TestContext.Out.WriteLine($"Number of education rows found: {educationPage.EducationRows.Count}");

                        // Capture actual education names
                        List<string> actualDegreesNames = new List<string>();

                        // Loop through education rows and collect their names
                        for (int i = 0; i < educationPage.EducationRows.Count; i++)
                        {
                            try
                            {
                                string degreeName = GetDegreeNameFromRow(i);
                                actualDegreesNames.Add(degreeName);
                                EducationCleanUp.Add(degreeName);
                                TestContext.Out.WriteLine($"Education degree {i + 1}: {degreeName}");
                            }
                            catch (Exception ex)
                            {
                                TestContext.Out.WriteLine($"Error retrieving education at index {i + 1}: {ex.Message}");
                            }
                        }

                        // Compare actual names with expected
                        Assert.That(actualDegreesNames, Is.EquivalentTo(expectedDegreesNames), "The degree names do not match.");

                        // Check success message
                        VerifyMessage();
                    }
                }
                catch (Exception ex)
                {
                    TestContext.Out.WriteLine("An error occurred while adding multiple education: " + ex.Message);
                    Assert.Fail("Test failed due to exception: " + ex.Message);
                }
            }
        }


        // Helper method to ensure all education rows are loaded
        private void WaitUntilAllRowsAreVisible()
        {

            // Adjust the wait time or use a condition that waits for the table to be fully populated with rows
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(d => educationPage.EducationRows.Count > 0 && educationPage.EducationRows.All(row => row.Displayed));
        }

        // Helper method to get education from row
        private string GetDegreeNameFromRow(int rowIndex)
        {
            IWebElement? degreeNameField = driver?.FindElement(By.XPath($"//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody[{rowIndex + 1}]/tr/td[4]"));
            if (degreeNameField != null)
            {
                WaitUntilElementIsVisible(degreeNameField);
                string degree = degreeNameField.Text.Trim();


                return degree;
            }
            else
            {
                throw new Exception($"Degree name element not found at index {rowIndex + 1}");
            }
        }
        private string GetDegreeName()
        {
            try
            {
                IWebElement? degreeNameField = driver?.FindElement(By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody[1]/tr/td[4]"));
                WaitUntilElementIsVisible(degreeNameField);
                return degreeNameField.Text.Trim();
            }
            catch (NoSuchElementException)
            {
                // Element not found, return empty string instead of failing
                Console.WriteLine("Degree name element not found. Skipping cleanup.");
                return string.Empty;
            }
        }


        // Helper method to verify success message

        private void VerifyMessage()
        {
            string? expectedMessage = educationPage?.Message;
            string? actualMessage = educationPage?.PopUpMessage;

            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The actual success message does not match the expected message.");
        }


        [Test]
        public void AddEducationWithoutUniversityName()
        {
            educationPage?.LoadEducationData("AddEducationWithoutUniversityName.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithoutTitle()
        {
            educationPage?.LoadEducationData("AddEducationWithoutTitle.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithoutCountryName()
        {
            educationPage?.LoadEducationData("AddEducationWithoutCountry.json");
            AddEducationTest();

        }
        [Test]
        public void AddEducationWithoutDegree()
        {
            educationPage?.LoadEducationData("AddEducationWithoutDegree.json");
            AddEducationTest();

        }
        [Test]
        public void AddEducationWithoutGraduationYear()
        {
            educationPage?.LoadEducationData("AddEducationWithoutGraduationYear.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithSpecialCharactersUniversity()
        {
            educationPage?.LoadEducationData("AddEducationSpecialCharactersUniversity.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithSpecialCharactersDegree()
        {
            educationPage?.LoadEducationData("AddEducationSpecialCharactersDegree.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithSpacesAsUniversity()
        {
            educationPage?.LoadEducationData("AddEducationSpacesAsUniversity.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithSpacesAsDegree()
        {
            educationPage?.LoadEducationData("AddEducationSpacesAsUniversity.json");
            AddEducationTest();
        }

        [Test]
        public void AddEducationWithMaliciousUniversityName()
        {
            educationPage?.LoadEducationData("AddEducationMaliciousUniversityName.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithMaliciousDegreeName()
        {
            educationPage?.LoadEducationData("AddEducationMaliciousDegreeName.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithLongUniversityName()
        {
            educationPage?.LoadEducationData("AddEducationLongUniversityName.json");
            AddEducationTest();
        }
        [Test]
        public void AddEducationWithLongDegreeName()
        {
            educationPage?.LoadEducationData("AddEducationLongDegreeName.json");
            AddEducationTest();
        }

        [Test]
        public void AddEducationWithVeryLongDegreeName()
        {
            educationPage?.LoadEducationData("AddEducationVeryLongDegreeName.json");
            AddEducationTest();
        }

        [Test]
        public void AddEducationWithVeryLongUniversityName()
        {
            educationPage?.LoadEducationData("AddEducationVeryLongUniversityName.json");
            AddEducationTest();
        }

        [Test]
        public void AddEducationWithDuplicateInfo()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("AddDuplicateEducationData.json");
            educationPage?.AddEducation();
            VerifyMessage();

        }
        [Test]
        public void AddEducationWithEmptyFields()
        {
            educationPage?.LoadEducationData("AddEducationEmptyFields.json");
            AddEducationTest();
        }

        public void AddEducationTest()
        {
            educationPage?.AddEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }

        [Test]
        public void UpdateEducation()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationValidData.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }

        [Test]
        public void UpdateEducationLongDegreeName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationLongDegreeName.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationLongUniversityName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationLongUniversityName.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationMaliciousDegreeName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationMaliciousDegreeName.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationMaliciousUniversityName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationMaliciousUniversityName.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationSpacesAsDegreeName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationSpacesAsDegree.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationSpacesAsUniversityName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationSpacesAsUniversity.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationSpecianCharactersAsDegreeName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationSpecialCharactersDegree.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationSpecialCharactersAsUniversityName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationSpecialCharactersUniversity.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationWithoutDegree()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationWithoutDegree.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationWithoutCountry()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationWithoutCountry.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationWithoutGraduationYear()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationWithoutGraduationYear.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }

        [Test]
        public void UpdateEducationWithoutTitle()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationWithoutTitle.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationWithoutUniversityName()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationWithoutUniversityName.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateEducationWithEmptyFields()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateEducationEmptyFields.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void UpdateDuplicateEducation()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.LoadEducationData("UpdateDuplicateEducation.json");
            educationPage?.UpdateEducation();
            AddToEducationCleanUp();
            VerifyMessage();

        }
        [Test]
        public void DeleteEducation()
        {
            educationPage?.LoadEducationData("AddValidEducationData.json");
            educationPage?.AddEducation();
            educationPage?.DeleteAddedEducation();
            AddToEducationCleanUp();
            string? expectedMessage = "Education entry successfully removed";
            string? actualMessage = educationPage?.PopUpMessage;
            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The actual success message does not match the expected message.");
        }
    }
}

