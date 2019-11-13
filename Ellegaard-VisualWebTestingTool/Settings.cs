using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ellegaard_VisualWebTestingTool
{
    public partial class Settings
    {
        public static int AmountOfImagesForEachTest = 5;
        public static int AmountOfSecTheImagesAreTakenOver = 3;
        public static string TestDataSavePath = Environment.CurrentDirectory;
        public static ScreenshotImageFormat ImagesFormat = ScreenshotImageFormat.Bmp;
    }
}
