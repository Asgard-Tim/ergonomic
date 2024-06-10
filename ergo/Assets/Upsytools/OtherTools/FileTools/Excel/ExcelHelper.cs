using System;
using UnityEngine;
using System.Collections.Generic;
using OfficeOpenXml;
using System.IO;

public class ExcelHelper
{

    public static List<string[,]> LoadExcelToArray(string path)
    {
        List<string[,]> sheets=new List<string[,]>();
        var excel =LoadExcel(path);
        for (int i = 0; i < excel.Tables.Count; i++)
        {
            sheets.Add(excel.Tables[i].GetTableValue());
        }

        return sheets;
    }

    public static void ArrayToExcel(string path,object[,] data)
    {
        FileInfo fi = new FileInfo(path);
        if (fi.Directory != null && !fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        var excel= CreateExcel(path);
        var row = data.GetLength(0);
        var column = data.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                excel.Tables[0].SetValue(i+1, j+1, data[i, j]);
            }
        }
        SaveOverExcel(excel, path);
    }
    
    public static Excel LoadExcel(string path)
    {
        FileInfo file = new FileInfo(path);
        ExcelPackage ep = new ExcelPackage(file);
        Excel xls = new Excel(ep.Workbook);
        return xls;
    }

	public static Excel CreateExcel(string path) {
		ExcelPackage ep = new ExcelPackage ();
        ep.Workbook.Worksheets.Add ("sheet");
        Debug.Log(ep.Workbook.Worksheets.Count);
		Excel xls = new Excel(ep.Workbook);
		//SaveExcel (xls, path);
        //SaveOverExcel(xls, path);
		return xls;
	}

    /// <summary>
    ///  保存Excel表；
    /// </summary>
    /// <param name="xls"></param>
    /// <param name="path">路径</param>
    public static void SaveExcel(Excel xls, string path)
    {
        FileInfo output = new FileInfo(path);
        ExcelPackage ep =new ExcelPackage(output);
        int length = ep.Workbook.Worksheets.Count;
        Debug.Log(length);
        int count = xls.Tables.Count;
 
        for (int i = 0; i < count; i++)
        {
            ExcelTable table = xls.Tables[i];
            //依次覆盖之前的表格；
            string key = table.TableName;
            ExcelWorksheet sheet = null;
            //如果表中没有数据，则跳过；
            if (table.NumberOfRows + table.NumberOfColumns == 0)
            {
                continue;
            }
            //获取正确的数据表
            if (i >= length)
                sheet = ep.Workbook.Worksheets.Add(table.TableName);
            else
            {
                //鬼鬼，他这个下标从1开始的真的是太秀了！
                sheet = ep.Workbook.Worksheets[i + 1];
                sheet.Name = table.TableName;
            }
 
            for (int row = 1; row <= table.NumberOfRows; row++)
            {
                for (int column = 1; column <= table.NumberOfColumns; column++)
                {
                    sheet.Cells[row, column].Value = table.GetValue(row, column);
                }
            }
        }
        try
        {
            ep.SaveAs(output);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        } 
    }
    
    public static void SaveOverExcel(Excel xls, string path)
    {
        FileInfo output = new FileInfo(path);
        ExcelPackage ep = new ExcelPackage();
        for (int i = 0; i < xls.Tables.Count; i++)
        {
            ExcelTable table = xls.Tables[i];
            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add(table.TableName);
            for (int row = 1; row <= table.NumberOfRows; row++) {
                for (int column = 1; column <= table.NumberOfColumns; column++) {
                    sheet.Cells[row, column].Value = table.GetValue(row, column);
                }
            }
        }

        try
        {
            ep.SaveAs(output);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        } 
    }
    
}
