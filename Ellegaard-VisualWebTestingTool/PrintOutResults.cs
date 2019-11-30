using System;
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
            if (settings.CreateImageShowingDifferencePixelPoints) CreateShowDifferenceByteImage(settings);

            
            var xmlDocument = CreateXmlElements();

            #region SaveXmlPath
            savepath = settings.ResultXmlSavePath + "\\XmlTestResult";
            if (!Directory.Exists(savepath)){ Directory.CreateDirectory(savepath); }
            savepath += "\\TestResultXmlDocument.xml";
            xmlDocument.Save(savepath);
            
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

        public void SendResultsAsEmail(SmtpClient client, string[] mailTo, string mailFrom,[Optional]string subject,[Optional]Settings settings)
        {
            List<Attachment> mailAttachment = new List<Attachment>();             
            if (settings == null) settings = new Settings();
            TestResultsSort(settings);
            if (settings.CreateImageShowingDifferencePixelPoints) CreateShowDifferenceByteImage(settings);
            if (settings.IncludeXmlFileInMail) { PrintToXML(settings); mailAttachment.Add(new Attachment(savepath)); }
            if (Directory.Exists(settings.ShowImageDifferencePointsImageDir + "\\TestDifferenceImages") && settings.InsertImageInMail && settings.CreateImageShowingDifferencePixelPoints)
            {
                var images = Directory.GetFiles(settings.ShowImageDifferencePointsImageDir + "\\TestDifferenceImages\\");
                foreach (var item in images)
                {
                    mailAttachment.Add(new Attachment(item));
                }
            }
            string mailBody = CreateMailBody();

            foreach (var mail in mailTo)
            {
                MailAddress addressFrom = new MailAddress(mailFrom);
                MailAddress addressTo = new MailAddress(mail);
                MailMessage message = new MailMessage(addressFrom, addressTo);
                mailAttachment.ForEach(a=>message.Attachments.Add(a));
                message.Subject = subject??settings.MailSubject;                
                message.Body = mailBody;
                message.IsBodyHtml = true;
                client.Send(message);
            }
        }
        
        void TestResultsSort(Settings settings)
        {
            #region Remove images outside a specified procent range
            List<ImageResults> myTest = new List<ImageResults>();
            
            
            for (int i = 0; i < testResults.Count; i++)
            {
                var shortWhileList = new List<string>();
                var myTestResults = testResults[i].testResults;
                if (settings.OnlyShowImagesBelowTheSetProcentValue == true && settings.OnlyShowImagesHigherThenTheSetProcentValue == false)
                {
                    foreach (var result in myTestResults)
                    {
                        if (result.Value >= settings.ImagesProcentDifference)
                        {
                            shortWhileList.Add(result.Key);
                        }
                    }
                    foreach (var item in shortWhileList)
                    {
                        myTestResults.Remove(item);
                    }
                }
                else if (settings.OnlyShowImagesBelowTheSetProcentValue == false && settings.OnlyShowImagesHigherThenTheSetProcentValue == true)
                {
                    foreach (var result in myTestResults)
                    {
                        if (result.Value <= settings.ImagesProcentDifference)
                        {
                            shortWhileList.Add(result.Key);
                        }
                    }
                    foreach (var item in shortWhileList)
                    {
                        myTestResults.Remove(item);
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
            var myDocument = new XDocument(new XDocumentType("html", null, null, null));
            var htmlTag = new XElement("html");
            myDocument.Add(htmlTag);
                var header = new XElement("head");
                var body = new XElement("body");
                htmlTag.Add(header);
                htmlTag.Add(body);
            
            //Body HTML
            foreach (var result in testResults)
            {
                if (result.testResults.Count==0){continue;}
                var testSectionName = result.testSectionName;
                var testName = result.testName;
                var htmlTable = new XElement("table", new XAttribute("style", "width:100%;"));
                body.Add(htmlTable);
                var headerWidth = new XAttribute("style", "width:20%");
                var htmlTableRowHeadline = new XElement("tr",
                    new XElement("th", "SectionName", new XAttribute("style", "width:25%")),
                    new XElement("th", "TestName", new XAttribute("style", "width:25%")),
                    new XElement("th", "ImageName", new XAttribute("style", "width:25%")),
                    new XElement("th", "ProcentImageDifference", new XAttribute("style", "width:25%")));
                htmlTable.Add(htmlTableRowHeadline);

                foreach (var image in result.testResults)
                {
                    XElement htmlTableRowData = new XElement("tr",
                            new XElement("td", testSectionName),
                            new XElement("td", testName),
                            new XElement("td", image.Key),
                            new XElement("td", image.Value));
                    
                    htmlTable.Add(htmlTableRowData);
                    testSectionName = "";
                    testName = "";
                }
            }
            return myDocument.ToString();
        }

        void CreateShowDifferenceByteImage(Settings settings)
        {
            if (Directory.Exists(settings.ShowImageDifferencePointsImageDir + "\\TestDifferenceImages")) Directory.Delete(settings.ShowImageDifferencePointsImageDir + "\\TestDifferenceImages", true);
            for (int i = 0; i < testResults.Count; i++)
            {
                if (testResults[i].testResults.Count == 0) continue;
                string lowestImageProcentDifferenceName = "";
                float lowestImageProcentDifference=0;

                foreach (var item in testResults[i].testResults)
                {
                    if (lowestImageProcentDifference < item.Value)
                    {
                        lowestImageProcentDifferenceName = item.Key;
                        lowestImageProcentDifference = item.Value;
                    }
                }
                var localFile = File.ReadAllBytes(settings.TestDataSavePath+"\\VisualImageTestData\\" + testResults[i].testSectionName + "\\"+ testResults[i].testName+"\\"+ lowestImageProcentDifferenceName);
                var webScreenFile = File.ReadAllBytes(settings.TestDataSavePath + "\\VisualImageTestData\\" + testResults[i].testSectionName + "\\" + testResults[i].testName + "\\WebImage\\CapturedWebImage.bmp");

                var redColors = new List<int>();
                for (int x = 36; x < webScreenFile.Length; x+=4) {  redColors.Add(x);  }

                for (int y = 34; y < webScreenFile.Length; y++)
                {
                    if(webScreenFile[y] != localFile[y] && redColors.Contains(y))
                    {
                        webScreenFile[y-2] = 0; /*Blue*/
                        webScreenFile[y-1] = 0; /*Green*/
                        webScreenFile[y] = 255; /*Red*/
                        webScreenFile[y+1] = 0; /*Transparent*/
                    }                    
                }
                if (!Directory.Exists(settings.ShowImageDifferencePointsImageDir + "\\TestDifferenceImages")) Directory.CreateDirectory(settings.ShowImageDifferencePointsImageDir + "\\TestDifferenceImages");
                File.WriteAllBytes(settings.ShowImageDifferencePointsImageDir + "\\TestDifferenceImages\\"+testResults[i].testName+".bmp", webScreenFile);
            }
        }
    }
    struct ImageResults
    {
        public string testSectionName { get; set; }
        public string testName { get; set; }
        public Dictionary<string, float> testResults;
    }
}
