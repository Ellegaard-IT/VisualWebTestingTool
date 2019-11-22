using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

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

            XmlDocument doc = new XmlDocument();

            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement TestResultsHeadline = doc.CreateElement(string.Empty, "TestResults", string.Empty);
            doc.AppendChild(TestResultsHeadline);
            XmlElement sectionElement;

            bool resultSectionIsInList = false;
            foreach (var testResult in testResults)
            {
                for (int i = 0; i < TestResultsHeadline.ChildNodes.Count; i++)
                {
                    if (TestResultsHeadline.ChildNodes.Item(i).Name == testResult.testSectionName)
                    {
                        resultSectionIsInList = true;
                        sectionElement = TestResultsHeadline.ChildNodes.Item(i);
                        break;
                    }
                }
                

                if (resultSectionIsInList == false)
                {
                    sectionElement = doc.CreateElement(string.Empty, testResult.testSectionName, string.Empty);
                }
                XmlElement testCase = doc.CreateElement(string.Empty, testResult.testName, string.Empty);
                sectionElement.AppendChild(testCase);

                foreach (var item in testResult.testResults)
                {
                    XmlElement attribute = doc.CreateElement(string.Empty, item.Key, string.Empty);
                    XmlText xmlText = doc.CreateTextNode("procent difference:" + item.Value.ToString());
                    attribute.AppendChild(xmlText);
                    sectionElement.AppendChild(attribute);
                }
            }
            TestResultsHeadline.AppendChild(sectionElement);
            #region SaveXmlPath
            var savepath = settings.ResultXmlSavePath + "\\XmlTestResult";
            if (!Directory.Exists(savepath)){ Directory.CreateDirectory(savepath); }
            doc.Save(savepath+"\\TestResultXmlDocument.xml");
            #endregion
        }

        public void SendResultsAsEmail()
        {

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
