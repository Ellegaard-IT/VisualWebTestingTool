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
    public class VisualTesting2
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
            VisualTest test = new VisualTest("TestSectionInClass2");
            test.RunTest(driver, "myTestImagesInClass2");
            VisualTest test2 = new VisualTest("Test2InClass2");
            test2.RunTest(driver, "hisTestImagesInClass2");
            test.RunTest(driver, "theirTestImagesInClass2");

            PrintOutResults.Instance().PrintToXML();
        }

        [TestCleanup]
        public void ShutDown()
        {
            driver.Close();
        }
    }
}
