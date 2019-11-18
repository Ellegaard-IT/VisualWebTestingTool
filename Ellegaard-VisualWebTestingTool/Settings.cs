using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ellegaard_VisualWebTestingTool
{
    public class Settings
    {
        public int AmountOfImagesForEachTest = 5;
        public int AmountOfSecTheImagesAreTakenOver = 3;
        public string TestDataSavePath = Environment.CurrentDirectory;
        public ScreenshotImageFormat ImagesFormat = ScreenshotImageFormat.Bmp;
    }
}
