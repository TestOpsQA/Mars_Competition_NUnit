using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Mars_Competition_Nunit.Data_Model;
using Mars_Competition_Nunit.Utilities;
using static Mars_Competition_Nunit.Utilities.CommonDriver;
using static Mars_Competition_Nunit.Utilities.WaitHelpers;


namespace Mars_Competition_Nunit.Pages
{
   internal class CertificationsPage
    {
        IWebElement? CertificateTab => driver?.FindElement(By.XPath("//a[normalize-space()='Certifications']"));
        IWebElement? CertificateNameTextbox => driver?.FindElement(By.Name("certificationName"));
        IWebElement? CertificateFromTextbox => driver?.FindElement(By.Name("certificationFrom"));
        IWebElement? AddNewButton => driver?.FindElement(By.XPath("//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[5]/div[1]/div[2]/div/table/thead/tr/th[4]/div"));
        public IWebElement? ChooseCertificateYearDropdown => driver?.FindElement(By.XPath("//select[@name='certificationYear']"));
        public IWebElement? AddButton => driver?.FindElement(By.XPath("//section[2]//form//div[2]//div[3]/input[1]"));
        public IReadOnlyCollection<IWebElement>? DeleteButtons => driver?.FindElements(By.XPath("//i[@class=\"remove icon\"]"));
        public IWebElement? PopUpBox => driver?.FindElement(By.ClassName("ns-box-inner"));
        public IWebElement? PopUpCloseButton => driver?.FindElement(By.ClassName("ns-close"));
        public IWebElement? DeleteButton => driver?.FindElement(By.XPath("//div[@data-tab='fourth']//tbody[last()]/tr[1]/td[4]/span[2]/i"));
        public IWebElement? EditButton => driver?.FindElement(By.XPath("//div[@data-tab='fourth']//tbody[last()]/tr[1]/td[4]/span[1]/i"));
        public IWebElement? UpdateButton => driver?.FindElement(By.XPath("//input[@Value=\"Update\"]"));
        public IWebElement? CancelButton => driver?.FindElement(By.XPath("//input[@Value=\"Cancel\"]"));
        public IWebElement? CertificatesTable => driver?.FindElement(By.XPath("\r\n//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[5]/div[1]/div[2]/div/table/tbody"));
        public IReadOnlyCollection<IWebElement>? Rows => driver?.FindElements(By.XPath("\r\n//*[@id=\"account-profile-section\"]/div/section[2]/div/div/div/div[3]/form/div[5]/div[1]/div[2]/div/table/tbody/tr"));
        public string? PopUpMessage { get; set; }

        // Class-level variables to hold login data
        private List<CertificateModel>? certificateDataList;
        public string? certificateName;
        public string? certificateFrom;
        public string? year;
        public string? message;
        public string? fileName { get; set; }



        public void LoadCertificateData(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    throw new Exception("🚨 fileName is not provided.");
                }

                this.fileName = fileName;  // Store the file name

                certificateDataList = JsonDataReader.GetCertificateData(fileName);

                if (certificateDataList == null || certificateDataList.Count == 0)
                {
                    throw new Exception("🚨 No certificate data found in the file.");
                }

                CertificateModel certificateData = certificateDataList[0]; // Get the first record
                certificateName = certificateData?.certificateName;
                certificateFrom = certificateData?.certificateFrom;
                year = certificateData?.certificateYear;
                message = certificateData?.message;

                TestContext.Out.WriteLine($"🎉 Successfully loaded certificate data from: {fileName}");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"❌ ERROR in LoadCertificateData: {ex.Message}");
                throw;
            }
        }

        public void GoToCertificateTab()
        {
            WaitUntilElementIsClickable(CertificateTab);
            CertificateTab?.Click();

        }


        public void AddCertificate()
        {
            certificateDataList = JsonDataReader.GetCertificateData("AddValidCertificateData.json");
            WaitUntilElementIsClickable(AddNewButton);
            AddNewButton?.Click();
            CertificateNameTextbox?.SendKeys(certificateName);
            CertificateFromTextbox?.SendKeys(certificateFrom);
            Wait(2);
            if (year != null)
                ChooseCertificateYear(year);
            AddButton?.Click();
            PopUpMessage = GetPopUpMessage();
            CancelOperation();
            Wait(2);
            ClosePopUp();
            Wait(1);
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
                    message = "No message found";  // Default value 
                }
                return message;
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"Error getting the message: {ex.Message}");
                return string.Empty;
            }
        }

        public void AddMultipleValidCertificates()
        {
            try
            {
                // Load certificate data from the JSON file
                certificateDataList = JsonDataReader.GetCertificateData("AddMultipleCertifiatesData.json");

                // Check if the certificate data list is null or empty
                if (certificateDataList == null || !certificateDataList.Any())
                {
                    throw new Exception("🚨 No certificate data found in the JSON file.");
                }

                // Loop through each certificate in the list
                foreach (var cert in certificateDataList)
                {
                    // Ensure all fields are not null
                    certificateName = cert.certificateName;
                    certificateFrom = cert.certificateFrom;
                    year = cert.certificateYear;
                    message = cert.message;
                    // Verify that the required data is available
                    if (string.IsNullOrWhiteSpace(certificateName) || string.IsNullOrWhiteSpace(certificateFrom) || string.IsNullOrWhiteSpace(year))
                    {
                        throw new Exception("🚨 Missing required certificate data.");
                    }

                    // Wait for the "Add New" button to be clickable and click it
                    WaitUntilElementIsClickable(AddNewButton);
                    AddNewButton?.Click();

                    // Input the certificate details into the form
                    CertificateNameTextbox?.SendKeys(certificateName);
                    CertificateFromTextbox?.SendKeys(certificateFrom);
                    ChooseCertificateYear(year); // Choose the year if it's not null

                    // Click the "Add" button to submit the data
                    AddButton?.Click();

                    // Capture the success message and close the pop-up
                    PopUpMessage = GetPopUpMessage();
                    Wait(2);
                    ClosePopUp();
                    Wait(5);
                }
            }
            catch (Exception ex)
            {
                // Output any exceptions that occur
                TestContext.Out.WriteLine("Unable to add valid certificate entries: " + ex);
            }
        }
        public void AddCertificateWithLongText()
        {
            certificateDataList = JsonDataReader.GetCertificateData("AddCertificateVeryLongCertificateName.json");
            WaitUntilElementIsClickable(AddNewButton);
            AddNewButton?.Click();

            PasteText(CertificateNameTextbox, certificateName);
            // EnterLongText(CertificateFromTextbox, certificateFrom);

            CertificateFromTextbox?.SendKeys(certificateFrom);
            Wait(2);
            if (year != null)
                ChooseCertificateYear(year);
            AddButton?.Click();

            PopUpMessage = GetPopUpMessage();
            TestContext.Out.WriteLine("Validation Message: " + PopUpMessage);

            CancelOperation();
            Wait(2);
            ClosePopUp();
            Wait(1);
        }
        public void PasteText(IWebElement element, string text)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("navigator.clipboard.writeText(arguments[0]);", text);
            element.Click();
            element.SendKeys(Keys.Control + "v");
        }

        public void SetTextUsingJavaScript(IWebElement element, string text)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].value = arguments[1];", element, text);
        }

        public void EnterLongTextInChunks(IWebElement element, string text, int chunkSize = 100)
        {
            try
            {
                element.Clear();
                Wait(1);
                for (int i = 0; i < text.Length; i += chunkSize)
                {
                    string chunk = text.Substring(i, Math.Min(chunkSize, text.Length - i));
                    element.SendKeys(chunk);
                    Wait(5);
                }
                Console.WriteLine($"Entered {text.Length} characters in chunks into {element.GetAttribute("name")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error entering long text: " + ex.Message);
            }
        }

        public void UpdateCertificate()
        {

            certificateDataList = JsonDataReader.GetCertificateData("UpdateCertificateValidData.json");
            EditButton?.Click();
            if (CertificateNameTextbox != null && CertificateFromTextbox != null && ChooseCertificateYearDropdown != null)
            {
                CertificateNameTextbox.Click();
                ClearCompleteField(CertificateNameTextbox);
                CertificateNameTextbox?.SendKeys(certificateName);

                Wait(2);
                CertificateFromTextbox.Click();
                ClearCompleteField(CertificateFromTextbox);
                CertificateFromTextbox?.SendKeys(certificateFrom);

                ChooseCertificateYearDropdown.Click();
                ClearDropdown(ChooseCertificateYearDropdown);
                ChooseCertificateYearDropdown?.SendKeys(year);

                UpdateButton?.Click();
                CancelOperation();
                Wait(1);
                PopUpMessage = GetPopUpMessage();
                ClosePopUp();
                Wait(2);
            }
        }
        public void ClearCompleteField(IWebElement field)
        {
            field?.SendKeys(Keys.Control + "a");
            field?.SendKeys(Keys.Delete);
            field?.SendKeys(" ");
            field?.SendKeys(Keys.Backspace);
            field?.SendKeys(Keys.Tab);
        }
        public void ClearDropdown(IWebElement dropdownElement)
        {
            SelectElement select = new SelectElement(dropdownElement);
            select.SelectByIndex(0); // Select the first option
        }

        public void ChooseCertificateYear(string year)
        {
            //Choose Lanuage Level
            ChooseCertificateYearDropdown?.Click();
            if (ChooseCertificateYearDropdown == null)
            {
                throw new Exception("dropdownLanguage is null. Ensure it is initialized before use.");
            }
            var selectLanguageLevelDropdown = new SelectElement(ChooseCertificateYearDropdown);

            selectLanguageLevelDropdown?.SelectByValue(year);

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
                TestContext.Out.WriteLine("Update cancel button not found. Continuing test...");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

        }

        public void ClosePopUp()
        {
            try
            {
                // Ensure element exists before checking Displayed
                if (PopUpCloseButton != null && PopUpCloseButton.Displayed)
                {
                    WaitUntilElementIsClickable(PopUpCloseButton);
                    PopUpCloseButton.Click();
                }
            }
            catch (NoSuchElementException)
            {
                TestContext.Out.WriteLine("Pop-up close button not found. Continuing test...");
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }


        public void DeleteAddedCertificate()
        {
            Wait(3);

            RefreshRows();

            if (Rows == null || Rows.Count == 0)
            {
                TestContext.Out.WriteLine("⚠️ No certificates found. Nothing to delete.");
                return;
            }

            try
            {
                var lastRow = Rows.Last(); // Get the last entered certificate row
                var certificateNameField = lastRow.FindElement(By.XPath("./td[1]"));
                string lastCertificateName = certificateNameField.Text.Trim();

                var deleteButton = lastRow.FindElement(By.XPath("./td[4]//span[2]/i"));
                deleteButton?.Click();
                PopUpMessage = GetPopUpMessage();
                Wait(1);
                ClosePopUp();
                Wait(3);

                TestContext.Out.WriteLine($"✅ Successfully deleted last entered certificate: {lastCertificateName}");
            }
            catch (StaleElementReferenceException)
            {
                TestContext.Out.WriteLine("⚠️ Stale element detected. Refreshing and retrying...");
                RefreshRows();
            }
            catch (NoSuchElementException)
            {
                TestContext.Out.WriteLine("❌ Delete button not found for the last entered certificate.");
            }
        }

        public void ClearCertificationsData()
        {
            int retryCount = 0;
            int maxRetries = 3; // Limit retries for stale elements

            while (true)
            {
                try
                {
                    // Re-find all delete buttons each time

                    if (DeleteButtons == null || DeleteButtons.Count == 0)
                    {
                        TestContext.Out.WriteLine("✅ No more certifications to delete.");
                        break; // Exit when no delete buttons are left
                    }
                    else
                    {
                        TestContext.Out.WriteLine("✅ Deleting Certificates");
                        // Always target the last delete button
                        IWebElement deleteBtn = DeleteButtons.Last();

                        WaitUntilElementIsClickable(deleteBtn);
                        deleteBtn.Click();
                        Wait(1); // Give some time for the DOM to refresh

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
        public void DeleteCertificatesData(string certificateName)
        {
            Wait(3);

            int retryCount = 0;
            const int maxRetries = 4;
            bool isAnyCertificateDeleted = false; // Track if any deletion happened

            while (true)
            {
                try
                {
                    RefreshRows();

                    if (Rows == null || Rows.Count == 0)
                    {
                        break; // Stop checking if no rows are found
                    }

                    bool isCertificateDeletedInThisIteration = false;

                    foreach (var row in Rows.ToList())
                    {
                        try
                        {
                            var certificatenameField = row.FindElement(By.XPath("./td[1]"));
                            string certificateNameText = certificatenameField.Text.Trim();

                            if (certificateNameText.Equals(certificateName, StringComparison.OrdinalIgnoreCase))
                            {
                                var deleteButton = row.FindElement(By.XPath("./td[4]//span[2]/i"));
                                deleteButton?.Click();
                                PopUpMessage = GetPopUpMessage();
                                Wait(1);
                                ClosePopUp();
                                Wait(3);

                                isCertificateDeletedInThisIteration = true;
                                isAnyCertificateDeleted = true;

                                break; // Stop after deleting and refresh rows
                            }
                        }
                        catch (StaleElementReferenceException)
                        {
                            TestContext.Out.WriteLine("⚠️ Stale element detected inside foreach loop. Skipping to next iteration...");
                            continue;
                        }
                    }

                    // If no deletion happened in this pass,done checking
                    if (!isCertificateDeletedInThisIteration)
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
                    TestContext.Out.WriteLine($"⚠️ No delete button found for '{certificateName}'. Exiting.");
                    break;
                }
            }

            // **Log cleanup message only at the very end, not after each deletion**
            RefreshRows(); // One final refresh to check if all are deleted
            if (Rows == null || Rows.Count == 0)
            {
                if (isAnyCertificateDeleted)
                {
                    TestContext.Out.WriteLine("✅ Cleanup process completed successfully.");
                }
                else
                {
                    TestContext.Out.WriteLine("✅ No certificates present, cleanup not needed.");
                }
            }
        }
        private void RefreshRows()
        {
            try
            {
                var rowCount = Rows?.Count;
                // Refresh the rows by finding them again after the deletion
                TestContext.Out.WriteLine($"Refreshed rows count: {rowCount}");
            }
            catch (Exception e)
            {
                // Handle potential errors while refreshing the rows
                TestContext.Out.WriteLine($"Error refreshing rows: {e.Message}");
            }
        }
    }
}
