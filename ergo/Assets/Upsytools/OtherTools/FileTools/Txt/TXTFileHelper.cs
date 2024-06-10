using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TXTFileHelper 
{
    public static string[,] LoadTxt(string fullPath,string separator)
    {
        FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read,FileShare.ReadWrite);
        
        StreamReader sr = new StreamReader(fs, Encoding.UTF8);
        //记录每次读取的一行记录
        string strLine = "";
        List<string[]> values=new List<string[]>();
        //记录每行记录中的各字段内容
        string[] aryLine = null;
        //标示列数
        int columnCount = 0;
        //标示是否是读取的第一行
        bool IsFirst = true;
        //逐行读取CSV中的数据
        while ((strLine = sr.ReadLine()) != null)
        {
            aryLine = strLine.Split(new []{separator},StringSplitOptions.RemoveEmptyEntries);
                if (IsFirst)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                }
                values.Add(aryLine);
        }

        var row = values.Count;
        var array=new string[row,columnCount];
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                array[i,j]= values[i][j];
            }
        }
        sr.Close();
        fs.Close();
        return array;
    }
    
    
    // public static void SaveTxt(string path, string separator,object[] data)
    // {
    //     FileInfo fi = new FileInfo(path);
    //     if (fi.Directory != null && !fi.Directory.Exists)
    //     {
    //         fi.Directory.Create();
    //     }
    //     //FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite,FileShare.ReadWrite);
    //     StreamWriter sw = new StreamWriter(path,true);
    //     var sb = new StringBuilder();
    //     for (int i = 0; i < data.Length; i++)
    //     {
    //         sb.Append(data[i]);
    //         if (i != data.Length - 1)
    //         {
    //             sb.Append(separator);
    //         }
    //     }
    //     // write column header
    //     sw.WriteLine(sb.ToString());
    //     sw.Flush();
    //     sw.Close();
    //     //fs.Close();
    //    
    // }
    
    
    public static void SaveTxt(string path, string separator,object[,] data)
    {
        FileInfo fi = new FileInfo(path);
        if (fi.Directory != null && !fi.Directory.Exists)
        {
            fi.Directory.Create();
        }
        //FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite,FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(path, true,Encoding.UTF8);
        var sb = new StringBuilder();
        var row = data.GetLength(0);
        var column = data.GetLength(1);
        for (int i = 0; i < column; i++)
        {
            sb.Append(data[0, i]);
            if (i != column - 1)
            {
                sb.Append(separator);
            }
        }
        // write column header
        sw.WriteLine(sb.ToString());
        
        for (int i = 1; i < row; i++)
        {
            sb.Clear();
            for (int j = 0; j < column; j++)
            {
                sb.Append(data[i, j]);
                if (j != column - 1)
                {
                    sb.Append(separator);
                }
            }
            // write row value
            sw.WriteLine(sb.ToString());
        }
        sw.Flush();
        sw.Close();
        //fs.Close();
       
    }
}
