using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Ellegaard_VisualWebTestingTool
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

        public void RunTest(IWebDriver driver, string testName)
        {
            if (!Directory.Exists(imageSavePath+testName))
            {
                PrintOutResults.Instance().logs.Add("" + testName + " didnt exist, a test case have bin created for future test cases in section "+testSectionName);
                CreateTest(testName,driver);
            }
            var imageBytesSavedImages = LoadTestImages(testName);
            LoadPage(driver);
            byte[] screenshotBytes = CaptureDriverWebImage(driver,testName);
            CompareImages(screenshotBytes, imageBytesSavedImages, testName);
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
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(imageSavePath + testName + "\\WebImage\\CapturedWebImage.bmp", settings.ImagesFormat);
            byte[] screenshot = File.ReadAllBytes(imageSavePath + testName + "\\WebImage\\CapturedWebImage.bmp");
            //Billed dimensonierne ganget og ganget med dybden trukket fra antal total bit filen fylder.
            //Dette er til testing
            //int test = 33;
            //Thread.Sleep(1);
            //for (int i = 0; i < 20; i++)
            //{
            //    var a = (byte[])screenshot.Clone();
            //    for (int y = test; y < screenshot.Length; y++)
            //    {
            //        a[y] = 0;
            //    }
            //    for (int x = test; x < screenshot.Length; x += 4)
            //    {
            //        a[x] = 255;
            //    }
            //    File.WriteAllBytes(imageSavePath + testName + "\\WebImage\\FullScreenTestImage" + test + ".bmp", a);
            //    test++;
            //    Thread.Sleep(1);
            //}
            
            //Dette er til testing

            return screenshot;
        }

        #endregion
        
        #region ImageComparison

        void CompareImages(byte[] siteImage, Dictionary<string, byte[]> localImages, string testName)
        {
            ImageResults results = new ImageResults();
            results.testResults = new Dictionary<string, float>();
            results.testName = testName;
            results.testSectionName = testSectionName;
            try
            {
                foreach (var imageBytes in localImages)
                {
                    float totalCounter = ((siteImage.Length + imageBytes.Value.Length) / 2) - 35;
                    if (siteImage.Length != imageBytes.Value.Length) Console.WriteLine("Warning the image taken from the site, and the local images are a different resolution, this can a negative impact on the results");
                    var equals = 0;
                    for (int i = 34; i < totalCounter; i++)
                    {
                        if (siteImage[i].Equals(imageBytes.Value[i])) { equals++; }
                    }
                    float procent = (equals / totalCounter) * 100;
                    results.testResults.Add(imageBytes.Key, procent);
                }
            }
            catch (Exception)
            {
                throw new Exception("The dimensions are different on the local image file, from the and the image taken");
            }
            PrintOutResults.Instance().testResults.Add(results);
        }

        #endregion

        #region General Methods
        static void LoadPage(IWebDriver driver)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(a => ((IJavaScriptExecutor)a).ExecuteScript("return document.readyState").Equals("complete"));
        }
        #endregion
    }
}
