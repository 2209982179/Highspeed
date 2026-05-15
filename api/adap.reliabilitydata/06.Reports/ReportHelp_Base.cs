using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Replacing;
using Aspose.Words.Tables;
using highspeed.framework.Common;
using highspeed.framework.Extensions;
using Svg;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Cell = Aspose.Words.Tables.Cell;
using Row = Aspose.Words.Tables.Row;
using Table = Aspose.Words.Tables.Table;

namespace adap.safetyandreliabilityapi._06.Reports
{
    public class ReportHelp_Base
    {
        //创建word文件 
        public DocumentBuilder buildWordoc;
        public Document worDoc;
        //文档保存路径
        public string szWordSavePath;
        //格式化设置
        protected DocumentFormatSetting documentFormatSetting = new DocumentFormatSetting();

        /// <summary>
        /// 替换文档时间
        /// </summary>
        public static void TimeExchange(Document worDoc)
        {
            worDoc.Range.Replace("[报告生成时间]", DateTime.Now.ToString("yyyy年MM月"), new FindReplaceOptions());
            worDoc.Range.Replace("[文档页数]", worDoc.PageCount.ToString(), new FindReplaceOptions());
        }

        #region Word 读取/保存
        ///<summary>
        ///创建word
        ///</summary>
        /// <param name="template">word模板文件名称</param>
        public int WordStart()
        {
            //创建word文档
            worDoc = new Document();
            buildWordoc = new DocumentBuilder(worDoc);

            return 0;
        }

        ///<summary>
        ///创建word
        ///</summary>
        /// <param name="template">word模板文件名称</param>
        public int WordStart(string templatePath)
        {
            try
            {
                //创建word文档
                worDoc = new Document(templatePath);
                worDoc.CustomDocumentProperties.Add("GUID", Guid.NewGuid().ToString());
                buildWordoc = new DocumentBuilder(worDoc);
                buildWordoc.ParagraphFormat.DefaultParagrapFormatting();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                //没有找到模板
                return -1;
            }
            return 0;
        }

        ///<summary>
        ///保存word
        ///</summary>
        public int WordSave()
        {
            try
            {
                if (worDoc != null)
                {
                    //刷新全文档格式
                    worDoc.FormatAll(documentFormatSetting);
                    //更新时间日期与页数（页数在FormatAll之后可能变更，需要在其后执行）
                    TimeExchange(worDoc);
                    //保存word文档  
                    worDoc.Save(szWordSavePath, SaveFormat.Docx);
                    // 清除生成过程记录的编号列表
                    worDoc.ClearList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return -1;
            }
            return 0;
        }
        #endregion

        /**>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> 文档操作基础接口 <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<**/

        #region 基础接口

        /// <summary>
        /// 将Double值转换成固定格式的字符串
        /// </summary>
        /// <param name="val">原始值</param>
        /// <param name="isMath">是否保留位数</param>
        /// <param name="MathLen">位数</param>
        /// <param name="isPercent">是否百分比显示</param>
        /// <returns></returns>
        public string ChangeDoubleToString(double val, bool isMath, int MathLen, bool isPercent)
        {
            try
            {
                if (!double.IsNormal(val))
                {
                    return "/";
                }

                if (isPercent)
                {
                    if (isMath)
                    {
                        return Math.Round(val * 100, MathLen).ToString() + "%";
                    }
                    else
                    {
                        return (val * 100).ToString() + "%";
                    }
                }
                else
                {
                    if (isMath)
                    {
                        return Math.Round(val, MathLen).ToString();
                    }
                    else
                    {
                        return val.ToString();
                    }
                }
            }
            catch (Exception)
            {
                return val.ToString();
            }
        }


        /// <summary>
        /// 将Double值转换成固定格式的字符串
        /// </summary>
        /// <param name="val">原始值</param>
        /// <param name="isMath">是否保留位数</param>
        /// <param name="MathLen">位数</param>
        /// <param name="isPercent">是否百分比显示</param>
        /// <returns></returns>
        public string ChangeDecimalToString(decimal val, bool isMath, int MathLen, bool isPercent)
        {
            try
            {
                if (val > decimal.MaxValue || val < decimal.MinValue)
                {
                    return "/";
                }

                if (isPercent)
                {
                    if (isMath)
                    {
                        return Math.Round(val * 100, MathLen).ToString() + "%";
                    }
                    else
                    {
                        return (val * 100).ToString() + "%";
                    }
                }
                else
                {
                    if (isMath)
                    {
                        return Math.Round(val, MathLen).ToString();
                    }
                    else
                    {
                        return val.ToString();
                    }
                }
            }
            catch (Exception)
            {
                return val.ToString();
            }
        }

        /// <summary>
        /// 计算字符串宽高添加换行符
        /// </summary>
        /// <param name="iniText"></param>
        /// <param name="font"></param>
        /// <param name="RectSize"></param>
        /// <returns></returns>
        public string WrapText(string iniText, System.Drawing.Font font, Size RectSize)
        {
            if (string.IsNullOrEmpty(iniText)) { return string.Empty; }
            try
            {
                Dictionary<int, string> LineStrs = new Dictionary<int, string>();//所有行
                LineStrs.Add(0, "");
                for (int index = 0; index <= iniText.Length - 1; index++)
                {
                    //计算行宽
                    SizeF fontsize = GetStringSize(LineStrs[LineStrs.Count - 1] + iniText[index], font);
                    if (15 * LineStrs.Count > RectSize.Height)//超出高度结束
                    {
                        LineStrs.Add(LineStrs.Count, "......");
                        break;
                    }
                    if (fontsize.Width > RectSize.Width + 50)//超出宽度换行
                    {
                        LineStrs.Add(LineStrs.Count, "" + iniText[index]);
                    }
                    else
                    {
                        LineStrs[LineStrs.Count - 1] += iniText[index];
                    }
                }

                return string.Join('\n', LineStrs.Values.ToArray());
            }
            catch (Exception)
            {
                return iniText;
            }
        }

        public SizeF GetStringSize(string text, System.Drawing.Font font)
        {
            try
            {
                using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    float width = 0;
                    int lineStart = 0;
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '\n')
                        {
                            width = Math.Max(width, graphics.MeasureString(text.Substring(lineStart, i - lineStart), font).Width);
                            lineStart = i + 1;
                        }
                    }
                    // Measure the last line
                    width = Math.Max(width, graphics.MeasureString(text.Substring(lineStart), font).Width);
                    return new SizeF(width, lineStart * graphics.MeasureString(text, font).Height);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region 通用表格操作
        /// <summary>
        /// 设置单元格水平合并
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="isH">合并的行</param>
        /// <param name="cellStart">合并行中要合并的首个单元格</param>
        /// <param name="cellEnd">合并行中接受合并的单元格</param>
        /// <returns></returns>
        public int MergeHLine(Table table, int isH, int cellStart, int cellEnd)
        {
            if (isH >= table.Rows.Count || cellStart > cellEnd || cellEnd > table.Rows[isH].Count)
            {
                return -1;
            }
            //设置合并行的首单元格
            table.Rows[isH].Cells[cellStart].CellFormat.HorizontalMerge = CellMerge.First;
            for (int i = cellStart; i < cellEnd; i++)
            {
                //设置单元格水平合并
                table.Rows[isH].Cells[i + 1].CellFormat.HorizontalMerge = CellMerge.Previous;
            }
            return 0;
        }

        /// <summary>
        /// 合并列
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="isV">合并列</param>
        /// <param name="cellStart">合并列开始的行号</param>
        /// <param name="cellEnd">合并列结束的行号</param>
        /// <returns></returns>
        public int MergeVLine(Table table, int isV, int cellStart, int cellEnd)
        {
            if (cellStart >= cellEnd || cellEnd >= table.Rows.Count || isV >= table.Rows[cellStart].Count)
            {
                return -1;
            }
            //设置合并列的首单元格
            table.Rows[cellStart].Cells[isV].CellFormat.VerticalMerge = CellMerge.First;
            for (int i = cellStart; i < cellEnd; i++)
            {
                //设置单元格垂直合并
                table.Rows[i + 1].Cells[isV].CellFormat.VerticalMerge = CellMerge.Previous;
            }
            return 0;
        }

        /// <summary>
        /// 设置表格线条宽度及颜色
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="color">颜色</param>
        /// <param name="bordersWidth">表网格线宽度</param>
        /// <param name="borderWidth">表框线宽</param>
        /// <param name="lineStyle">表格线风格</param>
        public void SetTableLine(Table table, Color color, double bordersWidth = 1.0, double borderWidth = 1.0, LineStyle lineStyle = LineStyle.Single)
        {
            table.SetBorders(lineStyle, bordersWidth, color);
            table.SetBorder(BorderType.Bottom, lineStyle, borderWidth, color, true);
            table.SetBorder(BorderType.Top, lineStyle, borderWidth, color, true);
            table.SetBorder(BorderType.Left, lineStyle, borderWidth, color, true);
            table.SetBorder(BorderType.Right, lineStyle, borderWidth, color, true);
            table.AllowAutoFit = false;
        }

        /// <summary>
        /// 添加一行数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="count"></param>
        /// <param name="Content"></param>
        public void AddOneLine(Row row, int count, string Content)
        {
            row.Cells.Add(new Cell(worDoc));
            row.Cells[count].AppendChild(new Paragraph(worDoc));
            row.Cells[count].Paragraphs[0].AppendChild(new Run(worDoc, Content ?? string.Empty));
        }

        /// <summary>
        /// 删除指定空列（检查指定列的值是否为空）
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool RemoveEmptyColumns(DataTable dt, string columnName)
        {
            if (!dt.Columns.Contains(columnName))
                throw new ArgumentException($"列名'{columnName}'不存在");

            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull(columnName) &&
                    !string.IsNullOrWhiteSpace(row[columnName].ToString()))
                {
                    return false;
                }
            }

            dt.Columns.Remove(columnName);
            return true;
        }
        #endregion

        #region 通用表插入
        /// <summary>
        /// 通用插入表接口
        /// </summary>
        /// <param name="signTest">书签</param>
        /// <param name="FMDatas">数据源</param>
        /// <param name="Cols">中文字段</param>
        /// <param name="NewCols">英文字段</param>
        /// <returns></returns>
        public Table? CreateTable_Common(string signTest, DataTable dt)
        {
            //获取标签,移动光标到该位置
            Bookmark bookmark = worDoc.Range.Bookmarks[signTest];
            buildWordoc.MoveToBookmark(signTest);

            // 在书签位置插入表格
            Table table = buildWordoc.StartTable();

            // 向表格中添加字段 
            table.Rows.Add(new Row(worDoc));
            Row row0 = table.Rows[0];
            for (int i = 0; i <= dt.Columns.Count - 1; i++)
            {
                row0.Cells.Add(new Cell(worDoc));
                row0.Cells[i].AppendChild(new Paragraph(worDoc));
                row0.Cells[i].Paragraphs[0].AppendChild(new Run(worDoc, dt.Columns[i].ColumnName));
            }
            foreach (Cell cell in row0.Cells)
            {
                cell.Paragraphs[0].Runs[0].Font.Size = 10.5;
                cell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
                cell.Paragraphs[0].ParagraphFormat.FirstLineIndent = 0;
                cell.Paragraphs[0].ParagraphFormat.LeftIndent = 0;
                cell.Paragraphs[0].ParagraphFormat.RightIndent = 0;
            }

            int index = 0;
            foreach (DataRow row in dt.Rows)
            {
                index += 1;
                table.Rows.Add(new Row(worDoc));
                Row row1 = table.Rows[index];

                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {
                    row1.Cells.Add(new Cell(worDoc));
                    if (row[i] != null && row[i].ToString().Contains("；"))
                    {
                        string[] strings = row[i].ToString().Split('；');
                        int indexS = -1;
                        foreach (string s in strings)
                        {
                            indexS += 1;
                            row1.Cells[i].AppendChild(new Paragraph(worDoc));
                            row1.Cells[i].Paragraphs[indexS].AppendChild(new Run(worDoc, s));
                        }
                    }
                    else
                    {
                        row1.Cells[i].AppendChild(new Paragraph(worDoc));
                        row1.Cells[i].Paragraphs[0].AppendChild(new Run(worDoc, row[i].ToString()));
                    }
                }
                foreach (Cell cell in row1.Cells)
                {
                    cell.Paragraphs[0].Runs[0].Font.Size = 10.5;
                    cell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    cell.Paragraphs[0].ParagraphFormat.FirstLineIndent = 0;
                    cell.Paragraphs[0].ParagraphFormat.LeftIndent = 0;
                    cell.Paragraphs[0].ParagraphFormat.RightIndent = 0;
                }
            }

            //将光标移出表格 
            table.Alignment = TableAlignment.Center;
            SetTableLine(table, Color.Black);

            // 设置表格自动适应内容
            table.AllowAutoFit = true;
            table.AutoFit(AutoFitBehavior.AutoFitToWindow);

            //移除书签
            bookmark.Remove();
            worDoc.Range.Replace("[" + signTest + "]", "", new FindReplaceOptions());
            return table;
        }

        /// <summary>
        /// 通用插入表接口
        /// </summary>
        /// <param name="signTest">书签</param>
        /// <param name="FMDatas">数据源</param>
        /// <param name="Cols">中文字段</param>
        /// <param name="NewCols">英文字段</param>
        /// <returns></returns>
        public Table? CreateTable_Top(string signTest, DataTable dt)
        {
            /*Dictionary<int, DataTable> valueTable = new Dictionary<int, DataTable>();
            foreach (var item in dt.Rows)
            {
              //  if (item["产品ID"].)
            }*/


            //获取标签,移动光标到该位置
            Bookmark bookmark = worDoc.Range.Bookmarks[signTest];
            buildWordoc.MoveToBookmark(signTest);

            // 在书签位置插入表格
            Table table = buildWordoc.StartTable();

            // 向表格中添加字段 
            table.Rows.Add(new Row(worDoc));
            Row row0 = table.Rows[0];
            for (int i = 0; i <= dt.Columns.Count - 1; i++)
            {
                row0.Cells.Add(new Cell(worDoc));
                row0.Cells[i].AppendChild(new Paragraph(worDoc));
                row0.Cells[i].Paragraphs[0].AppendChild(new Run(worDoc, dt.Columns[i].ColumnName));
            }
            foreach (Cell cell in row0.Cells)
            {
                cell.Paragraphs[0].Runs[0].Font.Size = 10.5;
                cell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
                cell.Paragraphs[0].ParagraphFormat.FirstLineIndent = 0;
                cell.Paragraphs[0].ParagraphFormat.LeftIndent = 0;
                cell.Paragraphs[0].ParagraphFormat.RightIndent = 0;
            }

            int index = 0;
            foreach (DataRow row in dt.Rows)
            {
                index += 1;
                table.Rows.Add(new Row(worDoc));
                Row row1 = table.Rows[index];

                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {
                    row1.Cells.Add(new Cell(worDoc));
                    if (row[i] != null && row[i].ToString().Contains("；"))
                    {
                        string[] strings = row[i].ToString().Split('；');
                        int indexS = -1;
                        foreach (string s in strings)
                        {
                            indexS += 1;
                            row1.Cells[i].AppendChild(new Paragraph(worDoc));
                            row1.Cells[i].Paragraphs[indexS].AppendChild(new Run(worDoc, s));
                        }
                    }
                    else
                    {
                        row1.Cells[i].AppendChild(new Paragraph(worDoc));
                        row1.Cells[i].Paragraphs[0].AppendChild(new Run(worDoc, row[i].ToString()));
                    }
                }
                foreach (Cell cell in row1.Cells)
                {
                    cell.Paragraphs[0].Runs[0].Font.Size = 10.5;
                    cell.Paragraphs[0].ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    cell.Paragraphs[0].ParagraphFormat.FirstLineIndent = 0;
                    cell.Paragraphs[0].ParagraphFormat.LeftIndent = 0;
                    cell.Paragraphs[0].ParagraphFormat.RightIndent = 0;
                }
            }

            //将光标移出表格 
            table.Alignment = TableAlignment.Center;
            SetTableLine(table, Color.Black);

            // 设置表格自动适应内容
            table.AllowAutoFit = true;
            table.AutoFit(AutoFitBehavior.AutoFitToWindow);

            //移除书签
            bookmark.Remove();
            worDoc.Range.Replace("[" + signTest + "]", "", new FindReplaceOptions());
            return table;
        }

        /// <summary>
        /// 通用插入表接口-批量
        /// </summary>
        /// <param name="signTest">书签</param>
        /// <param name="FMDatas">数据源</param>
        /// <param name="Cols">中文字段</param>
        /// <param name="NewCols">英文字段</param>
        /// <returns></returns>
        public void CreateTable_CommonList(string signTest, List<DataTable> dts)
        {
            //获取标签,移动光标到该位置
            Bookmark bookmark = worDoc.Range.Bookmarks[signTest];
            if (bookmark == null) return;

            buildWordoc.MoveTo(bookmark.BookmarkEnd);

            foreach (DataTable dt in dts)
            {
                buildWordoc.WriteTableTitleAnnotation("顶事件 " + dt.TableName);
                buildWordoc.Write("\r");
                Table table = buildWordoc.StartTable();

                // 向表格中添加字段 
                Row topRow = new Row(worDoc);
                table.Rows.Add(topRow);
                for (int i = 0; i <= dt.Columns.Count - 1; i++)
                {
                    var cell = new Cell(worDoc);
                    topRow.Cells.Add(cell);
                    cell.AppendChild(new Paragraph(worDoc));
                    cell.Paragraphs[0].AppendChild(new Run(worDoc, dt.Columns[i].ColumnName));

                    var paragraphFormat = cell.Paragraphs[0].ParagraphFormat;
                    paragraphFormat.Style.Font.Size = 10.5;
                    paragraphFormat.Alignment = ParagraphAlignment.Center;
                    paragraphFormat.FirstLineIndent = 0;
                    paragraphFormat.LeftIndent = 0;
                    paragraphFormat.RightIndent = 0;
                }

                int index = 0;
                foreach (DataRow dbRow in dt.Rows)
                {
                    index += 1;
                    Row docRow = new Row(worDoc);
                    table.Rows.Add(docRow);

                    for (int i = 0; i <= dt.Columns.Count - 1; i++)
                    {
                        var cell = new Cell(worDoc);
                        docRow.Cells.Add(cell);
                        if (dbRow[i] != null && dbRow[i].ToString().Contains("；"))
                        {
                            string[] strings = dbRow[i].ToString().Split('；');
                            int indexS = -1;
                            foreach (string s in strings)
                            {
                                indexS += 1;
                                cell.AppendChild(new Paragraph(worDoc));
                                cell.Paragraphs[indexS].AppendChild(new Run(worDoc, s));
                            }
                        }
                        else
                        {
                            cell.AppendChild(new Paragraph(worDoc));
                            cell.Paragraphs[0].AppendChild(new Run(worDoc, dbRow[i].ToString()));
                        }

                        var paragraphFormat = cell.Paragraphs[0].ParagraphFormat;
                        paragraphFormat.Style.Font.Size = 10.5;
                        paragraphFormat.Alignment = ParagraphAlignment.Center;
                        paragraphFormat.FirstLineIndent = 0;
                        paragraphFormat.LeftIndent = 0;
                        paragraphFormat.RightIndent = 0;
                    }
                }

                //将光标移出表格 
                table.Alignment = TableAlignment.Center;
                SetTableLine(table, Color.Black);

                // 设置表格自动适应内容
                table.AllowAutoFit = true;
                table.AutoFit(AutoFitBehavior.AutoFitToWindow);
            }

            //移除书签
            bookmark.Remove();
            worDoc.Range.Replace("[" + signTest + "]", "", new FindReplaceOptions());

        }
        #endregion

        #region 通用图片插入

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="signTest">标记字符</param> 
        /// <param name="picPath">图片地址</param>
        /// <returns></returns>
        public Shape? CreatePicture(string picPath, Size BitMapSize)
        {
            // 加载要插入的图片文件 
            if (File.Exists(picPath))
            {
                buildWordoc.Font.Spacing = 1;
                buildWordoc.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                buildWordoc.ParagraphFormat.Alignment = ParagraphAlignment.Center;

                Shape shape = buildWordoc.InsertImage(picPath);

                if (BitMapSize.Width <= 500)
                {
                    shape.Width = BitMapSize.Width;
                    shape.Height = BitMapSize.Height;
                }
                else
                {
                    shape.Width = worDoc.FirstSection.PageSetup.PageWidth - worDoc.FirstSection.PageSetup.LeftMargin - worDoc.FirstSection.PageSetup.RightMargin;
                    shape.Height = shape.Width * (BitMapSize.Height * 1.00 / BitMapSize.Width);
                }

                shape.AspectRatioLocked = false;
                shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;
                shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
                shape.VerticalAlignment = VerticalAlignment.Inline;
                shape.HorizontalAlignment = HorizontalAlignment.Inside;
                shape.WrapType = WrapType.Inline;
                return shape;
            }
            else
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 设置当前报告所有表格重复标题行
        /// </summary>
        public void SetAllTableRepeatHead()
        {
            foreach (Table table in worDoc.GetChildNodes(NodeType.Table, true))
            {
                // 确保表格至少有一行
                if (table.Rows.Count > 0)
                {
                    // 设置第一行作为重复标题行
                    table.FirstRow.RowFormat.HeadingFormat = true;
                }
            }
            return;
        }
    }
}
