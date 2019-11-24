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


        /// <summary>
        /// PrintOutResults Settings
        /// </summary>
        public string ResultXmlSavePath = Environment.CurrentDirectory;

        /// <summary>
        /// Only show images that is compared within a specified range in procent
        /// </summary>
        public bool OnlyShowImagesBelowTheSetProcentValue = false;
        public bool OnlyShowImagesHigherThenTheSetProcentValue = false;
        public float ImagesProcentDifference = 95;
    }
}
