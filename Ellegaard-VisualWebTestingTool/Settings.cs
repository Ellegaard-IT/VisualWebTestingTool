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
        //Xml settings
        public string ResultXmlSavePath = Environment.CurrentDirectory;

        //Mail settings
        public bool IncludeXmlFileInMail = false;
        public string MailSubject = "VisualTestResultMail";

        //ImageShowingDifferenceNumbers
        public bool CreateImageShowingDifferencePixelPoints = true;
        public bool InsertImageInMail = false;
        public string ShowImageDifferencePointsImageDir = Environment.CurrentDirectory;

        /// <summary>
        /// Only show images that is compared within a specified range in procent
        /// </summary>
        public bool OnlyShowImagesBelowTheSetProcentValue = true;
        public bool OnlyShowImagesHigherThenTheSetProcentValue = false;
        public float ImagesProcentDifference = 95;
    }
}
