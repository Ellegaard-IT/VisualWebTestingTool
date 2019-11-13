using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace Ellegaard_VisualWebTestingTool
{
    public class TestSectionPreprocess: Settings
    {
        string testSectionName;

        /// <summary>
        /// Sets up directorys and stuff for the test system, create one object for each test section you have.
        /// </summary>
        /// <param name="testSectionName">If you have multiple test sections on your site, you set the name of each here</param>
        /// <param name="testDataSavePath">The path for where you want your test data document to be storing its data</param>
        public TestSectionPreprocess(string testSectionName)
        {
            if (!Directory.Exists(TestDataSavePath + "\\VisualImageTestData"))
            {
                CreateDirPath(TestDataSavePath,testSectionName);
            }
        }
        
        void CreateDirPath(string dirPath, string testSectionName)
        {
            Directory.CreateDirectory(dirPath + "\\VisualImageTestData\\"+testSectionName);
        }

        public void CreateTestDirectory(string testName, List<Screenshot> screenshotList)
        {
            string saveUrl = TestDataSavePath + "\\VisualImageTestData\\" + testSectionName + "\\" + testName;
            int counter = 1;
            if (Directory.Exists(saveUrl))
            {
#if !DEBUG
                throw new Exception("The test named: " + testName + " already exist, couldnt create the test");
#endif
            }
            
            Directory.CreateDirectory(saveUrl);            
            foreach (var screenshot in screenshotList)
            {
                screenshot.SaveAsFile(saveUrl + "\\" + testName+counter, ImagesFormat);
                counter++;
            }
        }
    }
}
