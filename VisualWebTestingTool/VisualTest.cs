using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace VisualWebTestingTool
{
    public class VisualTest
    {
        string imageSavePath;
        string testSectionName;
        Settings settings;

        public VisualTest(string testSectionName, [Optional]Settings settings)
        {
            this.testSectionName = testSectionName;
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
            this.imageSavePath = testSection.ToString();
        }

        public void RunTest(IWebDriver driver, string testName, [Optional] Settings settings)
        {
            driver.Manage().Window.FullScreen();
            if (settings == null) settings = new Settings();
            if (!Directory.Exists(imageSavePath+testName))
            {
                PrintOutResults.Instance().logs.Add("" + testName + " didnt exist, a test case have bin created for future test cases in section "+testSectionName);
                CreateTest(testName,driver);
            }
            var imageBytesSavedImages = LoadTestImages(testName);
            LoadPage(driver);
            byte[] screenshotBytes = CaptureDriverWebImage(driver,testName);
            CompareImages(screenshotBytes, imageBytesSavedImages, testName, settings);
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
            string saveUrl = imageSavePath + testName;
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
                screenshot.SaveAsFile(saveUrl + "\\" + testName + counter+".bmp", settings.ImagesFormat);
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
            if (!Directory.Exists(imageSavePath + testName))
            {
                throw new Exception("Images cant be loaded, a directory by the name " + testName + " doesn't exist");
            }
            string saveUrl = imageSavePath + testName;
            Dictionary<string,byte[]> myImages = new Dictionary<string, byte[]>();

            var counter = 0;
            foreach (var image in Directory.GetFiles(saveUrl))
            {
                counter++;
                myImages.Add(testName+counter,File.ReadAllBytes(image));
            }

            return myImages;
        }

        #endregion

        #region CaptureWebImage

        public byte[] CaptureDriverWebImage(IWebDriver driver, string testName)
        {
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(imageSavePath + testName + "\\WebImage\\CapturedWebImage.png", settings.ImagesFormat);
            byte[] screenshot = File.ReadAllBytes(imageSavePath + testName + "\\WebImage\\CapturedWebImage.png");
            return screenshot;
        }

        #endregion
        
        #region ImageComparison

        void CompareImages(byte[] siteImage, Dictionary<string, byte[]> localImages, string testName, Settings settings)
        {
            ImageResults results = new ImageResults();
            List<int> redNumbers = new List<int>();
            results.testResults = new Dictionary<string, float>();
            results.testName = testName;
            results.testSectionName = testSectionName;
            float totalCounter = 0;
            float toManyPixelsCounter = 0;

            for (int i = 36; i < siteImage.Length; i+=4)
            {
                redNumbers.Add(i);
            }
            foreach (var imageBytes in localImages)
            {
                if (siteImage.Length > imageBytes.Value.Length) { totalCounter = imageBytes.Value.Length; toManyPixelsCounter = siteImage.Length - imageBytes.Value.Length; Warning(results); }
                else if (siteImage.Length < imageBytes.Value.Length) { totalCounter = siteImage.Length; toManyPixelsCounter = imageBytes.Value.Length - siteImage.Length; Warning(results); }
                else { totalCounter = ((siteImage.Length + imageBytes.Value.Length) / 2); }
                var equals = 0;
                for (int i = 34; i < totalCounter; i++)
                {
                    if (siteImage[i].Equals(imageBytes.Value[i])) { equals++; }
                }
                float procent = (equals / (totalCounter - toManyPixelsCounter)) * 100;
                results.testResults.Add(imageBytes.Key, procent);
            }

            KeyValuePair<string, float> highestProcentImage = new KeyValuePair<string, float>("", 0);
            foreach (var item in results.testResults)
            {
                if (item.Value > highestProcentImage.Value) highestProcentImage = new KeyValuePair<string, float>(item.Key, item.Value);
            }
            try
            {
                if (siteImage.Length > localImages[highestProcentImage.Key].Length) { totalCounter = localImages[highestProcentImage.Key].Length; }
            }
            catch (Exception){}
            try
            {
                if (siteImage.Length < localImages[highestProcentImage.Key].Length) { totalCounter = siteImage.Length; }
            }
            catch (Exception){}
            if(totalCounter == 0)totalCounter = ((siteImage.Length + localImages[highestProcentImage.Key].Length) / 2);


            byte[] highestProcentImageBytes = localImages[highestProcentImage.Key];
            for (int i = 34; i < totalCounter; i++)
            {
                if (siteImage[i] != highestProcentImageBytes[i] && redNumbers.Contains(i))
                {
                    siteImage[i - 2] = 0;//blue
                    siteImage[i - 1] = 0;//green
                    siteImage[i] = 255;//red
                    siteImage[i + 1] = 0;//transparent
                }
            }
            results.showPictureDifferencesInBytes = siteImage;

            PrintOutResults.Instance().testResults.Add(results);
        }

        #endregion

        #region General Methods
        static void LoadPage(IWebDriver driver)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(a => ((IJavaScriptExecutor)a).ExecuteScript("return document.readyState").Equals("complete"));
        }

        static void Warning(ImageResults result)
        {
            Console.WriteLine("Warning: Test pixels are not balanced and results may be effekted on section: {0}, testname: {1}", result.testSectionName, result.testName);
        }
        #endregion
    }
}
