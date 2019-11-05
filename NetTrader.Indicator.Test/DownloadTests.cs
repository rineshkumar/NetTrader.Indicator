using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Threading;

namespace NetTrader.Indicator.Test
{
    [TestClass]
    public class DownloadTests
    {
        IWebDriver driver;
        [TestMethod]
        public void TestMethod1()
        {
            CleanFolder();
            string[] stockNames = { "vas", "stw"/*, "ioz", "mvw", "ioo" */};
            string historyUrltemplate = @"https://au.finance.yahoo.com/quote/{0}.AX/history/";
            string url = "";
            foreach (var stock in stockNames)
            {
                url = string.Format(historyUrltemplate, stock);
                driver = new ChromeDriver(@"C:\chromewebdriver\new78\");
                driver.Url = url;
                var element = driver.FindElement(By.CssSelector(@"a[href*='download']"));
                element.Click();
                Thread.Sleep(3000);
                driver.Close();
            }


        }

        private void CleanFolder()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(@"C:\Users\rines\Downloads\");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
