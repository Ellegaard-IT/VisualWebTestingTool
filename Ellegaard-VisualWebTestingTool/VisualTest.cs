using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Ellegaard_VisualWebTestingTool
{
    public class VisualTest: Settings
    {
        public static void CreateTest(IWebDriver driver, VisualTestPreprocess preprocess)
        {
            if (preprocess == null) { throw new Exception("TestPreprocess cant be null. Create a testpreprocess object and give it as input"); }
            LoadPage(driver);
            var images = TakeScreenShots(driver);
            preprocess.CreateTestDirectory("TestImage", images);
        }

        static List<Screenshot> TakeScreenShots(IWebDriver driver)
        {
            var imageTimer = new TimeSpan(0,0,AmountOfSecTheImagesAreTakenOver).TotalMilliseconds/AmountOfImagesForEachTest;
            List<Screenshot> images = new List<Screenshot>();
            
            for (int i = 0; i < AmountOfImagesForEachTest; i++)
            {
                Thread.Sleep((int)imageTimer);
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                images.Add(screenshot);
            }
            return images;
        }

        static void LoadPage(IWebDriver driver)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(a => ((IJavaScriptExecutor)a).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
