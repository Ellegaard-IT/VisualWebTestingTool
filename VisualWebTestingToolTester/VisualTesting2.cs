using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisualWebTestingTool;
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
        Settings settings;

        [TestInitialize]
        public void StartUp()
        {
            driver = new FirefoxDriver(Environment.CurrentDirectory);
            settings = new Settings();
        }

        [TestMethod]
        public void TestMethod1()
        {
            //Set settings
            settings.IncludeXmlFileInMail = true;
            settings.IncludeDifferenceImageInMail = true;

            driver.Navigate().GoToUrl("https://wpsites.net/wordpress-tips/how-slow-page-loading-times-decrease-page-views/");
            VisualTest test = new VisualTest("TestSectionInClass2");
            test.RunTest(driver, "myTestImagesInClass2");
            VisualTest test2 = new VisualTest("Test2InClass2");
            test2.RunTest(driver, "hisTestImagesInClass2");
            test.RunTest(driver, "theirTestImagesInClass2");
            var RecieverEmails = new string[] { "Morten_hansen51@yahoo.dk" };


            PrintOutResults.Instance().PrintToXML(settings);

            //PrintOutResults.Instance().SendResultsAsEmail(MailTest(),RecieverEmails,"SenderEmail", null, settings) ;
        }

        [TestCleanup]
        public void ShutDown()
        {
            driver.Close();
        }



        public SmtpClient MailTest()
        {
            var credentials = new NetworkCredential();
            credentials.UserName = "SMTPserverUser";
            credentials.Password = "SMPTPserverPassword";

            SmtpClient smtp = new SmtpClient("ServerAddress", 25);
            smtp.Credentials = credentials;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            return smtp;
        }
    }
}
