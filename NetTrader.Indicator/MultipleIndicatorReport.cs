using NetTrader.Indicator.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Configuration;
using System.IO;
using System.Threading;

namespace NetTrader.Indicator.Test.ForPoc
{
    public class MultipleIndicatorReport
    {
        static void Main(string[] args)
        {
            DownloadAllStockFiles();

            string[] stockNames = ConfigurationManager.AppSettings["StockNames"].Split(',');
            foreach (var stockName in stockNames)
            {
                MACD macd = new MACD(false);

                MACDSerie macdSeries = GetMacdSeries(macd, stockName);
                RSI rsi = new RSI(14);
                RSISerie rsiSeries = GetRsiSeries(rsi, stockName);
                SMAV2 shortTermSma = new SMAV2(14);
                SingleDoubleSerieV2 shorttermSingleDoubleSerieV2 = GetSingleDoubleSeriesV2(shortTermSma, stockName);
                SMAV2 longTermSma = new SMAV2(60);
                SingleDoubleSerieV2 longtermSingleDoubleSerieV2 = GetSingleDoubleSeriesV2(longTermSma, stockName);

                ExcelUtilities.WriteMacdhistogramDataToExcel(macdSeries, rsiSeries, shorttermSingleDoubleSerieV2, longtermSingleDoubleSerieV2, stockName);
            }

        }


        private static void DownloadAllStockFiles()
        {
            IWebDriver driver;
            CleanFolder();
            string[] stockNames = ConfigurationManager.AppSettings["StockNames"].Split(',');
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

        private static MACDSerie GetMacdSeries(MACD macd, string stockName)
        {
            string downloadFolderName = ConfigurationManager.AppSettings["downloadLocation"];
            macd.Load(downloadFolderName + stockName + ".ax.csv");
            MACDSerie serie = macd.Calculate();

            //ExcelUtilities.WriteMacdhistogramDataToExcel(serie, stockName);
            return serie;
        }
        private static RSISerie GetRsiSeries(RSI rsi, string stockName)
        {
            string downloadFolderName = ConfigurationManager.AppSettings["downloadLocation"];
            rsi.Load(downloadFolderName + stockName + ".ax.csv");

            RSISerie serie = rsi.Calculate();

            //ExcelUtilities.WriteMacdhistogramDataToExcel(serie, stockName);
            return serie;
        }
        private static SingleDoubleSerieV2 GetSingleDoubleSeriesV2(SMAV2 shortTermSma, string stockName)
        {
            string downloadFolderName = ConfigurationManager.AppSettings["downloadLocation"];
            shortTermSma.Load(downloadFolderName + stockName + ".ax.csv");

            SingleDoubleSerieV2 serie = shortTermSma.Calculate();

            //ExcelUtilities.WriteMacdhistogramDataToExcel(serie, stockName);
            return serie;

        }

        private static void CleanFolder()
        {
            string downloadFolderName = ConfigurationManager.AppSettings["downloadLocation"];
            System.IO.DirectoryInfo di = new DirectoryInfo(downloadFolderName);

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
