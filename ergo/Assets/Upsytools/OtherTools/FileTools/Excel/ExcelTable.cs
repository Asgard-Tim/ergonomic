using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OfficeOpenXml;

public class ExcelTable
{
    private Dictionary <int, Dictionary<int, ExcelTableCell>> cells = new Dictionary<int, Dictionary<int, ExcelTableCell>>();

    public string TableName;
    public int NumberOfRows;
    public int NumberOfColumns;

    //public Vector2 Position;

    public ExcelTable()
    {

    }

    public ExcelTable(ExcelWorksheet sheet)
    {
        TableName = sheet.Name;
	if (sheet.Dimension != null)
	{
		NumberOfRows = sheet.Dimension.Rows;
		NumberOfColumns = sheet.Dimension.Columns;
	}
	else
	{
		//empty Sheet
		NumberOfRows = 0;
		NumberOfColumns = 0;
	}
        for (int row = 1; row <= NumberOfRows; row++)
        {
            for (int column = 1; column <= NumberOfColumns; column++)
            {
                string value = ""; //default value for empty cell
		if (sheet.Cells [row, column].Value != null)
		{
			value = sheet.Cells [row, column].Value.ToString ();
		}
                SetValue(row, column, value);
            }
        }
    }

    public ExcelTableCell SetValue(int row, int column, object value)
    {
		CorrectSize(row, column);
        if (!cells.ContainsKey(row))
        {
            cells[row] = new Dictionary<int, ExcelTableCell>();
        }
        if (cells[row].ContainsKey(column))
        {
            cells[row][column].Value = value.ToString();

            return cells[row][column];
        }
        else
        {
            ExcelTableCell cell = new ExcelTableCell(row, column, value.ToString());
            cells[row][column] = cell;
            return cell;
        }
    }
    /// <summary>
    /// 获取某单元格内的数据
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public string GetValue(int row, int column)
    {
        ExcelTableCell cell = GetCell(row, column);
        if (cell != null)
        {
            return cell.Value;
        }
        else
        {
            return SetValue(row, column, "").Value;
        }
    }
    /// <summary>
    /// 获取范围内所有的单元格数据
    /// </summary>
    /// <param name="startRowIndex">初始行</param>
    /// <param name="startColumnIndex">初始列</param>
    /// <param name="endRowIndex">结束行</param>
    /// <param name="endColumnIndex">结束列</param>
    /// <returns></returns>
    public string[,] GetAllValue(int startRowIndex, int startColumnIndex,int endRowIndex,int endColumnIndex)
    {
        var row = endRowIndex - startRowIndex + 1;
        var column = endColumnIndex - startColumnIndex + 1;
        if (row > NumberOfRows)
        {
            row = NumberOfRows;
        }

        if (column > NumberOfColumns)
        {
            column = NumberOfRows;
        }
        var str=new string[row,column];
        for (int i = startRowIndex; i < endRowIndex+1; i++)
        {
            for (int j = startColumnIndex; j < endColumnIndex+1; j++)
            {
                str[i - startRowIndex, j - startColumnIndex] = GetValue(i, j);
            }
        }
        return str;
    }
    /// <summary>
    /// 获取表中所有单元格数据
    /// </summary>
    /// <returns></returns>
    public string[,] GetTableValue()
    {
        var str=new string[NumberOfRows,NumberOfColumns];
        for (int i = 1; i < NumberOfRows+1; i++)
        {
            for (int j = 1; j < NumberOfColumns+1; j++)
            {
                str[i - 1, j - 1] = GetValue(i, j);
            }
        }
        return str;
    }
    public ExcelTableCell GetCell(int row, int column)
    {
        if (cells.ContainsKey(row))
        {
            if (cells[row].ContainsKey(column))
            {
                return cells[row][column];
            }
        }
        return null;
    }

    public void CorrectSize(int row, int column)
    {
        NumberOfRows = Mathf.Max(row, NumberOfRows);
        NumberOfColumns = Mathf.Max(column, NumberOfColumns);
    }

    public void SetCellTypeRow(int rowIndex, ExcelTableCellType type)
    {
        for (int column = 1; column <= NumberOfColumns; column++)
        {
            ExcelTableCell cell = GetCell(rowIndex, column);
            if (cell != null)
            {
                cell.Type = type;
            }
        }
    }

    public void SetCellTypeColumn(int columnIndex, ExcelTableCellType type, List<string> values = null)
    {
        for (int row = 1; row <= NumberOfRows; row++)
        {
            ExcelTableCell cell = GetCell(row, columnIndex);
            if (cell != null)
            {
                cell.Type = type;
                if (values != null)
                {
                    cell.ValueSelected = values;
                }
            }
        }
    }

    public void ShowLog() {
        string msg = "";
        for (int row = 1; row <= NumberOfRows; row++)
        {
            for (int column = 1; column <= NumberOfColumns; column++)
            {
                msg += string.Format("{0} ", GetValue(row, column));
            }
            msg += "\n";
        }
        Debug.Log(msg);
    }


}
