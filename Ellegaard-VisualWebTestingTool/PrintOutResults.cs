﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;

namespace Ellegaard_VisualWebTestingTool
{
    public class PrintOutResults
    {
        #region Singleton
        static PrintOutResults instance { get; set; }
        private static object threadLock = new object();
        public static PrintOutResults Instance()
        {
            lock (threadLock)
            {
                if (instance == null) { instance = new PrintOutResults(); return instance; }
                else { return instance; }
            }
        }
        #endregion

        internal List<string> logs = new List<string>();
        internal List<ImageResults> testResults = new List<ImageResults>();
        private List<ImageResults> myTest = new List<ImageResults>();
        string savepath;

        public int GetResultCount()
        {
            return testResults.Count;
        }
        public void ClearResults()
        {
            testResults.Clear();
        }

        public void PrintToXML([Optional]Settings settings)
        {
            if (settings == null) { settings = new Settings(); }

            TestResultsSort(settings);
            var xmlDocument = CreateXmlElements();

            #region SaveXmlPath
            savepath = settings.ResultXmlSavePath + "\\XmlTestResult";
            if (!Directory.Exists(savepath)){ Directory.CreateDirectory(savepath); }
            xmlDocument.Save(savepath+"\\TestResultXmlDocument.xml");
            #endregion
        }
        private XmlDocument CreateXmlElements()
        {
            XmlDocument doc = new XmlDocument();

            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement TestResultsHeadline = doc.CreateElement(string.Empty, "VisualTestResults", string.Empty);
            doc.AppendChild(TestResultsHeadline);
            XmlElement sectionElement = doc.CreateElement(string.Empty, testResults[0].testSectionName, string.Empty);
            string lastSectionName = testResults[0].testSectionName;
            foreach (var testResult in testResults)
            {
                if (testResult.testSectionName != lastSectionName)
                {
                    sectionElement = doc.CreateElement(string.Empty, testResult.testSectionName, string.Empty);
                }
                TestResultsHeadline.AppendChild(sectionElement);
                XmlElement testCase = doc.CreateElement(string.Empty, testResult.testName, string.Empty);
                sectionElement.AppendChild(testCase);
                
                foreach (var item in testResult.testResults)
                {
                    XmlElement attribute = doc.CreateElement(string.Empty, item.Key, string.Empty);
                    XmlText xmlText = doc.CreateTextNode("procent difference:" + item.Value.ToString());
                    attribute.AppendChild(xmlText);
                    testCase.AppendChild(attribute);
                }
            }

            return doc;
        }

        public void SendResultsAsEmail(SmtpClient client, string[] mailTo, string mailFrom,[Optional]Settings settings)
        {
            Attachment mailAttachment=null;
            if (settings == null) settings = new Settings();
            if (File.Exists(savepath) && settings.IncludeXmlFileInMail) { PrintToXML(settings); mailAttachment = new Attachment(savepath); }
            else TestResultsSort(settings);
            string mailBody = "";
            CreateMailBody();




            foreach (var mail in mailTo)
            {
                MailAddress addressFrom = new MailAddress(mailFrom);
                MailAddress addressTo = new MailAddress(mail);
                MailMessage message = new MailMessage(addressFrom, addressTo);
                if (mailAttachment != null && settings.IncludeXmlFileInMail) message.Attachments.Add(mailAttachment);
                message.Subject = settings.MailSubject;                
                message.Body = mailBody;
                client.Send(message);
            }
        }

        void TestResultsSort(Settings settings)
        {
            #region Remove images outside a specified procent range
            foreach (var result in testResults)
            {
                if (settings.OnlyShowImagesBelowTheSetProcentValue==true && settings.OnlyShowImagesHigherThenTheSetProcentValue==false)
                {
                    foreach (var item in result.testResults)
                    {
                        if (item.Value < settings.ImagesProcentDifference)
                        {
                            result.testResults.Remove(item.Key);
                        }
                    }
                }
                else if (settings.OnlyShowImagesBelowTheSetProcentValue == false && settings.OnlyShowImagesHigherThenTheSetProcentValue == true)
                {
                    foreach (var item in result.testResults)
                    {
                        if (item.Value > settings.ImagesProcentDifference)
                        {
                            result.testResults.Remove(item.Key);
                        }
                    }
                }
            }
            #endregion

            testResults.ForEach(a => myTest.Add(a));
            testResults.Clear();
            while (myTest.Count != 0)
            {
                foreach (var item in myTest.FindAll(a => a.testSectionName == myTest[0].testSectionName))
                {
                    testResults.Add(item);
                    myTest.Remove(item);
                }
            }
        }

        string CreateMailBody()
        {
            var xDocument = new XDocument(
                new XDocumentType("html",null,null,null),
                new XElement("html",
                    new XElement("head"),
                    new XElement("body",
                        new XElement("p",
                            "This is a paragraf"),
                        new XElement("p",
                            "this is another paragraf"
                            )
                        )
                    )
                );

            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, IndentChars = "\t" };
            var a = xDocument.ToString();


            return null;
        }
    }
    struct ImageResults
    {
        public string testSectionName { get; set; }
        public string testName { get; set; }
        public Dictionary<string, float> testResults;
        public byte[] showPictureDifferencesInBytes;
    }
}
