using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ellegaard_VisualWebTestingTool;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;
using System.Net;
using System.Net.Mail;

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
            Settings settings = new Settings();
            settings.IncludeXmlFileInMail = true;
            var a = new string[] { "Morten_hansen51@yahoo.dk" };
            PrintOutResults.Instance().SendResultsAsEmail(MailTest(),a,"VisualTestingTool@ellegaard-it.com", "Er stort set færdig med biblioteket, måtte skrive mail html koden i linq, vil gerne have billeder i mailen som viser hvor på billederne forskellen er men har ikke lavet metoden for det endnu. Men indtil videre ser det sådan her ud", settings) ;
        }

        [TestCleanup]
        public void ShutDown()
        {
            driver.Close();
        }

        public SmtpClient MailTest()
        {
            var credentials = new NetworkCredential();
            credentials.UserName = "donotrply@ellegaard-it.com";
            credentials.Password = "T6iM2KCX9RaKYwv";

            SmtpClient smtp = new SmtpClient("asmtp.unoeuro.com", 25);
            smtp.Credentials = credentials;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            return smtp;
        }
    }
}
