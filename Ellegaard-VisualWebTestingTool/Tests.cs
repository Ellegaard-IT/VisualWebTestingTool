using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ellegaard_VisualWebTestingTool
{
    public class Tests
    {
        public static void RunTest(IWebDriver driver)
        {
            LoadPage(driver);
            TakeScreenShot(driver);
        }

        private void CreateTest(IWebDriver driver)
        {

        }

        static void TakeScreenShot(IWebDriver driver)
        {
            Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile(driver.Url,ScreenshotImageFormat.Png);
        }

        static void LoadPage(IWebDriver driver)
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(a => ((IJavaScriptExecutor)a).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
