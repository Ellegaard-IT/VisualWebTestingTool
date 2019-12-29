﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ellegaard_VisualWebTestingTool;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Threading;
using System.Net.Mail;
using System.Net;

namespace VisualWebTestingToolTester
{
    [TestClass]
    public class VisualTesting
    {
        IWebDriver driver;
        VisualTest test;

        [TestInitialize]
        public void StartUp()
        {
            driver = new FirefoxDriver(Environment.CurrentDirectory);
            test = new VisualTest("TestSection");
        }

        [TestMethod]
        public void TestMethod1()
        {
            driver.Navigate().GoToUrl("https://wpsites.net/wordpress-tips/how-slow-page-loading-times-decrease-page-views/");
            
            test.RunTest(driver,"myTestImages");

            VisualTest test2 = new VisualTest("Test2");
            test2.RunTest(driver, "hisTestImages");
            test.RunTest(driver, "theirTestImages");
        }

        [TestCleanup]
        public void ShutDown()
        {
            driver.Close();
        }




        public void MailTest()
        {
            var credentials = new NetworkCredential();
            credentials.UserName = "donotrply@ellegaard-it.com";
            credentials.Password = "T6iM2KCX9RaKYwv";

            SmtpClient smtp = new SmtpClient("asmtp.unoeuro.com", 25);
            smtp.Credentials = credentials;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        }
    }
}
