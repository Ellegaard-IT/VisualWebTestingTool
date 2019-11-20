using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Ellegaard_VisualWebTestingTool
{
    public class VisualTest : PrintOutResults
    {
        string testSectionName;
        Settings settings;
        List<string> logs = new List<string>();

        public VisualTest(string testSectionName, [Optional]Settings settings)
        {
            this.settings = settings ?? new Settings();
            var testSection = new StringBuilder(this.settings.TestDataSavePath);
            
            testSection.Append("\\VisualImageTestData");
            if (!Directory.Exists(testSection.ToString()))
            {
                Directory.CreateDirectory(testSection.ToString());
            }

            testSection.Append("\\" + testSectionName);
            if (!Directory.Exists(testSection.ToString()))
            {
                Directory.CreateDirectory(testSection.ToString());
            }

            testSection.Append("\\");
            this.testSectionName = testSection.ToString();
        }

        public void RunTest(IWebDriver driver, string testName)
        {
            if (!Directory.Exists(testSectionName+testName))
            {
                CreateTest(testName,driver);
            }
            var imageBytesSavedImages = LoadTestImages(testName);
            LoadPage(driver);
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(testSectionName+testName+ "\\WebImage\\CapturedWebImage", settings.ImagesFormat);
            byte[] screenshot = File.ReadAllBytes(testSectionName + testName + "\\WebImage\\CapturedWebImage");
            CompareImages(screenshot, imageBytesSavedImages);
        }

        

        #region CreateTests
        void CreateTest(string testName, IWebDriver driver)
        {
            LoadPage(driver);
            var images = TakeScreenShots(driver);
            CreateTestDirectory(testName, images);
        }

        void CreateTestDirectory(string testName, List<Screenshot> screenshotList)
        {
            string saveUrl = testSectionName + testName;
            int counter = 1;
            if (Directory.Exists(saveUrl))
            {
#if !DEBUG
                throw new Exception("An test named " + testName + " already exist, give it another name or delete the old project, couldnt create the test");
#endif
            }

            Directory.CreateDirectory(saveUrl);
            Directory.CreateDirectory(saveUrl+"\\WebImage");
            foreach (var screenshot in screenshotList)
            {
                screenshot.SaveAsFile(saveUrl + "\\" + testName + counter, settings.ImagesFormat);
                counter++;
            }
        }

        List<Screenshot> TakeScreenShots(IWebDriver driver)
        {
            var imageTimer = new TimeSpan(0, 0, settings.AmountOfSecTheImagesAreTakenOver).TotalMilliseconds / settings.AmountOfImagesForEachTest;
            List<Screenshot> images = new List<Screenshot>();
            LoadPage(driver);

            for (int i = 0; i < settings.AmountOfImagesForEachTest; i++)
            {
                Thread.Sleep((int)imageTimer);
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                images.Add(screenshot);
            }
            return images;
        }
        #endregion

        #region LoadTestsIn

        public Dictionary<string, byte[]> LoadTestImages(string testName)
        {
            if (!Directory.Exists(testSectionName + testName))
            {
                throw new Exception("Images cant be loaded, a directory by the name " + testName + " doesn't exist");
            }
            string saveUrl = testSectionName + testName;
            Dictionary<string,byte[]> myImages = new Dictionary<string, byte[]>();

            foreach (var image in Directory.GetFiles(saveUrl))
            {
                myImages.Add(image,File.ReadAllBytes(image));
            }

            return myImages;
        }

        #endregion

        #region General Methods
        static void LoadPage(IWebDriver driver)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(a => ((IJavaScriptExecutor)a).ExecuteScript("return document.readyState").Equals("complete"));
        }
        #endregion

        #region ImageComparison

        public void CompareImages(byte[] siteImage, Dictionary<string, byte[]> localImages)
        {
            ImageResults results = new ImageResults();
            var counter = 0;
            var equals = 0;

            foreach (var imageBytes in localImages)
            {
                var totalCounter = (siteImage.Length + imageBytes.Value.Length) / 2;
                foreach (var imageByte in imageBytes.Value)
                {
                    if (siteImage[counter].Equals(imageByte)){  equals++; }
                    counter++;
                }
                counter = 0;
                var procent = (equals / totalCounter) * 100;
                //Fix denne bug
            }
        }

        struct ImageResults
        {
            public string TestName { get; set; }
            //Dictionary<string, float> results = new Dictionary<string, float>();
        }

        #endregion
    }
}
