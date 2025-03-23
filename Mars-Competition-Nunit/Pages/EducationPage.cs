using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Mars_Competition_Nunit.Data_Model;
using Mars_Competition_Nunit.Utilities;
using static Mars_Competition_Nunit.Utilities.CommonDriver;
using static Mars_Competition_Nunit.Utilities.WaitHelpers;



namespace Mars_Competition_Nunit.Pages
{
    class EducationPage
    {

        public IWebElement? EducationTab => driver?.FindElement(By.XPath("//a[normalize-space()='Education']"));
        public IWebElement? AddNewButton => driver?.FindElement(By.XPath(" //*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/thead/tr/th[6]/div"));
        public IWebElement? UniversityNameField => driver?.FindElement(By.Name("instituteName"));
        public IWebElement? CountryNameDropdown => driver?.FindElement(By.Name("country"));
        public IWebElement? TitleDropdown => driver?.FindElement(By.Name("title"));
        public IWebElement? DegreeField => driver?.FindElement(By.Name("degree"));
        public IWebElement? GraduationYearDropdown => driver?.FindElement(By.Name("yearOfGraduation"));
        public IWebElement? AddButton => driver?.FindElement(By.XPath("//input[@value='Add']"));
        public IWebElement? DeleteButton => driver?.FindElement(By.XPath("//div[@data-tab='third']//tbody[last()]/tr[1]/td[4]/span[2]/i"));
        public IWebElement? EditButton => driver?.FindElement(By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody/tr/td[6]/span[1]/i"));
        public IWebElement? UpdateButton => driver?.FindElement(By.XPath("//input[@value=\"Update\"]"));
        public IWebElement? CancelButton => driver?.FindElement(By.XPath("//input[@value=\"Cancel\"]"));
        public IWebElement? PopUpBox => driver?.FindElement(By.ClassName("ns-box-inner"));
        public IReadOnlyCollection<IWebElement>? DeleteButtons = driver?.FindElements(By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody[last()]/tr/td[6]/span[2]/i"));
        public IWebElement? PopUpCloseButton => driver?.FindElement(By.ClassName("ns-close"));
        public IWebElement? EducationTable => driver?.FindElement(By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody"));
        public IReadOnlyCollection<IWebElement>? EducationRows => driver?.FindElements(By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody/tr"));
        public string? PopUpMessage { get; set; }
        private List<EducationModel>? educationDataList;
        public string? Country;
        public string? UniversityName;
        public string? Title;
        public string? Degree;
        public string? GraduationYear;
        public string? Message;
        public int MaxRetries = 5;

        public void LoadEducationData(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new Exception("🚨 fileName is not set.");
                }

                // Get education data for the selected file
                educationDataList = JsonDataReader.GetEducationData(fileName);

                if (educationDataList == null || educationDataList.Count == 0)
                {
                    throw new Exception("🚨 No education data found.");
                }

                // Initialize education data
                EducationModel educationData = educationDataList[0];
                Country = educationData?.Country;
                UniversityName = educationData?.UniversityName;
                Title = educationData?.Title;
                Degree = educationData?.Degree;
                GraduationYear = educationData?.GraduationYear;
                Message = educationData?.Message;

                TestContext.Out.WriteLine($"🎉 Successfully loaded education data:{Title}, {Country}, {UniversityName}, {Degree}, {GraduationYear}");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"❌ ERROR: {ex.Message}");
                throw;
            }
        }

        public void GoToEducationPage()
        {
            WaitUntilElementIsClickable(EducationTab);
            EducationTab?.Click();
        }

        public void AddEducation()
        {
            var educationDataList = JsonDataReader.GetEducationData("AddValidEducationData.json");
            WaitUntilElementIsClickable(AddNewButton);
            AddNewButton?.Click();
            CountryNameDropdown?.SendKeys(Country);
            UniversityNameField?.SendKeys(UniversityName);
            TitleDropdown?.SendKeys(Title);
            DegreeField?.SendKeys(Degree);
            GraduationYearDropdown?.SendKeys(GraduationYear);
            AddButton?.Click();
            PopUpMessage = GetPopUpMessage();
            Wait(2);
            ClosePopUp();
            CancelOperation();
        }
        public void CancelOperation()
        {
            try
            {
                // Ensure element exists before checking Displayed
                if (CancelButton != null && CancelButton.Displayed)
                {
                    WaitUntilElementIsClickable(CancelButton);
                    CancelButton.Click();
                }
            }
            catch (NoSuchElementException)
            {
                TestContext.Out.WriteLine("Cancel button not found. Continuing test...");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
            Wait(2);
        }

        public void AddMultipleValidEducation()
        {
            try
            {
                // Load Education data from the JSON file
                var educationDataList = JsonDataReader.GetEducationData("AddMultipleEducationData.json");

                // Check if the Education data list is null or empty
                if (educationDataList == null || !educationDataList.Any())
                {
                    throw new Exception("🚨 No education data found in the JSON file.");
                }

                // Loop through each Education in the list
                foreach (var education in educationDataList)
                {
                    Country = education.Country;
                    UniversityName = education.UniversityName;
                    Title = education.Title;
                    Degree = education.Degree;

                    GraduationYear = education.GraduationYear;
                    Message = education.Message;

                    // Ensure all fields are not null
                    WaitUntilElementIsClickable(AddNewButton);
                    AddNewButton?.Click();
                    CountryNameDropdown?.SendKeys(Country);
                    if (UniversityNameField != null)
                        WaitUntilElementIsInteractable(UniversityNameField);

                    UniversityNameField?.SendKeys(UniversityName);
                    TitleDropdown?.SendKeys(Title);
                    DegreeField?.SendKeys(Degree);
                    GraduationYearDropdown?.SendKeys(GraduationYear);
                    AddButton?.Click();
                    PopUpMessage = GetPopUpMessage();
                    ClosePopUp();
                    // Verify that the required data is available
                    if (string.IsNullOrWhiteSpace(Country) || string.IsNullOrWhiteSpace(UniversityName)
                        || string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Degree)
                        || string.IsNullOrWhiteSpace(GraduationYear))
                    {
                        throw new Exception("🚨 Missing required education data.");
                    }

                }

            }
            catch (Exception ex)
            {
                // Output any exceptions that occur
                TestContext.Out.WriteLine("Unable to add valid education entries: " + ex);
            }

        }
        public void UpdateEducation()
        {
            educationDataList = JsonDataReader.GetEducationData("UpdateEducationValidData.json");
            WaitUntilElementIsClickable(AddNewButton);
            EditButton?.Click();
            if (CountryNameDropdown != null && UniversityNameField != null && TitleDropdown != null && GraduationYearDropdown != null && DegreeField != null)
            {
                CountryNameDropdown.Click();
                ClearDropdown(CountryNameDropdown); // Clears the Country dropdown
                CountryNameDropdown.SendKeys(Country);
                ClearCompleteField(UniversityNameField);
                UniversityNameField?.SendKeys(UniversityName);
                TitleDropdown.Click();
                ClearDropdown(TitleDropdown); // Clears the Title dropdown
                TitleDropdown?.SendKeys(Title);
                DegreeField.Click();
                ClearCompleteField(DegreeField);
                DegreeField?.SendKeys(Degree);
                Wait(2);
                GraduationYearDropdown.Click();
                ClearDropdown(GraduationYearDropdown);
                GraduationYearDropdown?.SendKeys(GraduationYear);
                UpdateButton?.Click();
                PopUpMessage = GetPopUpMessage();
                CancelOperation();
                Wait(2);
                ClosePopUp();
            }
        }

        public void ClearDropdown(IWebElement dropdownElement)
        {
            SelectElement select = new SelectElement(dropdownElement);
            select.SelectByIndex(0); // Select the first option
        }

        public void ClearCompleteField(IWebElement element)
        {
            element?.Click();
            element?.SendKeys(Keys.Control + "a");
            element?.SendKeys(Keys.Delete);
            element?.SendKeys("");
            element?.SendKeys(Keys.Backspace);
        }
        public void DeleteAddedEducation()
        {
            Wait(3);
            RefreshRows();

            if (EducationRows == null || EducationRows.Count == 0)
            {
                TestContext.Out.WriteLine("⚠️ No education entries found. Nothing to delete.");
                return;
            }

            try
            {
                var lastRow = EducationRows.Last(); // Get the last entered education row
                var degreeNameField = lastRow.FindElement(By.XPath("./td[4]"));
                string lastDegreeName = degreeNameField.Text.Trim();
                var deleteButton = lastRow.FindElement(By.XPath("./td[6]//span[2]/i"));
                deleteButton?.Click();
                PopUpMessage = GetPopUpMessage();
                Wait(1);
                ClosePopUp();
                Wait(3);

                TestContext.Out.WriteLine($"✅ Successfully deleted last entered education entry: {lastDegreeName}");
            }
            catch (StaleElementReferenceException)
            {
                TestContext.Out.WriteLine("⚠️ Stale element detected. Refreshing and retrying...");
                RefreshRows();
            }
            catch (NoSuchElementException)
            {
                TestContext.Out.WriteLine("❌ Delete button not found for the last entered education entry.");
            }
        }

        public void ClearEducationData()
        {
            int retryCount = 0;
            int maxRetries = 5; // Limit retries for stale elements

            while (true)
            {
                try
                {
                    // Re-find all delete buttons each time
                    var deleteButtons = driver?.FindElements(By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[4]/div/div[2]/div/table/tbody[last()]/tr/td[6]/span[2]/i"));

                    if (deleteButtons == null || deleteButtons.Count == 0)
                    {
                        TestContext.Out.WriteLine("✅ No more education entries to delete.");
                        break; // Exit when no delete buttons are left
                    }
                    else
                    {
                        TestContext.Out.WriteLine("✅ Deleting Education entry");
                        // Always target the last delete button
                        IWebElement deleteBtn = deleteButtons.Last();

                        WaitUntilElementIsClickable(deleteBtn);
                        deleteBtn.Click();
                        Wait(1);
                        // Reset retry count after a successful deletion
                        retryCount = 0;
                    }
                }
                catch (StaleElementReferenceException)
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        TestContext.Out.WriteLine("🚨 Too many stale element errors, stopping cleanup.");
                        break;
                    }
                }
                catch (NoSuchElementException)
                {
                    TestContext.Out.WriteLine("⚠ No delete button found, exiting.");
                    break; // Exit if no delete button is found
                }
                catch (WebDriverTimeoutException)
                {
                    TestContext.Out.WriteLine("⏳ Timeout waiting for delete button, exiting.");
                    break; // Exit if waiting for the button fails
                }
                ClosePopUp();
            }
        }
        public string GetPopUpMessage()
        {
            try
            {
                if (driver == null)
                {
                    throw new InvalidOperationException("Driver is not initialized.");
                }

                // Wait for the pop-up 
                WaitUntilElementIsPresent(PopUpBox);
                string? message = PopUpBox?.Text?.Trim();
                TestContext.Out.WriteLine($"Captured pop up message: {message}");
                if (message == null)
                {
                    message = "No message found";
                }
                return message;

            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"Error getting the message: {ex.Message}");
                return string.Empty;
            }
        }
        public void ClosePopUp()
        {
            try
            {
                // Ensure element exists before checking Displayed
                if (PopUpCloseButton != null && PopUpCloseButton.Displayed)
                {
                    WaitUntilElementIsInteractable(PopUpCloseButton);
                    PopUpCloseButton.Click();
                }
            }
            catch (NoSuchElementException)
            {
                TestContext.Out.WriteLine("Pop-up close button not found. Continuing test...");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"An unexpected error occurred while closing pop up: {ex.Message}");
            }
        }
        public void DeleteEducationData(string degree)
        {
            Wait(3);
            int retryCount = 0;
            const int maxRetries = 5;
            bool isAnyEducationDeleted = false; // Track if any deletion happened

            while (true)
            {
                try
                {
                    RefreshRows();

                    if (EducationRows == null || EducationRows.Count == 0)
                    {
                        break; // Stop checking if no rows are found
                    }

                    bool isEducationDeletedInThisIteration = false;

                    foreach (var row in EducationRows.ToList())
                    {
                        try
                        {
                            var degreeNameField = row.FindElement(By.XPath("./td[4]"));
                            string degreeName = degreeNameField.Text.Trim();

                            if (degreeName.Equals(degree, StringComparison.OrdinalIgnoreCase))
                            {
                                var deleteButton = row.FindElement(By.XPath("./td[6]//span[2]/i"));
                                deleteButton?.Click();
                                PopUpMessage = GetPopUpMessage();
                                Wait(1);
                                ClosePopUp();
                                Wait(3);
                                isEducationDeletedInThisIteration = true;
                                isAnyEducationDeleted = true;
                                break; // Stop after deleting and refresh rows
                            }
                        }
                        catch (StaleElementReferenceException)
                        {
                            TestContext.Out.WriteLine("⚠️ Stale element detected inside foreach loop. Skipping to next iteration...");
                            continue;
                        }
                    }

                    // If no deletion happened in this pass, done checking
                    if (!isEducationDeletedInThisIteration)
                    {
                        break;
                    }
                }
                catch (StaleElementReferenceException)
                {
                    retryCount++;
                    TestContext.Out.WriteLine($"⚠️ Stale element detected. Refreshing and retrying ({retryCount}/{maxRetries})...");

                    if (retryCount >= maxRetries)
                    {
                        TestContext.Out.WriteLine("❌ Maximum retries reached. Exiting delete process.");
                        break;
                    }

                    RefreshRows();
                }
                catch (NoSuchElementException)
                {
                    TestContext.Out.WriteLine($"⚠️ No delete button found for '{degree}'. Exiting.");
                    break;
                }
            }

            // **Log cleanup message only at the very end, not after each deletion**
            RefreshRows(); // One final refresh to check if all are deleted
            Wait(1);
            if (EducationRows == null || EducationRows.Count == 0)
            {
                if (isAnyEducationDeleted)
                {
                    TestContext.Out.WriteLine("✅ Cleanup process completed successfully.");
                }
                else
                {
                    TestContext.Out.WriteLine("✅ No education entries present, cleanup not needed.");
                }
            }
        }

        private void RefreshRows()
        {
            try
            {
                // Refresh the rows by finding them again after the deletion

                TestContext.Out.WriteLine($"Refreshed rows count: {EducationRows?.Count}");
            }
            catch (Exception e)
            {
                TestContext.Out.WriteLine($"Error refreshing rows: {e.Message}");
            }
        }
    }
}
