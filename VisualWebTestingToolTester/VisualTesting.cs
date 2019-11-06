using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ellegaard_VisualWebTestingTool;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace VisualWebTestingToolTester
{
    [TestClass]
    public class VisualTesting
    {
        IWebDriver driver;

        [TestInitialize]
        public void StartUp()
        {
            driver = new ChromeDriver();
        }

        [TestMethod]
        public void TestMethod1()
        {
            driver.Navigate().GoToUrl("https://wpsites.net/wordpress-tips/how-slow-page-loading-times-decrease-page-views/");
            Tests.RunTest(driver);
        }

        [TestCleanup]
        public void ShutDown()
        {
            driver.Close();
        }
    }
}
