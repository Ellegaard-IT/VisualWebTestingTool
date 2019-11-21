using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ellegaard_VisualWebTestingTool;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;

namespace VisualWebTestingToolTester
{
    [TestClass]
    public class VisualTesting
    {
        IWebDriver driver;

        [TestInitialize]
        public void StartUp()
        {
            driver = new FirefoxDriver(Environment.CurrentDirectory);
        }

        [TestMethod]
        public void TestMethod1()
        {
            driver.Navigate().GoToUrl("https://wpsites.net/wordpress-tips/how-slow-page-loading-times-decrease-page-views/");
            VisualTest test = new VisualTest("TestSection");
            test.RunTest(driver,"TestImages");
            test.RunTest(driver, "TestImages1");
            VisualTest test2 = new VisualTest("TestSection2");
            test2.RunTest(driver, "TestImages2");
        }

        [TestCleanup]
        public void ShutDown()
        {
            driver.Close();
        }
    }
}
