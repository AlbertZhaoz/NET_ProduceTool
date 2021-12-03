// See https://aka.ms/new-console-template for more information
using Albert.Utilities;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

NpoiExcelOperationService npoiExcelOperationService = new NpoiExcelOperationService();
npoiExcelOperationService.ExcelDataExport();

/// <summary>
/// Excel文档生成并保存操作类
/// <see href="https://www.cnblogs.com/Can-daydayup/p/12501400.html">简单Demon使用案例分析</see>
/// </summary>
public class NpoiExcelOperationService
{
    /// <summary>
    /// Excel数据导出简单示例
    /// </summary>
    /// <param name="resultMsg">导出结果</param>
    /// <param name="excelFilePath">保存excel文件路径</param>
    /// <returns></returns>
    public void ExcelDataExport()
    {
        try
        {
            var filePath = @"F:\Repo\producetool\NpoiExcelExample\bin\Debug\net6.0\UploadFile\AlbertNpoi_20211203171747.xls";
            using var excelFileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            var workBook = new HSSFWorkbook(excelFileStream);
            Console.WriteLine(workBook.NumberOfSheets); 

            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet(DateTime.Now.ToString("%y.%M.%d"));          
            //设置顶部大标题样式
            var cellStyleFont = NpoiExcelHelper.Instance.CreateStyle(workbook, HorizontalAlignment.Center, VerticalAlignment.Center, 20, true, true, "楷体", true, false, false, true, FillPattern.SolidForeground, HSSFColor.Coral.Index, HSSFColor.White.Index,
                FontUnderlineType.None, FontSuperScript.None, false);
            //第一行表单
            var row = NpoiExcelHelper.Instance.CreateRow(sheet, 0, 28);
            //路径文件夹路径
            var uploadPath = AppDomain.CurrentDomain.BaseDirectory + "UploadFile\\";
            string excelFileName = uploadPath +  "AlbertNpoi" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
            //创建目录文件夹
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            Console.WriteLine(excelFileName);
            //使用FileStream文件流来写入数据（传入参数为：文件所在路径，对文件的操作方式，对文件内数据的操作）
            var fileStream = new FileStream(excelFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //向Excel文件对象写入文件流，生成Excel文件
            workbook.Write(fileStream);
            //关闭文件流
            fileStream.Close();
            //释放流所占用的资源
            fileStream.Dispose();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    /// <summary>
    /// Excel数据导出简单示例
    /// </summary>
    /// <param name="resultMsg">导出结果</param>
    /// <param name="excelFilePath">保存excel文件路径</param>
    /// <returns></returns>
    public void ExcelDataExportAlbert()
    {
        
    }
}