using NPOI.XSSF.UserModel;
using NPOI.XSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Util;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using System.Globalization;
using System.ComponentModel;
using NPOI.SS.Util;

namespace Albert.Utilities
{
    //目前仅支持*.xls操作类
    public class NpoiExcelHelper
    {
        #region 属性
        private readonly int perSheetCount = 40000; //每个sheet要保存最大条数
        private static NpoiExcelHelper npoiExcelExportHelper;
        #endregion

        #region IExcelProvider 成员
        //单例
        public static NpoiExcelHelper Instance
        {
            get => npoiExcelExportHelper ?? (npoiExcelExportHelper = new NpoiExcelHelper());
            set => npoiExcelExportHelper = value;
        }

        public DataTable Import(Stream fs, string ext, out string msg, List<string> validates = null)
        {
            msg = string.Empty;
            var dt = new DataTable();
            try
            {
                IWorkbook workbook;
                if (ext == ".xls")
                    workbook = new HSSFWorkbook(fs);
                else
                    workbook = new XSSFWorkbook(fs);
                const int num = 0;
                var sheet = workbook.GetSheetAt(num);
                dt.TableName = sheet.SheetName;
                var rowCount = sheet.LastRowNum;
                const int firstNum = 0;
                var headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;
                if (validates != null)
                {
                    var validateCount = validates.Count;
                    if (validateCount > cellCount)
                    {
                        msg = "上传EXCEL文件格式不正确";
                        return null;
                    }
                    for (var i = 0; i < validateCount; i++)
                    {
                        var columnName = headerRow.GetCell(i).StringCellValue;
                        if (validates[i] == columnName) continue;
                        msg = "上传EXCEL文件格式不正确";
                        return null;
                    }
                }
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    dt.Columns.Add(column);
                }
                for (var i = firstNum + 1; i <= rowCount; i++)
                {
                    var row = sheet.GetRow(i);
                    var dataRow = dt.NewRow();
                    if (row != null)
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                            if (row.GetCell(j) != null)
                                dataRow[j] = GetCellValue(row.GetCell(j), ext);
                    dt.Rows.Add(dataRow);
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static IFont GetFont(IWorkbook workbook, HSSFColor color)
        {
            var font = workbook.CreateFont();
            font.Color = color.Indexed;
            font.FontHeightInPoints = 10;
            font.IsBold = true;
            font.IsItalic = true;
            return font;
        }

        public static void SetCellValues(ICell cell, string cellType, string cellValue)
        {
            switch (cellType)
            {
                case "System.String": //字符串类型
                    double result;
                    if (double.TryParse(cellValue, out result))
                        cell.SetCellValue(result);
                    else
                        cell.SetCellValue(cellValue);
                    break;
                case "System.DateTime": //日期类型
                    DateTime dateV;
                    DateTime.TryParse(cellValue, out dateV);
                    cell.SetCellValue(dateV);
                    break;
                case "System.Boolean": //布尔型
                    bool boolV;
                    bool.TryParse(cellValue, out boolV);
                    cell.SetCellValue(boolV);
                    break;
                case "System.Int16": //整型
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    int intV;
                    int.TryParse(cellValue, out intV);
                    cell.SetCellValue(intV);
                    break;
                case "System.Decimal": //浮点型
                case "System.Double":
                    double doubV;
                    double.TryParse(cellValue, out doubV);
                    cell.SetCellValue(doubV);
                    break;
                case "System.DBNull": //空值处理
                    cell.SetCellValue("");
                    break;
                default:
                    cell.SetCellValue("");
                    break;
            }
        }

        public string Export(string excelFileName, DataTable dtIn)
        {
            var workbook = new HSSFWorkbook();
            ICell cell;
            var sheetCount = 1; //当前的sheet数量
            var currentSheetCount = 0; //循环时当前保存的条数，每页都会清零

            //表头样式
            var style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            var green = new HSSFColor.Green();
            style.SetFont(GetFont(workbook, green));

            //内容样式
            style = workbook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            var blue = new HSSFColor.Blue();
            style.SetFont(GetFont(workbook, blue));

            var sheet = workbook.CreateSheet("Sheet" + sheetCount);
            //填充表头
            var row = sheet.CreateRow(0);
            for (var i = 0; i < dtIn.Columns.Count; i++)
            {
                cell = row.CreateCell(i);
                cell.SetCellValue(dtIn.Columns[i].ColumnName);
                cell.CellStyle = style;
            }
            //填充内容
            for (var i = 0; i < dtIn.Rows.Count; i++)
            {
                if (currentSheetCount >= perSheetCount)
                {
                    sheetCount++;
                    currentSheetCount = 0;
                    sheet = workbook.CreateSheet("Sheet" + sheetCount);
                }
                row = sheetCount == 1 ? sheet.CreateRow(currentSheetCount + 1) : sheet.CreateRow(currentSheetCount);
                currentSheetCount++;
                for (var j = 0; j < dtIn.Columns.Count; j++)
                {
                    cell = row.CreateCell(j);
                    cell.CellStyle = style;
                    SetCellValues(cell, dtIn.Columns[j].DataType.ToString(), dtIn.Rows[i][j].ToString());
                }
            }
            var fs = new FileStream(excelFileName, FileMode.CreateNew, FileAccess.Write);
            workbook.Write(fs);
            fs.Close();
            return excelFileName;
        }

        public DataTable Import(string filepath, string key, string sheetName, string endKey)
        {
            var table = new DataTable();
            try
            {
                using (var excelFileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    var file = Path.GetExtension(filepath);
                    if (file != null)
                    {
                        var type = file.Replace(".", "");
                        IWorkbook workbook;
                        if (type == "xls")
                            workbook = new HSSFWorkbook(excelFileStream);
                        else
                            workbook = new XSSFWorkbook(excelFileStream);

                        for (var num = 0; num < workbook.NumberOfSheets; num++)
                        {
                            var sheet = workbook.GetSheetAt(num);
                            if (sheet.SheetName != sheetName)
                                continue;
                            table.TableName = sheet.SheetName;
                            var rowCount = sheet.LastRowNum;
                            IRow headerRow = null;
                            var cellCount = 0;
                            var firstNum = 0;

                            for (var i = 0; i <= rowCount; i++)
                            {
                                if (sheet.GetRow(i).GetCell(0).StringCellValue != key) continue;
                                headerRow = sheet.GetRow(i);
                                cellCount = headerRow.LastCellNum;
                                firstNum = i;
                                break;
                            }

                            //handling header.
                            if (headerRow != null)
                                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                                {
                                    var column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                                    table.Columns.Add(column);
                                }

                            for (var i = firstNum + 1; i <= rowCount; i++)
                            {
                                var row = sheet.GetRow(i);
                                var dataRow = table.NewRow();
                                var isEnd = false;
                                if (row != null)
                                    for (int j = row.FirstCellNum; j < cellCount; j++)
                                    {
                                        if (row.GetCell(j) != null)
                                            dataRow[j] = GetCellValue(row.GetCell(j), type);
                                        if (dataRow[j].ToString() != endKey) continue;
                                        isEnd = true;
                                        break;
                                    }
                                if (isEnd)
                                    break;
                                table.Rows.Add(dataRow);
                            }
                            return table;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return table;
        }

        private static string GetCellValue(ICell cell, string type)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return string.Empty;
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric:
                    var format = cell.CellStyle.DataFormat;
                    if (format == 14 || format == 31 || format == 57 || format == 58)
                    {
                        var date = cell.DateCellValue;
                        var re = date.ToString("yyy-MM-dd");
                        return re;
                    }
                    return cell.ToString();

                case CellType.String:
                    return cell.StringCellValue;

                case CellType.Formula:
                    try
                    {
                        if (type == "xls")
                        {
                            var e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                            e.EvaluateInCell(cell);
                            return cell.ToString();
                        }
                        else
                        {
                            var e = new XSSFFormulaEvaluator(cell.Sheet.Workbook);
                            e.EvaluateInCell(cell);
                            return cell.ToString();
                        }
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    }
                case CellType.Unknown:
                    return cell.ToString();
                default:
                    return cell.ToString();
            }
        }

        #endregion

        #region 辅助导入

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datatable"></param>
        /// <returns></returns>
        public IEnumerable<T> ConvertTo<T>(DataTable datatable) where T : new()
        {
            var temp = new List<T>();
            try
            {
                var columnsNames =
                    (from DataColumn dataColumn in datatable.Columns select dataColumn.ColumnName).ToList();
                temp = datatable.AsEnumerable().ToList().ConvertAll(row => GetObject<T>(row, columnsNames));
                return temp;
            }
            catch
            {
                return temp;
            }
        }

        /// <summary>
        /// 根据DataTable生成对象，对象的属性与列同名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="columnsName"></param>
        /// <returns></returns>
        public T GetObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            var obj = new T();
            try
            {
                var properties = typeof(T).GetProperties();
                foreach (var objProperty in properties)
                {
                    var attrs = objProperty.GetCustomAttributes(typeof(DisplayNameAttribute), false);
                    if (!attrs.Any()) continue;
                    var displayName = ((DisplayNameAttribute)attrs.First()).DisplayName;

                    var columnname = columnsName.Find(s => s == displayName);
                    if (string.IsNullOrEmpty(columnname)) continue;
                    var value = row[columnname].ToString();
                    if (string.IsNullOrEmpty(value)) continue;
                    if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                    {
                        value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                        objProperty.SetValue(obj,
                            Convert.ChangeType(value,
                                Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                    }
                    else
                    {
                        value = row[columnname].ToString().Replace("%", "");
                        objProperty.SetValue(obj,
                            Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }

        public static void CopyRow(int startRow, int endRow, int pPosition, ISheet sheet)
        {
            int pStartRow = startRow - 1;
            int pEndRow = endRow - 1;
            int targetRowFrom;
            int targetRowTo;
            int cloumnCount;

            CellRangeAddress region = null;

            if (pStartRow == -1 || pEndRow == -1)
            {
                return;
            }

            //拷贝合并的单元格
            for (int k = 0; k < sheet.NumMergedRegions; k++)
            {
                region = sheet.GetMergedRegion(k);
                if (region.FirstRow >= pStartRow && region.LastRow <= pEndRow)
                {
                    targetRowFrom = region.FirstRow - pStartRow + pPosition;
                    targetRowTo = region.LastRow - pStartRow + pPosition;
                    CellRangeAddress newRegion = region.Copy();
                    newRegion.FirstRow = targetRowFrom;
                    newRegion.FirstColumn = region.FirstColumn;
                    newRegion.LastRow = targetRowTo;
                    newRegion.LastColumn = region.LastColumn;
                    sheet.AddMergedRegion(newRegion);
                }

            }

            //设置列宽
            for (int k = pStartRow; k <= pEndRow; k++)
            {
                IRow sourceRow = sheet.GetRow(k);
                cloumnCount = sourceRow.LastCellNum;
                if (sourceRow != null)
                {
                    IRow newRow = sheet.CreateRow(pPosition - pStartRow + k);
                    newRow.Height = sourceRow.Height;
                    for (int l = 0; l < cloumnCount; l++)
                    {
                        ICell templateCell = sourceRow.GetCell(l);
                        if (templateCell != null)
                        {
                            ICell newCell = newRow.CreateCell(l);
                            CopyCell(templateCell, newCell);
                        }
                    }
                }

            }


        }

        private static void CopyCell(ICell srcCell, ICell distCell)
        {
            distCell.CellStyle = srcCell.CellStyle;
            if (srcCell.CellComment != null)
            {
                distCell.CellComment = srcCell.CellComment;
            }

            CellType srcCellType = srcCell.CellType;
            distCell.SetCellType(srcCellType);

            string cellValue = GetCellValue(srcCell, "xlsx");
            SetCellValues(distCell, "System.String", cellValue);
        }

        #endregion

        #region Npoi之Excel数据导出
        /// <summary>
        /// 先创建行，然后在创建对应的列
        /// 创建Excel中指定的行
        /// </summary>
        /// <param name="sheet">Excel工作表对象</param>
        /// <param name="rowNum">创建第几行(从0开始)</param>
        /// <param name="rowHeight">行高</param>
        public HSSFRow CreateRow(ISheet sheet, int rowNum, float rowHeight)
        {
            HSSFRow row = (HSSFRow)sheet.CreateRow(rowNum); //创建行
            row.HeightInPoints = rowHeight; //设置列头行高
            return row;
        }

        /// <summary>
        /// 创建行内指定的单元格
        /// </summary>
        /// <param name="row">需要创建单元格的行</param>
        /// <param name="cellStyle">单元格样式</param>
        /// <param name="cellNum">创建第几个单元格(从0开始)</param>
        /// <param name="cellValue">给单元格赋值</param>
        /// <returns></returns>
        public HSSFCell CreateCells(HSSFRow row, HSSFCellStyle cellStyle, int cellNum, string cellValue)
        {
            HSSFCell cell = (HSSFCell)row.CreateCell(cellNum); //创建单元格
            cell.CellStyle = cellStyle; //将样式绑定到单元格
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                //单元格赋值
                cell.SetCellValue(cellValue);
            }

            return cell;
        }

        /// <summary>
        /// 行内单元格常用样式设置
        /// </summary>
        /// <param name="workbook">Excel文件对象</param>
        /// <param name="hAlignment">水平布局方式</param>
        /// <param name="vAlignment">垂直布局方式</param>
        /// <param name="fontHeightInPoints">字体大小</param>
        /// <param name="isAddBorder">是否需要边框</param>
        /// <param name="boldWeight">字体加粗 (None = 0,Normal = 400，Bold = 700</param>
        /// <param name="fontName">字体（仿宋，楷体，宋体，微软雅黑...与Excel主题字体相对应）</param>
        /// <param name="isAddBorderColor">是否增加边框颜色</param>
        /// <param name="isItalic">是否将文字变为斜体</param>
        /// <param name="isLineFeed">是否自动换行</param>
        /// <param name="isAddCellBackground">是否增加单元格背景颜色</param>
        /// <param name="fillPattern">填充图案样式(FineDots 细点，SolidForeground立体前景，isAddFillPattern=true时存在)</param>
        /// <param name="cellBackgroundColor">单元格背景颜色（当isAddCellBackground=true时存在）</param>
        /// <param name="fontColor">字体颜色</param>
        /// <param name="underlineStyle">下划线样式（无下划线[None],单下划线[Single],双下划线[Double],会计用单下划线[SingleAccounting],会计用双下划线[DoubleAccounting]）</param>
        /// <param name="typeOffset">字体上标下标(普通默认值[None],上标[Sub],下标[Super]),即字体在单元格内的上下偏移量</param>
        /// <param name="isStrikeout">是否显示删除线</param>
        /// <returns></returns>
        public HSSFCellStyle CreateStyle(HSSFWorkbook workbook, HorizontalAlignment hAlignment, VerticalAlignment vAlignment, short fontHeightInPoints, bool isAddBorder, bool boldWeight, string fontName = "宋体", bool isAddBorderColor = true, bool isItalic = false, bool isLineFeed = false, bool isAddCellBackground = false, FillPattern fillPattern = FillPattern.NoFill, short cellBackgroundColor = HSSFColor.Yellow.Index, short fontColor = HSSFColor.Black.Index, FontUnderlineType underlineStyle =
            FontUnderlineType.None, FontSuperScript typeOffset = FontSuperScript.None, bool isStrikeout = false)
        {
            HSSFCellStyle cellStyle = (HSSFCellStyle)workbook.CreateCellStyle(); //创建列头单元格实例样式
            cellStyle.Alignment = hAlignment; //水平居中
            cellStyle.VerticalAlignment = vAlignment; //垂直居中
            cellStyle.WrapText = isLineFeed;//自动换行

            //背景颜色，边框颜色，字体颜色都是使用 HSSFColor属性中的对应调色板索引，关于 HSSFColor 颜色索引对照表，详情参考：https://www.cnblogs.com/Brainpan/p/5804167.html
            //十分注意设置单元格背景色必须是FillForegroundColor和FillPattern两个属性同时设置，否则是不会显示背景颜色
            //FillForegroundColor属性实现 Excel 单元格的背景色设置
            //FillPattern 为单元格背景色的填充样式
            if (isAddCellBackground)
            {
                cellStyle.FillForegroundColor = cellBackgroundColor;//单元格背景颜色
                cellStyle.FillPattern = fillPattern;//填充图案样式(FineDots 细点，SolidForeground立体前景)
            }

            //是否增加边框
            if (isAddBorder)
            {
                //常用的边框样式
                //None(没有),Thin(细边框，瘦的),Medium(中等),Dashed(虚线),
                //Dotted(星罗棋布的),Thick(厚的),Double(双倍),Hair(头发)[上右下左顺序设置]
                cellStyle.BorderBottom = BorderStyle.Thin;
                cellStyle.BorderRight = BorderStyle.Thin;
                cellStyle.BorderTop = BorderStyle.Thin;
                cellStyle.BorderLeft = BorderStyle.Thin;
            }

            //是否设置边框颜色
            if (isAddBorderColor)
            {
                //边框颜色[上右下左顺序设置]
                cellStyle.TopBorderColor = HSSFColor.DarkGreen.Index;//DarkGreen(黑绿色)
                cellStyle.RightBorderColor = HSSFColor.DarkGreen.Index;
                cellStyle.BottomBorderColor = HSSFColor.DarkGreen.Index;
                cellStyle.LeftBorderColor = HSSFColor.DarkGreen.Index;
            }

            //设置相关字体样式
            var cellStyleFont = (HSSFFont)workbook.CreateFont(); //创建字体
            cellStyleFont.IsBold = boldWeight; //字体加粗
            cellStyleFont.FontHeightInPoints = fontHeightInPoints; //字体大小
            cellStyleFont.FontName = fontName;//字体（仿宋，楷体，宋体 ）
            cellStyleFont.Color = fontColor;//设置字体颜色
            cellStyleFont.IsItalic = isItalic;//是否将文字变为斜体
            cellStyleFont.Underline = underlineStyle;//字体下划线
            cellStyleFont.TypeOffset = typeOffset;//字体上标下标
            cellStyleFont.IsStrikeout = isStrikeout;//是否有删除线
            cellStyle.SetFont(cellStyleFont); //将字体绑定到样式

            return cellStyle;
        }
        #endregion


        //操作Excel的第Index个Sheet
        //public HSSFWorkbook GetIndexWorkbook(string filePath)
        //{
        //    IWorkbook workBook = null;
        //    try
        //    {
        //        //如果路径为空，则设置一个默认的路径
        //        if(filePath == null)
        //        {
        //            filePath = AppDomain.CurrentDomain.BaseDirectory + "AlbertNopi.xls";
        //        }
        //        //给定的文件不存在，则创建文件,创建后关闭
        //        if (!File.Exists(filePath))
        //        {
        //            File.Create(filePath).Close();                 
        //        }
        //        //获取文件后缀，当前只支持HSSFWorkBook即*.xls文件
        //        var fileSuffix = Path.GetExtension(filePath);
        //        if(fileSuffix == ".xls")
        //        {
        //            using (var excelFileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
        //            {
        //                excelFileStream.Position = 0; // 增加这句
        //                //首先创建workBook再去写，不然直接创建的文件不支持直接打开
        //                workBook = new HSSFWorkbook();
        //                workBook.Write(excelFileStream);
        //            }                                               
        //        }
        //        else
        //        {
        //            Console.WriteLine("Sorry not support it at the moment.");
        //            workBook = null;
        //        }              
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        workBook = null;
        //    }
        //    return (HSSFWorkbook)workBook;
        //}
    }
}
