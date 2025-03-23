using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mars_Competition_Nunit.Data_Model;
using Newtonsoft.Json.Linq;

namespace Mars_Competition_Nunit.Utilities
{
    class JsonDataReader
    {
        private static string GetJsonFilePath(string fileName)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo? parent = Directory.GetParent(currentDirectory)?.Parent?.Parent;

            if (parent == null)
            {
                throw new Exception("🚨 Unable to locate the project root directory.");
            }

            // Returns the path for any JSON file in the TestData folder
            return Path.Combine(parent.FullName, "TestData", fileName);
        }

        private static List<T> ReadJsonData<T>(string fileName, string key)
        {
            string filePath = GetJsonFilePath(fileName);

            if (!File.Exists(filePath))
            {
                throw new Exception($"🚨 JSON file not found: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new Exception($"🚨 JSON file is empty: {filePath}");
            }

            JObject jsonData = JObject.Parse(jsonContent);
            if (!jsonData.TryGetValue(key, out JToken? dataToken) || dataToken == null)
            {
                throw new Exception($"🚨 Key '{key}' not found in {fileName}. Available keys: {string.Join(", ", jsonData.Properties().Select(p => p.Name))}");
            }
            return dataToken.ToObject<List<T>>() ?? throw new Exception($"🚨 Failed to parse '{key}' from {fileName}");
        }
        public static List<LoginModel> GetLoginData()
        {
            return ReadJsonData<LoginModel>("loginData.json", "logins");
        }
        public static List<CertificateModel> GetCertificateData(string fileName)
        {

            string filePath = GetJsonFilePath(fileName);

            if (!File.Exists(filePath))
            {
                throw new Exception($"🚨 Education JSON file not found: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);
            return ReadJsonData<CertificateModel>(fileName, "certificate");
        }
        public static List<EducationModel> GetEducationData(string fileName)
        {
            string filePath = GetJsonFilePath(fileName);

            if (!File.Exists(filePath))
            {
                throw new Exception($"🚨 Education JSON file not found: {filePath}");
            }

            string jsonContent = File.ReadAllText(filePath);
            TestContext.Out.WriteLine($"🔍 Reading Education JSON file at: {filePath}");
            TestContext.Out.WriteLine($"📜 JSON Content: {jsonContent}");
            return ReadJsonData<EducationModel>(fileName, "education");
        }
    }
}
