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
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(testSectionName+testName+"testImage",settings.ImagesFormat);
            byte[] screenshot = File.ReadAllBytes(testSectionName + testName + "testImage");
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

        public List<byte[]> LoadTestImages(string testName)
        {
            if (!Directory.Exists(testSectionName + testName))
            {
                throw new Exception("Images cant be loaded, a directory by the name " + testName + " doesn't exist");
            }
            string saveUrl = testSectionName + testName;
            List<byte[]> myImages = new List<byte[]>();

            foreach (var image in Directory.GetFiles(saveUrl))
            {
                myImages.Add(File.ReadAllBytes(image));
            }

            return myImages;
        }

        #endregion

        #region General Methods
        static void LoadPage(IWebDriver driver)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(a => ((IJavaScriptExecutor)a).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void ImageConvert()
        {

        }
        #endregion

        #region ImageComparison

        public void CompareImages(byte[] siteImage, List<byte[]> localImages)
        {
            var totalCounter = (siteImage.Length + localImages[0].Length)/2;
            var counter = 0;
            var equal = 0;
            foreach (var imageBytes in localImages)
            {
                foreach (var imageByte in imageBytes)
                {
                    if (imageByte.Equals(siteImage[counter]))
                    {
                        equal++;
                    }
                    counter++;
                }
                var procent = equal / totalCounter * 100;
            }
        }

        #endregion
    }
}
