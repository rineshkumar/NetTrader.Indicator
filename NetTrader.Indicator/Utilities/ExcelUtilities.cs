using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;

namespace NetTrader.Indicator.Utilities
{
    public class ExcelUtilities
    {
        public static void WriteMacdhistogramDataToExcel(MACDSerie mACDSerie, string sheetName)
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
                    var row = sheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(item.DataDate.Date);
                    row.CreateCell(1).SetCellValue(item.isConvergingOrDiverging);
                    row.CreateCell(2).SetCellValue(item.EmaLineDifference.Value);
                    row.CreateCell(3).SetCellValue(item.isDiffereneAmountDecreasing);
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
