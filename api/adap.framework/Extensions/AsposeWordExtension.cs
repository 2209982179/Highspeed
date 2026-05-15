using highspeed.framework.Common;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Layout;
using Aspose.Words.Lists;
using Aspose.Words.Rendering;
using Aspose.Words.Replacing;
using Aspose.Words.Tables;
using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace highspeed.framework.Extensions
{
    public static class AsposeWordExtension
    {
        #region 插入题注

        /// <summary>
        /// 写入表格题注-正文。插入指针停留在题注段落结尾
        /// </summary>
        /// <param name="builder">DocumentBuilder</param>
        /// <param name="text">题注内容</param>
        public static void WriteTableTitleAnnotation(this DocumentBuilder builder, string text)
        {
            WriteTitleAnnotation(builder, text, "SEQ 表 \\* ARABIC", "表 ", "居中题注");
        }

        /// <summary>
        /// 写入表格题注-附录。插入指针停留在题注段落结尾
        /// </summary>
        /// <param name="builder">DocumentBuilder</param>
        /// <param name="text">题注内容</param>
        /// <param name="appendixIndex">附录序号（A-Z）</param>
        public static void WriteAppendixTableTitleAnnotation(this DocumentBuilder builder, string text, string appendixIndex)
        {
            WriteTitleAnnotation(builder, text, $"SEQ 表{appendixIndex} \\* ARABIC", $"表{appendixIndex}.", "居中题注");
        }

        /// <summary>
        /// 写入表格题注-正文。插入指针停留在题注段落结尾
        /// </summary>
        /// <param name="builder">DocumentBuilder</param>
        /// <param name="text">题注内容</param>
        public static void WritePictureTitleAnnotation(this DocumentBuilder builder, string text)
        {
            WriteTitleAnnotation(builder, text, "SEQ 图 \\* ARABIC", "图 ", "居中题注");
        }

        /// <summary>
        /// 写入表格题注-附录。插入指针停留在题注段落结尾
        /// </summary>
        /// <param name="builder">DocumentBuilder</param>
        /// <param name="text">题注内容</param>
        /// <param name="appendixIndex">附录序号（A-Z）</param>
        public static void WriteAppendixPictureTitleAnnotation(this DocumentBuilder builder, string text, string appendixIndex)
        {
            WriteTitleAnnotation(builder, text, $"SEQ 图{appendixIndex} \\* ARABIC", $"图{appendixIndex}.", "居中题注");
        }

        /// <summary>
        /// 写入题注。插入指针停留在题注段落结尾
        /// </summary>
        /// <param name="builder">DocumentBuilder</param>
        /// <param name="text">题注内容</param>
        /// <param name="fieldCode">域Code</param>
        /// <param name="fieldPrefix">域起始文字</param>
        /// <param name="styleName">题注样式名</param>
        private static void WriteTitleAnnotation(this DocumentBuilder builder, string text, string fieldCode, string fieldPrefix, string styleName)
        {
            var style = builder.Document.Styles.FirstOrDefault(x => x.Name == styleName);
            if (style == null) style = GetDefaultTitleAnnotationStyle(builder.Document);
            style.Font.Color = Color.Black;
            Paragraph paragraph = builder.NewLine();
            paragraph.ParagraphFormat.Style = style;
            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            paragraph.AppendChild(new Run(builder.Document, fieldPrefix));
            paragraph.AppendField(fieldCode);
            paragraph.AppendChild(new Run(builder.Document, " " + text));
            builder.MoveTo(paragraph);
        }

        /// <summary>
        /// 获取默认的“居中题注”样式
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static Style GetDefaultTitleAnnotationStyle(Document document)
        {
            var style = document.Styles.FirstOrDefault(x => x.Name == "居中题注");
            if (style != null) return style;

            style = document.Styles.AddCopy(document.Styles.DefaultParagraphFormat.Style);

            style.BaseStyleName = document.Styles.DefaultParagraphFormat.Style.Name;
            style.Name = "居中题注";
            style.NextParagraphStyleName = style.BaseStyleName;

            // 字体
            style.Font.TitleDefault();

            return style;
        }

        #endregion 插入题注

        #region 插入章节标题

        /// <summary>
        /// 写入章节标题。插入指针停留在章节标题段落结尾
        /// </summary>
        /// <param name="builder">DocumentBuilder</param>
        /// <param name="text">题注内容</param>
        /// <param name="headingLevel">标题级别</param>
        public static void WriteChapterHeading(this DocumentBuilder builder, string text, int headingLevel = 1)
        {
            StyleIdentifier styleIdentifier = StyleIdentifier.Normal;
            System.Enum.TryParse("Heading" + headingLevel, out styleIdentifier);
            Paragraph paragraph = builder.NewLine();
            paragraph.ParagraphFormat.StyleIdentifier = styleIdentifier;
            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            paragraph.AppendChild(new Run(builder.Document, text ?? string.Empty));
            builder.MoveTo(paragraph);
        }

        private static Dictionary<string, List> DocList = new Dictionary<string, List>();

        public static void ClearList(this Document document)
        {
            var removeKeys = DocList.Keys.Where(key => key.StartsWith(document.CustomDocumentProperties["GUID"].Value.ToString()))
                                         .ToList();
            removeKeys.ForEach(key => DocList.Remove(key));
        }

        /// <summary>
        /// 编号列表格式化
        /// </summary>
        /// <param name="list"></param>
        public static void DefaultFormatting(this List list)
        {
            list.ListLevels.ForEach(level =>
            {
                // 居左
                level.Alignment = ListLevelAlignment.Left;
                // 编号缩进
                level.NumberPosition = 0;
                // 文字缩进
                level.TextPosition = 0;
                // 编号之后使用空格分隔
                level.TrailingCharacter = ListTrailingCharacter.Space;
            });
        }

        /// <summary>
        /// 写入章节标题。插入指针停留在章节标题段落结尾
        /// </summary>
        /// <param name="builder">DocumentBuilder</param>
        /// <param name="text">题注内容</param>
        /// <param name="appendixIndex">附录序号（A-Z）</param>
        /// <param name="headingLevel">标题级别</param>
        public static void WriteAppendixChapterHeading(this DocumentBuilder builder, string text, string appendixIndex, int headingLevel = 1)
        {
            List list = null;
            var key = $"{builder.Document.CustomDocumentProperties["GUID"]}-AppendixList{appendixIndex}";
            if (!DocList.ContainsKey(key))
            {
                list = builder.Document.Lists.Add(ListTemplate.NumberDefault);
                list.DefaultFormatting();

                for (int i = 0; i < list.ListLevels.Count; i++)
                {
                    ListLevel level = list.ListLevels[i];
                    level.NumberFormat = $"附录{appendixIndex}.{string.Join('.', Enumerable.Repeat("\0", i + 1))}";
                    level.NumberStyle = NumberStyle.Arabic;
                    level.RestartAfterLevel = i - 1;
                    level.StartAt = 1;
                    level.Font.Size = 12;
                }

                DocList.Add(key, list);
            }
            else list = DocList[key];

            StyleIdentifier styleIdentifier = StyleIdentifier.Normal;
            System.Enum.TryParse("Heading" + headingLevel, out styleIdentifier);
            Paragraph paragraph = builder.NewLine();
            paragraph.ListFormat.List = list;
            paragraph.ListFormat.ListLevelNumber = headingLevel - 1;
            paragraph.ParagraphFormat.StyleIdentifier = styleIdentifier;
            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            paragraph.AppendChild(new Run(builder.Document, text ?? string.Empty));
            builder.MoveTo(paragraph);
        }

        #endregion 插入章节标题

        #region 插入新行（段落）

        /// <summary>
        /// 在当前段落后插入一个正文格式的新行（新段落）。插入指针停留在段落结尾
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="text"></param>
        public static Paragraph NewLine(this DocumentBuilder builder, string text = "")
        {
            builder.MoveTo(builder.CurrentParagraph);
            builder.InsertBreak(BreakType.ParagraphBreak);
            var paragraph = builder.InsertParagraph();
            paragraph.ParagraphFormat.DefaultParagrapFormatting();
            paragraph.AppendChild(new Run(builder.Document, text ?? string.Empty));
            builder.MoveTo(paragraph);
            return paragraph;
        }

        /// <summary>
        /// 在文档中替换为段落，多行为多个段落。插入指针停留在最后一个段落结尾
        /// </summary>
        /// <param name="document"></param>
        /// <param name="mark"></param>
        /// <param name="lines"></param>
        /// <returns>最后一个段落</returns>
        public static Paragraph ReplaceWithParagraphs(this DocumentBuilder builder, string mark, IEnumerable<string> lines)
        {
            Paragraph last = null;
            if (lines?.Count() > 0)
            {
                NodeCollection paragraphs = builder.Document.GetChildNodes(NodeType.Paragraph, true);
                foreach (Paragraph paragraph in paragraphs)
                {
                    //找到标记所在的段落
                    if (paragraph.GetText().IndexOf(mark) >= 0)
                    {
                        // 插入指针移动到段落最后
                        builder.MoveTo(paragraph);
                        foreach (string line in lines)
                        {
                            last = builder.NewLine(line);
                        }
                        break;
                    }
                }
                builder.Document.Range.Replace(mark, "", new FindReplaceOptions());
            }
            return last;
        }

        #endregion 插入新行（段落）

        #region 插入图片

        /// <summary>
        /// 在当前段落后插入一个图片。插入指针停留在图片段落结尾，如果有题注则停留在题注结尾
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="stream"></param>
        /// <param name="annotation"></param>
        /// <param name="appendixIndex"></param>
        public static Shape? NewShape(this DocumentBuilder builder, SKBitmap image, string annotation = null, string appendixIndex = null)
        {
            if (image == null) return null;
            var parentParagraph = builder.NewLine();
            parentParagraph.ParagraphFormat.DefaultShapeFormatting();
            var shape = builder.InsertImage(image);
            shape.DistanceLeft = 0;
            shape.DistanceRight = 0;
            shape.DistanceTop = 0;
            shape.DistanceBottom = 0;
            shape.WrapType = WrapType.Inline;
            shape.AspectRatioLocked = true;
            // 防止图像插入错位，移动到指定的段落
            shape.ParentParagraph.RemoveChild(shape);
            parentParagraph.AppendChild(shape);
            builder.MoveTo(parentParagraph);

            if (!string.IsNullOrEmpty(annotation))
            {
                if (string.IsNullOrEmpty(appendixIndex)) WritePictureTitleAnnotation(builder, annotation);
                else WriteAppendixPictureTitleAnnotation(builder, annotation, appendixIndex);
            }

            return shape;
        }

        /// <summary>
        /// 在当前段落后插入一个图片。插入指针停留在图片段落结尾，如果有题注则停留在题注结尾
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="image"></param>
        /// <param name="annotation"></param>
        /// <param name="appendixIndex"></param>
        public static Shape? NewShape(this DocumentBuilder builder, Image image, string annotation = null, string appendixIndex = null)
        {
            if (image == null) return null;
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                var skImage = SKBitmap.Decode(stream);
                return NewShape(builder, skImage, annotation, appendixIndex);
            }
        }

        /// <summary>
        /// 在当前段落后插入一个图片。插入指针停留在图片段落结尾，如果有题注则停留在题注结尾
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="image"></param>
        /// <param name="annotation"></param>
        /// <param name="appendixIndex"></param>
        public static Shape? NewShape(this DocumentBuilder builder, Stream stream, string annotation = null, string appendixIndex = null)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var skImage = SKBitmap.Decode(stream);
            return NewShape(builder, skImage, annotation, appendixIndex);
        }

        #endregion 插入图片

        #region 段落格式

        /// <summary>
        /// 默认段落格式(正文)
        /// </summary>
        /// <param name="format"></param>
        public static void DefaultParagrapFormatting(this ParagraphFormat format)
        {
            format.ClearFormatting();
            //设置字体
            format.Style.Font.Default();
            // 段落对齐
            format.Alignment = ParagraphAlignment.Left;
            // 段落行距20磅
            format.LineSpacing = 20;
            format.LineSpacingRule = LineSpacingRule.Exactly;
            // 设置段前距、段后距
            format.SpaceBefore = 0;
            format.SpaceAfter = 0;
            // 设置缩进
            format.LeftIndent = 0;
            format.RightIndent = 0;
            // 首行缩进2个字
            format.FirstLineIndent = format.Style.Font.Size * 2;
        }

        /// <summary>
        /// 默认段落格式(图片)
        /// </summary>
        /// <param name="format"></param>
        public static void DefaultShapeFormatting(this ParagraphFormat format)
        {
            format.DefaultParagrapFormatting();
            // 居中显示
            format.Alignment = ParagraphAlignment.Center;
            // 单倍行距
            format.LineSpacing = 12;
            format.LineSpacingRule = LineSpacingRule.Multiple;
            // 设置段前距、段后距
            format.SpaceBefore = 0;
            format.SpaceAfter = 0;
            // 设置缩进
            format.LeftIndent = 0;
            format.RightIndent = 0;
            // 首行缩进2个字
            format.FirstLineIndent = 0;
        }

        /// <summary>
        /// 设置段落字体
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="font"></param>
        public static void SetupFont(this Paragraph paragraph, Aspose.Words.Font font = null)
        {
            if (font == null) font = paragraph.ParagraphFormat.Style.Font;
            foreach (Run run in paragraph.Runs)
            {
                // 设置字体名称
                run.Font.Name = font.Name;
                run.Font.NameAscii = font.NameAscii;
                run.Font.NameAscii = font.NameBi;
                run.Font.NameAscii = font.NameFarEast;
                run.Font.NameAscii = font.NameOther;
                // 设置字体大小
                run.Font.Size = font.Size;
                run.Font.SizeBi = font.SizeBi;
                // 设置字体颜色（例如：红色）
                run.Font.Color = font.Color;
                // 设置字体是否为粗体
                run.Font.Bold = font.Bold;
                // 设置字体是否为斜体
                run.Font.Italic = font.Italic;
                // 设置字体是否为下划线
                run.Font.Underline = font.Underline;
            }
        }

        /// <summary>
        /// 是否在跳过格式化书签“IgnoreFormatXXX”内
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        public static bool IsIgnoreFormat(this Paragraph paragraph)
        {
            bool ignoreFormat = false;
            if (paragraph.ParentNode != null)
            {
                foreach (BookmarkStart bookmark in paragraph.ParentNode.GetChildNodes(NodeType.BookmarkStart, true))
                {
                    if (bookmark.Name.StartsWith("IgnoreFormat"))
                    {
                        ignoreFormat = true;
                        break;
                    }
                }

                if (ignoreFormat) return ignoreFormat;

                foreach (BookmarkEnd bookmark in paragraph.ParentNode.GetChildNodes(NodeType.BookmarkEnd, true))
                {
                    if (bookmark.Name.StartsWith("IgnoreFormat"))
                    {
                        ignoreFormat = true;
                        break;
                    }
                }
            }
            return ignoreFormat;
        }

        #endregion 段落格式

        #region 字体格式

        /// <summary>
        /// 将字体设置为标题默认字体
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static Aspose.Words.Font TitleDefault(this Aspose.Words.Font font, double? fontSize = null)
        {
            font.Color = Color.Black;
            font.Name = "黑体";
            font.NameAscii = "黑体";
            font.NameBi = "Arial";
            font.NameFarEast = "黑体";
            font.NameOther = "黑体";
            font.Size = fontSize ?? FontSizeInPoints.小四; // 标题默认大小为小四
            font.SizeBi = fontSize ?? FontSizeInPoints.小四; // 标题默认大小为小四
            return font;
        }

        /// <summary>
        /// 将字体设置为表格默认字体
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static Aspose.Words.Font TableDefault(this Aspose.Words.Font font, double? fontSize = null)
        {
            font.Color = Color.Black;
            font.Name = "宋体";
            font.NameAscii = "宋体";
            font.NameBi = "Arial";
            font.NameFarEast = "宋体";
            font.NameOther = "宋体";
            font.Size = fontSize ?? FontSizeInPoints.五号; // 默认大小为五号
            font.SizeBi = fontSize ?? FontSizeInPoints.五号; // 默认大小为五号
            return font;
        }

        /// <summary>
        /// 将字体设置为页眉页脚默认字体
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static Aspose.Words.Font HeaderFooterDefault(this Aspose.Words.Font font, double? fontSize = null)
        {
            font.Color = Color.Black;
            font.Name = "宋体";
            font.NameAscii = "宋体";
            font.NameBi = "Arial";
            font.NameFarEast = "宋体";
            font.NameOther = "宋体";
            font.Size = fontSize ?? FontSizeInPoints.五号; // 默认大小为五号
            font.SizeBi = fontSize ?? FontSizeInPoints.五号; // 默认大小为五号
            return font;
        }

        /// <summary>
        /// 将字体设置为正文默认字体
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static Aspose.Words.Font Default(this Aspose.Words.Font font, double? fontSize = null)
        {
            font.Color = Color.Black;
            font.Name = "宋体";
            font.NameAscii = "宋体";
            font.NameBi = "Arial";
            font.NameFarEast = "宋体";
            font.NameOther = "宋体";
            font.Size = fontSize ?? FontSizeInPoints.小四; // 标题默认大小为小四
            font.SizeBi = fontSize ?? FontSizeInPoints.小四; // 标题默认大小为小四
            return font;
        }

        #endregion 字体格式

        #region 文档格式化

        /// <summary>
        /// 刷新全文档格式
        /// </summary>
        /// <param name="builder"></param>
        public static void FormatAll(this Document document, DocumentFormatSetting? formatSetting = null)
        {
            if (formatSetting == null) formatSetting = new DocumentFormatSetting();

            // 刷文档标题样式设置
            FlushHeadingStyle(document);

            // 图、表题注域编码（正文+附录）
            var annotationRegex = new Regex(@"SEQ [表图]([A-Z])? \\\* ARABIC", RegexOptions.IgnoreCase);
            var tableAnnotationRegex = new Regex(@"SEQ 表([A-Z])? \\\* ARABIC", RegexOptions.IgnoreCase);

            // 遍历文档段落设置格式
            foreach (Paragraph paragraph in document.GetChildNodes(NodeType.Paragraph, true))
            {
                // 去空行
                if ((string.IsNullOrWhiteSpace(paragraph.GetText()) || paragraph.GetText() == "\r\n")
                    && !paragraph.ChildNodes.Any(x => x.NodeType == NodeType.Shape))
                {
                    paragraph.Remove();
                    continue;
                }

                // 处理跳过格式化书签“IgnoreFormatXXX”
                if (paragraph.IsIgnoreFormat()) continue;

                // 处理"项目代号："在保存后未知原因的格式变化
                if (paragraph.GetText().Contains("项目代号："))
                {
                    // 居右
                    paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    // 设置缩进
                    paragraph.ParagraphFormat.LeftIndent = 0;
                    paragraph.ParagraphFormat.RightIndent = 0;
                    // 首行缩进
                    paragraph.ParagraphFormat.FirstLineIndent = 0;
                    continue;
                }

                // 跳过表格段落或表格内的段落
                if (paragraph.ParentNode is Cell
                    || paragraph.ParentNode?.ParentNode is Cell
                    || paragraph.ChildNodes.Any(x => x.NodeType == NodeType.Table)
                    ) continue;

                paragraph.ParagraphFormat.KeepWithNext = false;

                // 标题段落
                // 附录标题
                if (paragraph.IsAppendixHeader())
                {
                    //设置字体
                    var font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                    if (font != null)
                    {
                        font.TitleDefault(FontSizeInPoints.四号);
                        paragraph.SetupFont(font);
                    }

                    // 段落对齐居中
                    paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    // 段落单倍行距
                    paragraph.ParagraphFormat.LineSpacing = 12;
                    paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                    // 设置缩进
                    paragraph.ParagraphFormat.LeftIndent = 0;
                    paragraph.ParagraphFormat.RightIndent = 0;
                    // 首行缩进
                    paragraph.ParagraphFormat.FirstLineIndent = 0;
                    // 设置段前距、段后距
                    paragraph.ParagraphFormat.SpaceBefore = 8;
                    paragraph.ParagraphFormat.SpaceAfter = 8;
                }
                else if (paragraph.ParagraphFormat.StyleIdentifier.ToString().StartsWith("Heading"))
                {
                    //设置字体
                    var font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                    if (font != null)
                    {
                        font.TitleDefault();
                        paragraph.SetupFont(font);
                    }

                    // 段落对齐居左
                    paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                    // 段落单倍行距
                    paragraph.ParagraphFormat.LineSpacing = 12;
                    paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;

                    // 设置缩进
                    paragraph.ParagraphFormat.LeftIndent = 0;
                    paragraph.ParagraphFormat.RightIndent = 0;
                    // 首行缩进
                    paragraph.ParagraphFormat.FirstLineIndent = 0;

                    // 设置段前距、段后距
                    // 一级标题
                    if (paragraph.ParagraphFormat.StyleIdentifier == StyleIdentifier.Heading1)
                    {
                        // 附录一级标题
                        if (paragraph.IsAppendixChapterHeading(1))
                        {
                            paragraph.ParagraphFormat.SpaceBefore = 3;
                            paragraph.ParagraphFormat.SpaceAfter = 3;
                        }
                        else
                        {
                            paragraph.ParagraphFormat.SpaceBefore = 12;
                            paragraph.ParagraphFormat.SpaceAfter = 12;
                        }
                    }
                    // 其他标题
                    else
                    {
                        paragraph.ParagraphFormat.SpaceBefore = 3;
                        paragraph.ParagraphFormat.SpaceAfter = 3;
                    }
                }
                // 图、表题注
                else if (annotationRegex.IsMatch(paragraph.GetText()))
                {
                    //设置字体
                    var font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                    if (font != null)
                    {
                        font.TitleDefault();
                        paragraph.SetupFont(font);
                    }

                    // 段落对齐
                    paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    // 段落单倍行距
                    paragraph.ParagraphFormat.LineSpacing = 12;
                    paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;

                    // 设置段前距、段后距
                    paragraph.ParagraphFormat.SpaceBefore = 3;
                    paragraph.ParagraphFormat.SpaceAfter = 3;

                    // 设置缩进
                    paragraph.ParagraphFormat.LeftIndent = 0;
                    paragraph.ParagraphFormat.RightIndent = 0;
                    // 首行缩进
                    paragraph.ParagraphFormat.FirstLineIndent = 0;
                }
                // 正文
                else
                {
                    double? fontSize = null; // null 为默认字体大小
                    // 当正文段落为“注”时，字体大小为五号
                    if (paragraph.GetText().StartsWith("注：")) fontSize = 10.5;

                    //设置字体
                    var font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                    if (font != null)
                    {
                        font.Default(fontSize);
                        paragraph.SetupFont(font);
                    }

                    if (paragraph.GetText().Trim(new char[] { ' ', '\r', '\n' }).Replace(" ", "") == "目次")
                    {
                        font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                        if (font != null)
                        {
                            font.TitleDefault(FontSizeInPoints.三号); // 黑体三号
                            paragraph.SetupFont(font);
                        }
                        // 居中
                        paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;

                        // 段落单倍行距
                        paragraph.ParagraphFormat.LineSpacing = 12;
                        paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;

                        // 设置段前距、段后距：1.5 行（转换为磅数值）
                        paragraph.ParagraphFormat.SpaceBefore = FontSizeInPoints.三号 * 1.5;
                        paragraph.ParagraphFormat.SpaceAfter = FontSizeInPoints.三号 * 1.5;

                        // 首行缩进2个字
                        paragraph.ParagraphFormat.FirstLineIndent = 0;
                    }
                    else
                    {
                        // 段落对齐
                        paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;

                        // 段落行距20磅
                        paragraph.ParagraphFormat.LineSpacing = 20;
                        paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;

                        // 设置段前距、段后距
                        paragraph.ParagraphFormat.SpaceBefore = 0;
                        paragraph.ParagraphFormat.SpaceAfter = 0;

                        // 首行缩进2个字
                        paragraph.ParagraphFormat.FirstLineIndent = paragraph.ParagraphFormat.Style.Font.Size * 2;
                    }

                    // 设置缩进
                    paragraph.ParagraphFormat.LeftIndent = 0;
                    paragraph.ParagraphFormat.RightIndent = 0;
                }
            }

            // 遍历文档表格设置格式
            foreach (Table table in document.GetChildNodes(NodeType.Table, true))
            {
                // 仅遍历有表格题注的表格
                if (table.PreviousSibling is Paragraph pp && tableAnnotationRegex.IsMatch(pp.GetText()))
                {
                    // 遍历表格中的所有行
                    foreach (Row row in table.Rows)
                    {
                        // 遍历行中的所有单元格
                        foreach (Cell cell in row.Cells)
                        {
                            // 获取单元格中的所有段落
                            foreach (Paragraph paragraph in cell.GetChildNodes(NodeType.Paragraph, true))
                            {
                                //设置字体
                                var font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                                if (font != null)
                                {
                                    font.TableDefault();
                                    paragraph.SetupFont(font);
                                }

                                // 段落单倍行距
                                paragraph.ParagraphFormat.LineSpacing = 12;
                                paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;

                                // 设置段前距、段后距
                                paragraph.ParagraphFormat.SpaceBefore = 0;
                                paragraph.ParagraphFormat.SpaceAfter = 0;

                                // 设置缩进
                                paragraph.ParagraphFormat.LeftIndent = 0;
                                paragraph.ParagraphFormat.RightIndent = 0;
                                // 首行缩进2个字
                                paragraph.ParagraphFormat.FirstLineIndent = 0;
                            }
                        }
                    }
                }
            }

            // 遍历图设置格式
            foreach (Shape shape in document.GetChildNodes(NodeType.Shape, true))
            {
                // 跳过表格内的图
                if (shape.ParentNode is Cell || shape.ParentNode?.ParentNode is Cell) continue;

                // 对图所在段落设置段落格式
                if (shape.ParentNode is Paragraph para)
                {
                    // 处理跳过格式化书签“IgnoreFormatXXX”
                    if (para.IsIgnoreFormat()) continue;

                    para.ParagraphFormat.DefaultShapeFormatting();
                }
            }

            // 遍历页眉页脚设置格式
            foreach (Section section in document.Sections)
            {
                if (section.HeadersFooters.Any())
                {
                    foreach (HeaderFooter headerFooter in section.HeadersFooters)
                    {
                        // 判断是否是页眉页脚
                        if (headerFooter.IsHeader || headerFooter.HeaderFooterType.ToString().StartsWith("Footer"))
                        {
                            foreach (Paragraph paragraph in headerFooter.Paragraphs)
                            {
                                //设置字体
                                var font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                                if (font != null)
                                {
                                    font.HeaderFooterDefault();
                                    paragraph.SetupFont(font);
                                }

                                // 段落单倍行距
                                paragraph.ParagraphFormat.LineSpacing = 12;
                                paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;

                                // 设置段前距、段后距
                                paragraph.ParagraphFormat.SpaceBefore = 0;
                                paragraph.ParagraphFormat.SpaceAfter = 0;

                                // 设置缩进
                                paragraph.ParagraphFormat.LeftIndent = 0;
                                paragraph.ParagraphFormat.RightIndent = 0;
                                // 首行缩进2个字
                                paragraph.ParagraphFormat.FirstLineIndent = 0;
                            }
                        }
                    }
                }
            }

            // 刷新全文布局
            document.UpdatePageLayout();

            #region 如果图高度超过阈值，设置图缩放

            // 遍历图设置缩放
            foreach (Shape shape in document.GetChildNodes(NodeType.Shape, true))
            {
                // 仅处理图片
                if (!shape.IsImage) continue;

                shape.WrapType = WrapType.Inline;
                shape.AspectRatioLocked = true; // 锁定缩放宽高比
                var page = shape.GetLocatedPage();
                if (page == null || page.Value.PageInfo == null) continue;

                var pageSetup = shape.ParentParagraph?.ParentSection.PageSetup ?? document.FirstSection.PageSetup;
                var rect = page.Value.Rectangle;
                var pageHeight = double.Parse(rect.Height.ToString()) - pageSetup.TopMargin - pageSetup.BottomMargin;
                var pageWidth = double.Parse(rect.Width.ToString()) - pageSetup.LeftMargin - pageSetup.RightMargin;
                var zoom = shape.Height / pageHeight;

                var zoomLimit = formatSetting.ShapeZoomHeightLimit;

                // 高度超过阈值，进行缩放
                if (zoom > zoomLimit)
                {
                    var targetHeight = pageHeight * zoomLimit;
                    zoom = targetHeight / shape.Height;

                    if (shape.Width * zoom > pageWidth)
                    {
                        shape.Width = pageWidth;
                    }
                    else
                    {
                        shape.Height = targetHeight;
                    }
                }
                else if (shape.Width > pageWidth) // 高度缩放不超过时，检查宽度不超过页宽
                    shape.Width = pageWidth;

                // 图像充满减少留白
                shape.DistanceLeft = 0;
                shape.DistanceRight = 0;
                shape.DistanceTop = 0;
                shape.DistanceBottom = 0;
            }

            #endregion 如果图高度超过阈值，设置图缩放

            // 章节标题编号格式化
            foreach (List list in document.Lists)
                list.DefaultFormatting();

            // 调整图片大小后，重新刷新全文布局
            document.UpdatePageLayout();

            // 目录仅显示到3级标题
            var field = document.Range.Fields.FirstOrDefault(f => f.Type == Aspose.Words.Fields.FieldType.FieldTOC);
            if (field != null && field is FieldToc toc)
            {
                toc.HeadingLevelRange = "1-3";
            }

            // 更新页码和目录
            document.UpdateFields();

            // 目录内容段落格式化（刷新域后可能造成格式变化）
            foreach (Paragraph paragraph in document.GetChildNodes(NodeType.Paragraph, true))
            {
                // 目录段落
                if (paragraph.GetText().Contains("PAGEREF _Toc"))
                {
                    //设置字体
                    var font = (paragraph.Runs.FirstOrDefault() as Run)?.Font;
                    if (font != null)
                    {
                        font.Default();
                        paragraph.SetupFont(font);
                    }

                    // 段落对齐
                    paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;

                    // 段落行距18磅（更美观，但因客户需求改为单倍行距）
                    //paragraph.ParagraphFormat.LineSpacing = 18;
                    //paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
                    // 段落单倍行距
                    paragraph.ParagraphFormat.LineSpacing = 12;
                    paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;

                    // 设置段前距、段后距
                    paragraph.ParagraphFormat.SpaceBefore = 0;
                    paragraph.ParagraphFormat.SpaceAfter = 0;

                    // 设置缩进
                    paragraph.ParagraphFormat.LeftIndent = 0;
                    paragraph.ParagraphFormat.RightIndent = 12 * 2;
                    // 首行缩进
                    paragraph.ParagraphFormat.FirstLineIndent = 0;
                }
            }

            // 状态页表格设置格式
            foreach (Table table in document.GetChildNodes(NodeType.Table, true))
            {
                var isTitleMatch = table.PreviousSibling is Paragraph pp && pp.GetText().Trim(new char[] { ' ', '\a', '\r', '\n' }).Replace(" ", "") == "状态页";
                var isFirstRowMatch = table.FirstRow?.FirstCell?.FirstParagraph?.GetText().Trim(new char[] { ' ', '\a', '\r', '\n' }).Replace(" ", "") == "状态页";

                // 状态页表格
                if (isTitleMatch || isFirstRowMatch)
                {
                    int rowIndex = 0;
                    int startIndex = isFirstRowMatch ? 2 : 1;
                    // 遍历表格中的所有行
                    foreach (Row row in table.Rows)
                    {
                        if (rowIndex++ < startIndex) continue;

                        if (row.Cells.Count > 0)
                        {
                            // 序号列段落
                            Paragraph paragraph = row.FirstCell.FirstParagraph;
                            if (paragraph == null) continue;

                            // 居中
                            paragraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;

                            // 段落单倍行距
                            paragraph.ParagraphFormat.LineSpacing = 12;
                            paragraph.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;

                            // 设置段前距、段后距
                            paragraph.ParagraphFormat.SpaceBefore = 0;
                            paragraph.ParagraphFormat.SpaceAfter = 0;

                            // 设置缩进
                            paragraph.ParagraphFormat.LeftIndent = 0;
                            paragraph.ParagraphFormat.RightIndent = 0;
                            paragraph.ParagraphFormat.FirstLineIndent = 0;

                            var listFont = paragraph.ListLabel?.Font;
                            if (listFont != null)
                            {
                                // 去掉序号后的间隔符，会影响居中显示
                                if (paragraph.ListFormat?.ListLevel != null)
                                    paragraph.ListFormat.ListLevel.TrailingCharacter = ListTrailingCharacter.Nothing;

                                listFont.TableDefault(FontSizeInPoints.小四); // 小四宋体
                                listFont.Name = "Times New Roman";
                            }
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// 刷新章节标题样式
        /// </summary>
        /// <param name="builder"></param>
        private static void FlushHeadingStyle(this Document document)
        {
            // 刷文档样式设置
            for (int i = 1; i < 8; i++)
            {
                StyleIdentifier styleIdentifier = StyleIdentifier.Normal;
                System.Enum.TryParse("Heading" + i, out styleIdentifier);
                if (styleIdentifier != StyleIdentifier.Normal)
                {
                    var style = document.Styles[styleIdentifier];

                    // 字体
                    style.Font.TitleDefault();

                    // 关联List中的样式设置
                    if (style.ListFormat?.ListLevel != null)
                    {
                        // 字体
                        style.ListFormat.ListLevel.Font.TitleDefault();
                        // 去除缩进
                        style.ListFormat.ListLevel.NumberPosition = 0;
                        style.ListFormat.ListLevel.TextPosition = 0;
                    }

                    // 前三级章节标题加粗
                    //style.Font.Bold = i < 4;
                    //style.Font.BoldBi = i < 4;
                }
            }

            // 刷文档现有序号设置（在上面Style中设置，暂时注释）
            // ！注：这里的序号包含所有域的序号，不只是章节序号。需要单独调整需要进一步识别
            //document.Lists.ForEach(ls =>
            //{
            //    ls.ListLevels.ForEach(lvl => lvl.Font.TitleDefault());
            //});
        }

        /// <summary>
        /// 获取Node所在页
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static (PageInfo? PageInfo, int PageIndex, RectangleF Rectangle)? GetLocatedPage(this Node node)
        {
            int pageIndex = 0;
            var document = node.Document as Document;
            // 对象Layout集合和索引器
            LayoutCollector layoutCollector = new LayoutCollector(document);
            LayoutEnumerator layoutEnumerator = new LayoutEnumerator(document);
            PageInfo? pageInfo = null;
            try
            {
                var shapeEntity = layoutCollector.GetEntity(node);
                if (shapeEntity == null) return null;

                layoutEnumerator.Current = shapeEntity;
                while (layoutEnumerator.MoveParent())
                {
                    if (layoutEnumerator.Type == LayoutEntityType.Page)
                        break;
                }
                pageIndex = layoutEnumerator.PageIndex;
                pageInfo = document.GetPageInfo(pageIndex - 1);

                return new(pageInfo, pageIndex, layoutEnumerator.Rectangle);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        /// <summary>
        /// 是否是附录标题
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        private static bool IsAppendixHeader(this Paragraph paragraph)
        {
            var listLable = (paragraph.ListLabel?.LabelString ?? "").Trim(new char[] { ' ', '\r', '\n' }).Replace(" ", "");
            return paragraph.ParagraphFormat.Style.Name == "标准文件_附录标识" || new Regex("^附录[A-Z]+$").IsMatch(listLable);
        }

        /// <summary>
        /// 段落是否为附录章节标题
        /// </summary>
        /// <param name="paragraph">段落</param>
        /// <param name="level">是否是level级标题，为空时仅判断是否是附录标题</param>
        /// <returns></returns>
        private static bool IsAppendixChapterHeading(this Paragraph paragraph, int? level = null)
        {
            var isAppendix = paragraph.ListFormat.List?.ListLevels.Count > 0
                             && new Regex("^附录[A-Z].").IsMatch(paragraph.ListFormat.List.ListLevels[0].NumberFormat);

            if (level != null) return isAppendix
                                      && paragraph.ListFormat.ListLevelNumber == (level - 1);
            else return isAppendix;
        }

        #endregion 文档格式化
    }

    /// <summary>
    /// 字体大小常量定义
    /// </summary>
    public static class FontSizeInPoints
    {
        public const double 初号 = 42;
        public const double 小初 = 36;
        public const double 一号 = 26;
        public const double 小一 = 24;
        public const double 二号 = 22;
        public const double 小二 = 18;
        public const double 三号 = 16;
        public const double 小三 = 15;
        public const double 四号 = 14;
        public const double 小四 = 12;
        public const double 五号 = 10.5;
        public const double 小五 = 9;
        public const double 六号 = 7.5;
        public const double 小六 = 6.5;
        public const double 七号 = 5.5;
        public const double 八号 = 5;
    }

    /// <summary>
    /// 文档格式化参数
    /// </summary>
    public class DocumentFormatSetting
    {
        /// <summary>
        /// 图像在文档中的高度限制，不能超过当前页高度的80%（默认值）
        /// </summary>
        public double ShapeZoomHeightLimit { get; set; } = 0.8;
    }
}