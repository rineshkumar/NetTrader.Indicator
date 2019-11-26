using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetTrader.Indicator.Utilities
{
    public class ExcelUtilities
    {
        public static void WriteMacdhistogramDataToExcel(MACDSerie mACDSerie, RSISerie rsiSeries, SingleDoubleSerieV2 shorttermSingleDoubleSerieV2, SingleDoubleSerieV2 longtermSingleDoubleSerieV2, string sheetName)
        {
            var subset = mACDSerie.MACDHistogramDataList.GetRange(mACDSerie.MACDHistogramDataList.Count - 60, 60);
            //Search For the template file

            XSSFWorkbook workbook;
            string excelFile = sheetName + "Data.xlsx";
            File.Delete(excelFile);
            // File.Copy(@"MacdHDataTemplate.xlsx", excelFile);

            using (FileStream file = new FileStream(@"MacdHDataTemplate.xlsx", FileMode.Open, FileAccess.Read))
            {

                workbook = new XSSFWorkbook(file);
                ISheet sheet = workbook.GetSheet("template");

                //sheet = sheet.CopySheet(sheetName);
                //                sheet = workbook.CloneSheet(workbook.IndexOf(sheet));


                int i = 1;
                foreach (var item in subset)
                {

                    List<BuySellSignal> signals = new List<BuySellSignal>();
                    signals = MACD.SetSignalType(item, signals);
                    var row = sheet.CreateRow(i);
                    var rsiData = rsiSeries.rsiDataPoints.Where(x => x.Date == item.DataDate).FirstOrDefault();
                    var shortTermSingleDoubleSeriesData = shorttermSingleDoubleSerieV2.Values.Where(x => x.date == item.DataDate).FirstOrDefault();
                    var longTermSingleDoubleSeriesData = longtermSingleDoubleSerieV2.Values.Where(x => x.date == item.DataDate).FirstOrDefault();
                    shortTermSingleDoubleSeriesData.findSignals(longTermSingleDoubleSeriesData, signals, item.ClosingValue);

                    row.CreateCell(0).SetCellValue(item.DataDate.Date.ToString("dd/MM/yyyy"));
                    row.CreateCell(1).SetCellValue(item.isConvergingOrDiverging.ToString());
                    row.CreateCell(2).SetCellValue(item.EmaLineDifference.Value);
                    row.CreateCell(3).SetCellValue(item.changeInDivergenceMomentum.Value);
                    row.CreateCell(4).SetCellValue(item.isDiffereneAmountDecreasing);
                    //if (rsiData != null)
                    //{
                    //    row.CreateCell(5).SetCellValue(rsiData.RS.Value);
                    //    row.CreateCell(6).SetCellValue(rsiData.RSI.Value);
                    //}
                    if (longTermSingleDoubleSeriesData != null && shortTermSingleDoubleSeriesData != null)
                    {
                        row.CreateCell(7).SetCellValue(shortTermSingleDoubleSeriesData.data.Value);
                        row.CreateCell(8).SetCellValue(longTermSingleDoubleSeriesData.data.Value);
                        row.CreateCell(9).SetCellValue(shortTermSingleDoubleSeriesData.data.Value < longTermSingleDoubleSeriesData.data.Value);
                    }
                    row.CreateCell(10).SetCellValue(item.ClosingValue);
                    row.CreateCell(11).SetCellValue(string.Join(Environment.NewLine, signals.ToArray()));
                    i++;
                }

            }
            using (FileStream file = new FileStream(excelFile, FileMode.CreateNew, FileAccess.Write))
            {
                workbook.Write(file);
                file.Close();

            }
            //Write to specific Rows in the excel file 

        }

    }
}
