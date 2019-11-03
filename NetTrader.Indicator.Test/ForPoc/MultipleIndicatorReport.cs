using NetTrader.Indicator.Utilities;
using System.IO;

namespace NetTrader.Indicator.Test.ForPoc
{
    public class MultipleIndicatorReport
    {
        static void Main(string[] args)
        {
            MACD macd = new MACD(false);
            string stockName = "stw";
            MACDSerie macdSeries = GetMacdSeries(macd, stockName);
            RSI rsi = new RSI(14);
            RSISerie rsiSeries = GetRsiSeries(rsi, stockName);
            ExcelUtilities.WriteMacdhistogramDataToExcel(macdSeries, rsiSeries, stockName);
        }
        private static MACDSerie GetMacdSeries(MACD macd, string stockName)
        {
            macd.Load(Directory.GetCurrentDirectory() + "\\" + stockName + ".csv");
            MACDSerie serie = macd.Calculate();

            //ExcelUtilities.WriteMacdhistogramDataToExcel(serie, stockName);
            return serie;
        }
        private static RSISerie GetRsiSeries(RSI rsi, string stockName)
        {
            rsi.Load(Directory.GetCurrentDirectory() + "\\" + stockName + ".csv");
            RSISerie serie = rsi.Calculate();

            //ExcelUtilities.WriteMacdhistogramDataToExcel(serie, stockName);
            return serie;
        }
    }
}
