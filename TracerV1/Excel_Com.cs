using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;

namespace TracerV1
{
    class Excel_Com
    {
        object misValue = System.Reflection.Missing.Value;

        public Excel.Workbook xlWorkBook;

        public Excel.Application xlApp;

        public Excel.Worksheet xlWorkSheet;

        public string errorMessage = "";

        public Excel_Com()
        {
            try
            {
                xlApp = new Microsoft.Office.Interop.Excel.Application();

                xlWorkBook = xlApp.Workbooks.Add(misValue);
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
            }

        }

        public Excel_Com(string excelSheetNameAndPath)
        {
            try
            {

                xlApp = new Microsoft.Office.Interop.Excel.Application();

                xlWorkBook = xlApp.Workbooks.Add(misValue);



                xlWorkBook.SaveAs(excelSheetNameAndPath + ".xlsx");

                closeExcelBook();

            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
            }
        }

        public bool newExcelBook(string excelSheetNameAndPath)
        {
            try
            {

                xlApp = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp == null)
                {
                    return false;
                }

                xlWorkBook = xlApp.Workbooks.Add(misValue);

                xlWorkBook.SaveAs(excelSheetNameAndPath + ".xlsx");

                ChangeWorkSheet(1);

                return true;
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return false;
            }
        }

        public void openExcelBook(string excelBookName)
        {
            try
            {

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(excelBookName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
            }
        }

        public void closeExcelBook()
        {

            try
            {
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
            }
        }

        public void ChangeWorkSheet(int sheetNumber)
        {
            try
            {
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNumber);
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
            }
        }

        public void addPicture(int left,int top, int width, int height,string FilePath)
        {
            ChangeWorkSheet(1);
            
            xlWorkSheet.Activate();

            xlWorkSheet.Shapes.AddPicture(FilePath, MsoTriState.msoFalse, MsoTriState.msoCTrue, left, top, width, height);
        }

        public Excel.Worksheet GetWorkSheet(int sheetNumber)
        {
            try
            {
                return (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNumber);
            }
            catch (Exception ex)
            {

                errorMessage = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetNumber"></param>
        /// <returns></returns>
        public DataTable getWorkSheetData(int sheetNumber)
        {
            try
            {
                Excel.Worksheet xlWorkSheetLocal = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(sheetNumber);
                Excel.Range range = xlWorkSheetLocal.UsedRange;
                DataTable dt = new DataTable();
                DataRow dr;

                for (int cl = 0; cl < range.Columns.Count; cl++)
                {
                    DataColumn column;
                    column = new DataColumn();
                    object r = (range.Cells[1, cl + 1] as Excel.Range).Value2;
                    if(cl >0)
                        column.DataType= System.Type.GetType("System.Int32");
                    else
                    column.DataType = System.Type.GetType(r.GetType().ToString());

                    column.ColumnName = r.ToString();
                    dt.Columns.Add(column);
                }

                for (int rCnt = 2; rCnt <= range.Rows.Count; rCnt++)
                {
                    dr = dt.NewRow();
                    for (int cCnt = 1; cCnt <= range.Columns.Count; cCnt++)
                    {
                        dr[cCnt - 1] = (range.Cells[rCnt, cCnt] as Excel.Range).Value2;
                    }
                    dt.Rows.Add(dr);
                }

                return dt;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                return null;
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                errorMessage = ex.Message;

            }
            finally
            {
                GC.Collect();
            }
        }

    }
}
