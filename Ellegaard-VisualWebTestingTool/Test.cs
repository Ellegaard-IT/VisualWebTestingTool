using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ellegaard_VisualWebTestingTool
{
    public class Test: Settings
    {
        public static void CreateTest(IWebDriver driver, TestSectionPreprocess preprocess)
        {
            if (preprocess == null) { throw new Exception("TestPreprocess cant be null. Create a testpreprocess object and give it as input"); }
            LoadPage(driver);
            var images = TakeScreenShots(driver);
            preprocess.CreateTestDirectory("TestImage", images);
        }

        static List<Screenshot> TakeScreenShots(IWebDriver driver)
        {
            //var ts = TimeSpan.FromSeconds(AmountOfSecTheImagesAreTakenOver).;
            //var drop = AmountOfSecTheImagesAreTakenOver / AmountOfImagesForEachTest;
            //while (ts.)
            //{

            //}

            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            var images = new List<Screenshot>() { screenshot };
            return images;
        }

        static void LoadPage(IWebDriver driver)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(a => ((IJavaScriptExecutor)a).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
