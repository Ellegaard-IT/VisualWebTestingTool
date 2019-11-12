using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace Ellegaard_VisualWebTestingTool
{
    public class TestPreprocess
    {
        string dataUrl;
        public TestPreprocess(string testDataSavePath = null)
        {
            dataUrl = testDataSavePath ?? Environment.CurrentDirectory;
            if (!Directory.Exists(dataUrl + "\\VisualImageTestData"))
            {
                CreateDirPath(dataUrl);
            }
            if (!File.Exists(dataUrl + "\\VisualImageTestData\\ImageTestDataController.sqlite"))
            {
                CreateDatabase(dataUrl);
            }
        }

        void CreateDirPath(string DirPath)
        {
            Directory.CreateDirectory(DirPath + "\\VisualImageTestData\\Imagefolder");
        }

        void CreateDatabase(string DirPath)
        {
            File.Create(DirPath + "\\VisualImageTestData\\ImageTestDataController.sqlite");
        }

        public void CreateSubDir(string dirName)
        {
            Directory.CreateDirectory(dataUrl + "\\VisualImageTestData\\Imagefolder\\" + dirName);
        }
    }
}
