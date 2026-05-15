using adap.safetyandreliabilityapi._05.Data.Reliability_Prediction;
using Aspose.Words;
using Aspose.Words.Replacing;
using Aspose.Words.Tables;
using highspeed.framework.Common;
using System.Drawing;
using Document = Aspose.Words.Document;
using Row = Aspose.Words.Tables.Row;

namespace adap.safetyandreliabilityapi._06.Reports
{
    public class ReportHelp_TestCase : ReportHelp_Base
    {
        /// <summary>
        /// 根据模板文件保存Word报告
        /// </summary>
        /// <param name="WordSavePath">临时文件</param>
        /// <param name="ModelFile">模板文件</param>
        /// <param name="ExportParaColumns">参数字段</param> 
        /// <returns></returns>
        public Document SaveWord_WithModel(string WordSavePath, string ModelFile)
        {
            try
            {
                //复制模板
                File.Copy(ModelFile, WordSavePath);
                szWordSavePath = WordSavePath;

                //word开始
                WordStart(WordSavePath);

                SetAllTableRepeatHead();  //表格重复标题行
                Thread.Sleep(100);
                //保存word
                WordSave();
                return worDoc;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw new Exception("导出报告失败：" + ex.Message + "\r\n" + ex.ToString());
            }
        }
    }
}
