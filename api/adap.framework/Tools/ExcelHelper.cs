using highspeed.framework.Data;
using Aspose.Cells;
using Org.BouncyCastle.Bcpg;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;

namespace highspeed.framework.Common
{
	/// <summary>
	/// Excel帮助类
	/// </summary>
	public class ExcelHelper
	{
		private static string _TypeName;

		/// <summary>
		/// 构造函数
		/// </summary>
		static ExcelHelper()
		{
			_TypeName = typeof(ExcelHelper).FullName;
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
		}
		#region Load
		/// <summary>
		/// 读取Excel文件为DataSet
		/// </summary>
		/// <param name="file">Excel文件绝对路径</param>
		/// <param name="hasHeadRow">首行为标题行</param>
		/// <returns></returns>
		public static DataSet LoadExcel(string file, bool hasHeadRow = true)
		{
			if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
			{
				Logger.Error(_TypeName + ".LoadExcel", "No file found.");
				return null;
			}
			else
			{
				try
				{
					LoadOptions opt = new LoadOptions();
					opt.MemorySetting = MemorySetting.MemoryPreference;
					Workbook workbook = new Workbook(file, opt);
					DataSet ds = new DataSet();
					foreach (Worksheet sheet in workbook.Worksheets)
					{
						var dt = _LoadExcel(sheet, hasHeadRow);
						dt.TableName = sheet.Name;
						ds.Tables.Add(dt);
					}
					return ds;
				}
				catch (Exception ex)
				{
					Logger.Error(_TypeName + ".LoadExcel", "Encounter error when load file '" + file + "'.", ex);
					return null;
				}
			}
		}

		public static DataSet LoadExcel(string file, ExportTableOptions exportTableOptions)
		{
			if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
			{
				Logger.Error(_TypeName + ".LoadExcel", "No file found.");
				return null;
			}
			else
			{
				try
				{
					LoadOptions opt = new LoadOptions();
					opt.MemorySetting = MemorySetting.MemoryPreference;
					Workbook workbook = new Workbook(file, opt);
					DataSet ds = new DataSet();

					foreach (Worksheet sheet in workbook.Worksheets)
					{
						var dt = _LoadExcel(sheet, exportTableOptions);
						dt.TableName = sheet.Name;
						ds.Tables.Add(dt);
					}
					return ds;
				}
				catch (Exception ex)
				{
					Logger.Error(_TypeName + ".LoadExcel", "Encounter error when load file '" + file + "'.", ex);
					return null;
				}
			}
		}

		/// <summary>
		/// 读取Excel文件为DataSet
		/// </summary>
		/// <param name="stream">Excel文件流</param>
		/// <param name="hasHeadRow">首行为标题行</param>
		/// <returns></returns>
		public static DataSet LoadExcel(Stream stream, bool hasHeadRow = true)
		{
			try
			{
				LoadOptions opt = new LoadOptions();
				opt.MemorySetting = MemorySetting.MemoryPreference;
				Workbook workbook = new Workbook(stream, opt);
				DataSet ds = new DataSet();
				foreach (Worksheet sheet in workbook.Worksheets)
				{
					var dt = _LoadExcel(sheet, hasHeadRow);
					dt.TableName = sheet.Name;
					ds.Tables.Add(dt);
				}
				return ds;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".LoadExcel", "Encounter error when load steam.", ex);
				return null;
			}
		}

		public static Workbook GetWorkbook(Stream stream)
		{
			try
			{
				LoadOptions opt = new LoadOptions();
				opt.MemorySetting = MemorySetting.MemoryPreference;
				Workbook workbook = new Workbook(stream, opt);
				return workbook;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".GetWorkbook", "Encounter error when load steam.", ex);
				return null;
			}
		}

		public static Workbook GetWorkbook(string file)
		{
			if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
			{
				Logger.Error(_TypeName + ".GetWorkbook", "No file found.");
				return null;
			}
			try
			{
				LoadOptions opt = new LoadOptions();
				opt.MemorySetting = MemorySetting.MemoryPreference;
				Workbook workbook = new Workbook(file, opt);
				return workbook;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".GetWorkbook", "Encounter error when load steam.", ex);
				return null;
			}
		}

		/// <summary>
		/// 读取Excel文件为对象集合
		/// </summary>
		/// <param name="stream">Excel文件流</param>
		/// <param name="hasHeadRow">首行为标题行</param>
		/// <returns></returns>
		public static List<T> LoadExcel<T>(Workbook workbook)
		{
			var type = typeof(T);

			List<T> result = new List<T>();

			string? sheetName = null;
			int startRow = 1; // 默认从第一行开始读取数据
			var typeAttris = TypeDescriptor.GetAttributes(type);
			foreach (Attribute attri in typeAttris)
			{
				if (attri is ExcelSheet)
				{
					var excelSheet = attri as ExcelSheet;
					sheetName = excelSheet.Sheet;
					startRow = excelSheet.StartRow;
				}
			}

			if (string.IsNullOrEmpty(sheetName)) return result;
			if (!workbook.Worksheets.Any(s => s.Name == sheetName)) return result;

			// 获取列与字段Mapping
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
			// Key: 列Index，Value: 属性
			List<(string Column, int Index, PropertyInfo? Property)> colnums = new List<(string Column, int Index, PropertyInfo? Property)>();
			foreach (PropertyDescriptor prop in properties)
			{
				foreach (Attribute attri in prop.Attributes)
				{
					if (attri is ExcelColumn)
					{
						var col = (attri as ExcelColumn)?.Column;
						if (col == null) continue;
						colnums.Add(new(col, AlphaColunmToIndex(col), type.GetProperty(prop.Name)));
					}
				}
			}

			// 填充数据
			var cells = workbook.Worksheets[sheetName].Cells;
			int row_index = 1; string col_name = "";
			try
			{
				foreach (Row row in cells.Rows)
				{
					if (row_index++ < startRow) continue;

					T r = (T)type.Assembly.CreateInstance(type.FullName);
					foreach (var col in colnums)
					{
						col_name = col.Column;
						var cell = row[col.Index];
						// 处理非字符串类型保存到字符串类型属性中
						var value = col.Property?.PropertyType.FullName == typeof(String).FullName ? cell.Value?.ToString() : cell.Value;
						col.Property?.SetValue(r, value);
					}
					result.Add(r);
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"行 {row_index} 列 '{col_name}' 数据错误：{ex.Message}", ex);
			}

			return result;
		}

		/// <summary>
		/// Excel列标记转为Index。例，"A" => 0 , "C" => 2 , "AA" = 26
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		public static int AlphaColunmToIndex(string column)
		{
			int index = 0;
			foreach (char c in column.ToUpper())
			{
				index = index * 26 + (c - 'A' + 1);
			}
			return index - 1;
		}

		/// <summary>
		/// 读取Excel文件为DataTable
		/// </summary>
		/// <param name="file">Excel文件绝对路径</param>
		/// <param name="sheetIndex">Sheet Index</param>
		/// <param name="hasHeadRow">首行为标题行</param>
		/// <returns></returns>
		public static DataTable LoadExcel(string file, int sheetIndex, bool hasHeadRow = true)
		{
			if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
			{
				Logger.Error(_TypeName + ".LoadExcel", "No file found.");
				return null;
			}
			else
			{
				try
				{
					LoadOptions opt = new LoadOptions();
					opt.MemorySetting = MemorySetting.MemoryPreference;
					Workbook workbook = new Workbook(file, opt);
					return _LoadExcel(workbook.Worksheets[sheetIndex], hasHeadRow);
				}
				catch (Exception ex)
				{
					Logger.Error(_TypeName + ".LoadExcel", "Encounter error when load file '" + file + "'.", ex);
					return null;
				}
			}
		}

		/// <summary>
		/// 读取Excel文件为DataTable
		/// </summary>
		/// <param name="file">Excel文件绝对路径</param>
		/// <param name="sheetName">Sheet名</param>
		/// <param name="hasHeadRow">首行为标题行</param>
		/// <returns></returns>
		public static DataTable LoadExcel(string file, string sheetName, bool hasHeadRow = true)
		{
			if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
			{
				Logger.Error(_TypeName + ".LoadExcel", "No file found.");
				return null;
			}
			else
			{
				try
				{
					LoadOptions opt = new LoadOptions();
					opt.MemorySetting = MemorySetting.MemoryPreference;
					Workbook workbook = new Workbook(file, opt);
					return _LoadExcel(workbook.Worksheets[sheetName], hasHeadRow);
				}
				catch (Exception ex)
				{
					Logger.Error(_TypeName + ".LoadExcel", "Encounter error when load file '" + file + "'.", ex);
					return null;
				}
			}
		}

		private static DataTable _LoadExcel(Worksheet sheet, bool hasHeadRow = true)
		{
			if (sheet == null) return null;
			var cells = sheet.Cells;
			var exportTableOptions = new ExportTableOptions { CheckMixedValueType = false, ExportColumnName = true, ExportAsString = true };
			var detailTable = cells.ExportDataTable(0, 0, cells.MaxRow + 1, cells.MaxColumn + 1, exportTableOptions);
			return detailTable;
		}

		private static DataTable _LoadExcel(Worksheet sheet, ExportTableOptions exportTableOptions)
		{
			if (sheet == null) return null;
			Cells cells = sheet.Cells;
			return sheet.Cells.ExportDataTable(0, 0, cells.MaxDataRow + 1, cells.MaxColumn + 1, exportTableOptions);
		}
		#endregion

		public class DataPackage
		{
			public DataColumnCollection Columns;
			public DataRow[] Rows;
			public string SheetName;
			public ICollection<string> HideCols = null;
			public Dictionary<string, string> TransCols;
		}

		/// <summary>
		/// 保存Excel为Pdf
		/// </summary>
		/// <param name="excelFile">Excel文件路径</param>
		/// <param name="pdfFile">PDF文件路径</param>
		/// <returns></returns>
		public static bool ConverToPdf(string excelFile, string pdfFile)
		{
			try
			{
				FileInfo f1 = new FileInfo(excelFile);
				string ext1 = f1.Extension.ToLower().Substring(1);
				if (ext1 == "xlsx" || ext1 == "xls")
				{
					Workbook book1 = new Workbook(excelFile);
					book1.Save(pdfFile, SaveFormat.Pdf);
					book1.Dispose();
					book1 = null;
					GC.Collect();
				}
				else
				{
					Logger.Error(_TypeName + ".ConverToPdf", "Not an Excel file.");
					return false;
				}
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".ConverToPdf", "Conver to pdf failed.", ex);
				return false;
			}

			return true;
		}

		#region SaveToExcel DataColumnCollection columns, DataRow[] rows

		public static Style GetHeaderStyle(Workbook book)
		{
			Style style = book.CreateStyle();
			style.SetBorder(
				BorderType.LeftBorder | BorderType.TopBorder | BorderType.RightBorder | BorderType.BottomBorder,
				CellBorderType.Thin,
				Color.FromArgb(0x66, 0x66, 0x66));
			style.HorizontalAlignment = TextAlignmentType.Left;
			style.VerticalAlignment = TextAlignmentType.Center;
			style.Font.Name = "Calibri";
			style.Font.IsBold = true;
			style.Font.Size = 11;
			style.Font.Color = Color.FromArgb(0x33, 0x33, 0x33);
			style.ForegroundColor = Color.LightGray;
			style.Pattern = BackgroundType.Solid;
			//style.IsTextWrapped = true;
			return style;
		}

		public static Style GetCellStyle(Workbook book)
		{
			Style style = book.CreateStyle();
			style.SetBorder(
				BorderType.LeftBorder | BorderType.TopBorder | BorderType.RightBorder | BorderType.BottomBorder,
				CellBorderType.Thin,
				Color.FromArgb(0x66, 0x66, 0x66));
			style.HorizontalAlignment = TextAlignmentType.Left;
			style.VerticalAlignment = TextAlignmentType.Center;
			style.Font.Name = "Calibri";
			style.Font.IsBold = false;
			style.Font.Size = 11;
			style.Font.Color = Color.FromArgb(0x33, 0x33, 0x33);
			return style;
		}

		private static SaveFormat? GetSaveFormat(string file)
		{
			if (string.IsNullOrWhiteSpace(file)) return null;

			SaveFormat? format = null; // 返回空为默认Xls文件格式

			var lowerName = file.ToLower();
			if (lowerName.EndsWith(".xlsx")) format = SaveFormat.Xlsx; // 基于XML的通用Excel文件格式
			else if (lowerName.EndsWith(".xlsb")) format = SaveFormat.Xlsb; // 支持宏的二进制Excel文件格式
			else if (lowerName.EndsWith(".xlsm")) format = SaveFormat.Xlsm; // Xlsx + 宏支持
			else if (lowerName.EndsWith(".csv")) format = SaveFormat.Csv; // 基于分隔符的文本文件格式

			return format;
		}

		/// <summary>
		/// 保存DataTable到Excel，返回临时Workbook，供后续处理（不保存文件）
		/// </summary>
		/// <param name="book">文件</param>
		/// <param name="dt">数据源</param> 
		/// <param name="sheetName">Sheet页</param>
		/// <param name="hasHeader">是否有表头</param>
		/// <returns></returns>
		public static Workbook SaveDataTableToWorkbook(Workbook? book, DataTable? dt, string sheetName, bool hasHeader)
		{
			if (book == null)
			{
				book = new Workbook();
			}

			try
			{
				book.Settings.MemorySetting = MemorySetting.MemoryPreference;
				Worksheet sheet = book.Worksheets[sheetName];
				if (sheet == null)
				{
					sheet = book.Worksheets.Add(sheetName);
				}
				if (!string.IsNullOrWhiteSpace(sheetName))
				{
					sheet.Name = sheetName;
				}
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				//创建样式
				Style headerStyle = GetHeaderStyle(book);
				Style cellStyle = GetCellStyle(book);

				//生成表头
				if (hasHeader && dt != null)
				{
					for (int i = 0; i < dt.Columns.Count; i++)
					{
						cells[0, i].SetStyle(headerStyle);
						cells.SetColumnWidth(i, dt.Columns[i].ColumnName.Length * 2 + 1.5);
						cells.SetRowHeight(0, 30);
						cells[0, i].PutValue(dt.Columns[i].ColumnName);
					}
				}

				//生成数据行
				if (hasHeader && dt != null)
				{
					sheet.Cells.ImportDataView(dt.DefaultView, 1, 0);
				}
				else if (dt != null)
				{
					sheet.Cells.ImportDataView(dt.DefaultView, 0, 0);
				}

				if (dt != null)
				{
					for (int i = 0; i < dt.Columns.Count; i++)
					{
						for (int j = 0; j < dt.Rows.Count; j++)
						{
							if (hasHeader)
							{
								cells[j + 1, i].SetStyle(cellStyle);
							}
							else
							{
								cells[j, i].SetStyle(cellStyle);
							}
						}
					}
				}

				#region 设置忽略数字文本的校验错误
				if (dt != null)
				{
					 ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
					int optId = opts.Add();
					ErrorCheckOption opt = opts[optId];
					opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

					CellArea ca = new CellArea();
					ca.StartRow = 0;
					ca.StartColumn = 0;
					ca.EndRow = dt.Rows.Count;
					ca.EndColumn = cells.MaxColumn;
					opt.AddRange(ca);
				}
				#endregion

				sheet.AutoFitColumns(); //自适应宽 
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error("ExcelReport_Safety_Analysis.SaveToExcel", "Save datatable to excel failed.", ex);
				return book;
			}
		}

		/// <summary>
		/// 保存DataTable到Excel，返回临时Workbook，供后续处理（不保存文件）
		/// </summary>
		/// <param name="book">文件</param>
		/// <param name="dt">数据源</param> 
		/// <param name="sheetName">Sheet页</param>
		/// <param name="hasHeader">是否有表头</param>
		/// <returns></returns>
		public static Workbook SaveDataTablesToWorkbook(Workbook? book, List<DataTable> dts, List<string> sheetNames, bool hasHeader)
		{
			if (book == null)
			{
				book = new Workbook();
			}

			try
			{
				for (int p = 0; p < dts.Count; p++)
				{
					book.Settings.MemorySetting = MemorySetting.MemoryPreference;
					Worksheet sheet = book.Worksheets[sheetNames[p]];
					if (sheet == null && p == 0)
					{
						sheet = book.Worksheets[0];
					}
					else if (sheet == null)
					{
						book.Worksheets.Add();
						sheet = book.Worksheets[p];
					}
					if (!string.IsNullOrWhiteSpace(sheetNames[p]))
					{
						sheet.Name = sheetNames[p];
					}
					Cells cells = sheet.Cells;
					cells.MemorySetting = MemorySetting.MemoryPreference;

					//创建样式
					Style headerStyle = GetHeaderStyle(book);
					Style cellStyle = GetCellStyle(book);

					//生成表头
					if (hasHeader)
					{
						for (int i = 0; i < dts[p].Columns.Count; i++)
						{
							cells[0, i].SetStyle(headerStyle);
							cells.SetColumnWidth(i, dts[p].Columns[i].ColumnName.Length * 2 + 1.5);
							cells.SetRowHeight(0, 30);
							cells[0, i].PutValue(dts[p].Columns[i].ColumnName);
						}
					}

					//生成数据行
					if (hasHeader)
					{
						sheet.Cells.ImportDataView(dts[p].DefaultView, 1, 0);
					}
					else
					{
						sheet.Cells.ImportDataView(dts[p].DefaultView, 0, 0);
					}
					for (int i = 0; i < dts[p].Columns.Count; i++)
					{
						for (int j = 0; j < dts[p].Rows.Count; j++)
						{
							if (hasHeader)
							{
								cells[j + 1, i].SetStyle(cellStyle);
							}
							else
							{
								cells[j, i].SetStyle(cellStyle);
							}
						}
					}

					#region 设置忽略数字文本的校验错误
					ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
					int optId = opts.Add();
					ErrorCheckOption opt = opts[optId];
					opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

					CellArea ca = new CellArea();
					ca.StartRow = 0;
					ca.StartColumn = 0;
					ca.EndRow = dts[p].Rows.Count;
					ca.EndColumn = cells.MaxColumn;
					opt.AddRange(ca);
					#endregion

					sheet.AutoFitColumns(); //自适应宽  
				}
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error("ExcelReport_Safety_Analysis.SaveToExcel", "Save datatable to excel failed.", ex);
				return book;
			}
		}

		/// <summary>
		/// 保存DataTable为Excel文件
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="file">导出文件路径</param>
		/// <param name="sheetName">Sheet名称</param>
		/// <param name="isTemplated">是否为模板</param>
		/// <param name="hideCols">隐藏的列</param>
		/// <param name="isAppend">是否追加数据</param>
		/// <returns>成功与否</returns>
		public static bool SaveToExcel(DataTable data, string file, string sheetName = null, bool isTemplated = false, ICollection<string> hideCols = null, bool isAppend = false)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				Logger.Error(_TypeName + ".SaveToExcel", "File path is empty.");
				return false;
			}
			else
			{
				Workbook book = null;
				file = file.Replace("/", "\\");
				if (isTemplated)
				{
					FileHelper.CheckOrCreateFile(file, false);
					book = SaveToTemplatedExcel(book, data.Columns, data.Select(), file, sheetName, hideCols);
				}
				else
				{
					if (!FileHelper.CheckOrCreateFile(file, false) || !isAppend)
					{
						book = SaveToNewExcel(book, data.Columns, data.Select(), file, sheetName, hideCols);
					}
					else
					{
						book = SaveToExistExcel(book, data.Columns, data.Select(), file, sheetName, hideCols);
					}
				}
				if (book == null) return false;

				//保存
				var format = GetSaveFormat(file);
				if (format != null) book.Save(file, format.Value);
				else book.Save(file);

				book.Dispose();
				book = null;
				GC.Collect();
				return true;
			}
		}
		/// <summary>
		/// 保存DataTable为Excel文件
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="file">导出文件路径</param>
		/// <param name="isAppendSheet">是否追加Sheet页</param>
		/// <param name="sheetName">Sheet名称</param>
		/// <param name="isTemplated">是否为模板</param>
		/// <param name="hideCols">隐藏的列</param>
		/// <returns>成功与否</returns>
		public static bool SaveToExcel(DataTable data, string file, bool isAppendSheet, string sheetName = null, bool isTemplated = false, ICollection<string> hideCols = null)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				Logger.Error(_TypeName + ".SaveToExcel", "File path is empty.");
				return false;
			}
			else
			{
				Workbook book = null;
				file = file.Replace("/", "\\");
				if (isTemplated)
				{
					FileHelper.CheckOrCreateFile(file, false);
					book = SaveToTemplatedExcel(book, data.Columns, data.Select(), file, sheetName, hideCols);
				}
				else
				{
					if (!FileHelper.CheckOrCreateFile(file, false) || !isAppendSheet)
					{
						book = SaveToNewExcel(book, data.Columns, data.Select(), file, sheetName, hideCols);
					}
					else
					{
						book = SaveAppendSheetToExcel(book, data.Columns, data.Select(), file, sheetName, hideCols);
					}
				}
				if (book == null) return false;

				//保存
				var format = GetSaveFormat(file);
				if (format != null) book.Save(file, format.Value);
				else book.Save(file);

				book.Dispose();
				book = null;
				//防止导出数据量过大时，导致内存溢出
				GC.Collect();
				return true;
			}
		}
		/// <summary>
		/// 保存DataTable为Excel文件
		/// </summary>
		/// <param name="columns">列定义</param>
		/// <param name="rows">行集合</param>
		/// <param name="file">导出文件路径</param>
		/// <param name="sheetName">Sheet名称</param>
		/// <param name="isTemplated">是否为模板</param>
		/// <param name="hideCols">隐藏的列</param>
		/// <param name="transCols">转换的列</param>
		/// <returns>成功与否</returns>
		public static bool SaveToExcel(DataColumnCollection columns, DataRow[] rows, string file, string sheetName = null, bool isTemplated = false, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				Logger.Error(_TypeName + ".SaveToExcel", "File path is empty.");
				return false;
			}
			else
			{
				Workbook book = null;
				file = file.Replace("/", "\\");
				if (isTemplated)
				{
					FileHelper.CheckOrCreateFile(file, false);
					book = SaveToTemplatedExcel(book, columns, rows, file, sheetName, hideCols, transCols);
				}
				else
				{

					FileHelper.CheckOrCreateFile(file, false);
					book = SaveToNewExcel(book, columns, rows, file, sheetName, hideCols, transCols);

				}
				if (book == null) return false;

				//保存
				var format = GetSaveFormat(file);
				if (format != null) book.Save(file, format.Value);
				else book.Save(file);

				book.Dispose();
				book = null;
				GC.Collect();
				return true;
			}
		}

		/// <summary>
		/// 保存DataTable为Excel文件
		/// </summary>
		/// <param name="file">导出文件路径</param>
		/// <param name="dataPacks">数据</param>
		/// <param name="isTemplated">是否为模板</param>
		public static bool SaveToExcel(string file, IEnumerable<DataPackage> dataPacks, bool isTemplated = false)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				Logger.Error(_TypeName + ".SaveToExcel", "File path is empty.");
				return false;
			}
			else
			{
				Workbook book = null;
				file = file.Replace("/", "\\");
				if (isTemplated)
				{
					FileHelper.CheckOrCreateFile(file, false);
					foreach (var dp in dataPacks)
						book = SaveToTemplatedExcel(book, dp.Columns, dp.Rows, file, dp.SheetName, dp.HideCols, dp.TransCols);
				}
				else
				{
					FileHelper.CheckOrCreateFile(file, false);
					foreach (var dp in dataPacks)
						book = SaveToNewExcel(book, dp.Columns, dp.Rows, file, dp.SheetName, dp.HideCols, dp.TransCols);
				}
				if (book == null) return false;

				//保存
				var format = GetSaveFormat(file);
				if (format != null) book.Save(file, format.Value);
				else book.Save(file);

				book.Dispose();
				book = null;
				GC.Collect();
				return true;
			}
		}

		public static Workbook SaveToNewExcel(Workbook book, DataColumnCollection columns, DataRow[] rows, string file, string sheetName = null, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			try
			{
				if (book == null) book = new Workbook();
				book.Settings.MemorySetting = MemorySetting.MemoryPreference;
				Worksheet sheet = book.Worksheets[0];
				if (!string.IsNullOrWhiteSpace(sheetName)) sheet.Name = sheetName;
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				#region 创建样式
				Style headerStyle = GetHeaderStyle(book);
				Style cellStyle = GetCellStyle(book);
				#endregion

				//表格填充数据
				int Colnum = columns.Count;
				int Rownum = rows.Length;
				List<int> hideIndex = new List<int>();

				#region 生成表头
				for (int i = 0; i < Colnum; i++)
				{
					var col = columns[i].ColumnName;
					if (transCols != null && transCols.ContainsKey(col)) col = transCols[col];
					cells[0, i].PutValue(col);
					if (hideCols != null && !hideCols.Contains(col))
					{
						if (!hideIndex.Contains(i)) hideIndex.Add(i);
					}
					cells[0, i].SetStyle(headerStyle);
					cells.SetColumnWidth(i, col.Length * 2 + 1.5);
					cells.SetRowHeight(0, 30);
				}
				#endregion

				#region 生成数据行
				for (int i = 0; i < Rownum; i++)
				{
					for (int k = 0; k < Colnum; k++)
					{
						try
						{
							cells[1 + i, k].PutValue(rows[i][k].ToString()); //添加数据
						}
						catch (Exception ex)
						{
							Logger.Error("SaveToNewExcel", ex.ToString());
							cells[1 + i, k].PutValue(string.Empty); //添加空数据
						}
						if (!hideIndex.Contains(k)) cells[1 + i, k].SetStyle(cellStyle); //添加样式
					}
				}
				#endregion

				#region 设置忽略数字文本的校验错误
				ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
				int optId = opts.Add();
				ErrorCheckOption opt = opts[optId];
				opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

				CellArea ca = new CellArea();
				ca.StartRow = 0;
				ca.StartColumn = 0;
				ca.EndRow = Rownum;
				ca.EndColumn = cells.MaxColumn;
				opt.AddRange(ca);
				#endregion

				sheet.AutoFitColumns(); //自适应宽
				hideIndex.ForEach(i => cells.HideColumn(i));
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".SaveToExcel", "Save datatable to excel failed.", ex);
				return book;
			}
		}
		public static Workbook SaveToExistExcel(Workbook book, DataColumnCollection columns, DataRow[] rows, string file, string sheetName = null, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			try
			{
				if (book == null)
				{
					LoadOptions opt1 = new LoadOptions();
					opt1.MemorySetting = MemorySetting.MemoryPreference;
					book = new Workbook(file, opt1);
				}
				Worksheet sheet;
				if (!string.IsNullOrWhiteSpace(sheetName))
				{
					sheet = book.Worksheets[sheetName];
					if (sheet == null)
					{
						sheet = book.Worksheets[0];
						sheet.Name = sheetName;
					}
				}
				else
				{
					sheet = book.Worksheets[0];
				}
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				#region 创建样式
				Style headerStyle = GetHeaderStyle(book);
				Style cellStyle = GetCellStyle(book);
				#endregion

				//表格填充数据
				int Colnum = columns.Count;
				int Rownum = rows.Length;
				List<int> hideIndex = new List<int>();

				#region 生成表头
				if (cells.MaxDataRow <= 0)
					for (int i = 0; i < Colnum; i++)
					{
						var col = columns[i].ColumnName;
						if (transCols != null && transCols.ContainsKey(col)) col = transCols[col];
						cells[0, i].PutValue(col);
						if (hideCols != null && !hideCols.Contains(col))
						{
							if (!hideIndex.Contains(i)) hideIndex.Add(i);
						}
						cells[0, i].SetStyle(headerStyle);
						cells.SetColumnWidth(i, col.Length * 2 + 1.5);
						cells.SetRowHeight(0, 30);
					}
				#endregion

				#region 生成数据行
				var startRow = cells.MaxDataRow <= 0 ? 1 : cells.MaxDataRow + 1;
				for (int i = startRow; i < startRow + Rownum; i++)
				{
					for (int k = 0; k < Colnum; k++)
					{
						cells[i, k].PutValue(rows[i - startRow][k].ToString()); //添加数据
						if (!hideIndex.Contains(k)) cells[i, k].SetStyle(cellStyle); //添加样式
					}
				}
				#endregion

				#region 设置忽略数字文本的校验错误
				ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
				int optId = opts.Add();
				ErrorCheckOption opt = opts[optId];
				opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

				CellArea ca = new CellArea();
				ca.StartRow = 0;
				ca.StartColumn = 0;
				ca.EndRow = Rownum;
				ca.EndColumn = cells.MaxColumn;
				opt.AddRange(ca);
				#endregion

				sheet.AutoFitColumns(); //自适应宽
				hideIndex.ForEach(i => cells.HideColumn(i));
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".SaveToExcel", "Save datatable to excel failed.", ex);
				return book;
			}
		}

		/// <summary>
		/// 追加Sheet页
		/// </summary>
		/// <param name="book">Workbook</param>
		/// <param name="columns">列定义</param>
		/// <param name="rows">行集合</param>
		/// <param name="file">导出文件路径</param>
		/// <param name="sheetName">Sheet名称</param>
		/// <param name="hideCols">隐藏的列</param>
		/// <param name="transCols">转换的列</param>
		/// <returns></returns>
		public static Workbook SaveAppendSheetToExcel(Workbook book, DataColumnCollection columns, DataRow[] rows, string file, string sheetName = null, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			try
			{
				if (book == null)
				{
					LoadOptions opt1 = new LoadOptions();
					opt1.MemorySetting = MemorySetting.MemoryPreference;
					book = new Workbook(file, opt1);
				}
				Worksheet sheet;
				if (!string.IsNullOrWhiteSpace(sheetName))
				{
					sheet = book.Worksheets[sheetName];
					if (sheet == null)
					{
						sheet = book.Worksheets.Add(sheetName);
						sheet.Name = sheetName;
					}
				}
				else
				{
					sheet = book.Worksheets[0];
				}
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				#region 创建样式
				Style headerStyle = GetHeaderStyle(book);
				Style cellStyle = GetCellStyle(book);
				#endregion

				//表格填充数据
				int Colnum = columns.Count;
				int Rownum = rows.Length;
				List<int> hideIndex = new List<int>();

				#region 生成表头
				if (cells.MaxDataRow <= 0)
					for (int i = 0; i < Colnum; i++)
					{
						var col = columns[i].ColumnName;
						if (transCols != null && transCols.ContainsKey(col)) col = transCols[col];
						cells[0, i].PutValue(col);
						if (hideCols != null && !hideCols.Contains(col))
						{
							if (!hideIndex.Contains(i)) hideIndex.Add(i);
						}
						cells[0, i].SetStyle(headerStyle);
						cells.SetColumnWidth(i, col.Length * 2 + 1.5);
						cells.SetRowHeight(0, 30);
					}
				#endregion

				#region 生成数据行
				var startRow = cells.MaxDataRow <= 0 ? 1 : cells.MaxDataRow + 1;
				for (int i = startRow; i < startRow + Rownum; i++)
				{
					for (int k = 0; k < Colnum; k++)
					{
						cells[i, k].PutValue(rows[i - startRow][k].ToString()); //添加数据
						if (!hideIndex.Contains(k)) cells[i, k].SetStyle(cellStyle); //添加样式
					}
				}
				#endregion

				#region 设置忽略数字文本的校验错误
				ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
				int optId = opts.Add();
				ErrorCheckOption opt = opts[optId];
				opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

				CellArea ca = new CellArea();
				ca.StartRow = 0;
				ca.StartColumn = 0;
				ca.EndRow = Rownum;
				ca.EndColumn = cells.MaxColumn;
				opt.AddRange(ca);
				#endregion

				sheet.AutoFitColumns(); //自适应宽
				hideIndex.ForEach(i => cells.HideColumn(i));
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".SaveToExcel", "Save datatable to excel failed.", ex);
				return book;
			}
		}
		private static Workbook SaveToTemplatedExcel(Workbook book, DataColumnCollection columns, DataRow[] rows, string file, string sheetName = null, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			try
			{
				if (book == null)
				{
					LoadOptions opt1 = new LoadOptions();
					opt1.MemorySetting = MemorySetting.MemoryPreference;
					book = new Workbook(file, opt1);
				}
				Worksheet sheet = null;
				if (!string.IsNullOrWhiteSpace(sheetName))
				{
					sheet = book.Worksheets[sheetName];
					if (sheet == null)
					{
						sheet = book.Worksheets[0];
						sheet.Name = sheetName;
					}
				}
				else
				{
					sheet = book.Worksheets[0];
				}
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				#region 创建样式
				Style cellStyle = GetCellStyle(book);
				#endregion

				//表格填充数据
				int Colnum = columns.Count;
				List<string> cols = new List<string>();
				foreach (DataColumn c in columns)
				{
					cols.Add(c.ColumnName);
				}
				int Rownum = rows.Length;

				Dictionary<string, int> columnMapping = new Dictionary<string, int>();
				List<int> showIndex = new List<int>();
				List<int> hideIndex = new List<int>();
				#region 读取表头
				for (int i = 0; i <= cells.MaxColumn; i++)
				{
					var val = cells[0, i].Value;
					if (val != null)
					{
						var col = val.ToString();
						if (transCols != null)
						{
							var kv = transCols.FirstOrDefault(o => o.Value == col);
							if (!string.IsNullOrEmpty(kv.Key)) col = kv.Key;
						}

						if ((hideCols == null || !hideCols.Contains(col)) && !showIndex.Contains(i))
						{
							showIndex.Add(i);
						}
						else if (hideCols != null && hideCols.Contains(col) && !hideIndex.Contains(i))
						{
							hideIndex.Add(i);
						}

						if (cols.Contains(col))
						{
							if (!columnMapping.ContainsKey(col))
								columnMapping.Add(col, i);
						}
					}
				}
				#endregion

				#region 生成数据行
				for (int i = 0; i < Rownum; i++)
				{
					foreach (var col in cols)
					{
						if (columnMapping.ContainsKey(col))
						{
							var colIndex = columnMapping[col];
							cells[1 + i, colIndex].PutValue(rows[i][col].ToString()); //添加数据
						}
					}
				}
				#endregion

				#region 刷样式
				for (int i = 0; i < Rownum; i++)
				{
					for (int k = 0; k <= cells.MaxColumn; k++)
					{
						if (showIndex.Contains(k)) cells[1 + i, k].SetStyle(cellStyle); //添加样式
					}
				}
				#endregion


				#region 设置忽略数字文本的校验错误
				ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
				int optId = opts.Add();
				ErrorCheckOption opt = opts[optId];
				opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

				CellArea ca = new CellArea();
				ca.StartRow = 0;
				ca.StartColumn = 0;
				ca.EndRow = Rownum;
				ca.EndColumn = cells.MaxColumn;
				opt.AddRange(ca);
				#endregion

				sheet.AutoFitColumns(); //自适应宽
				hideIndex.ForEach(i => cells.HideColumn(i));
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".SaveToExcel", "Save datatable to excel failed.", ex);
				return book;
			}
		}
		public static void WriteRows(Worksheet sheet, DataTable dt, int startRow = 1)
		{
			Cells cells = sheet.Cells;
			cells.MemorySetting = MemorySetting.MemoryPreference;

			#region 生成数据行
			var Colnum = dt.Columns.Count;
			var rows = dt.Rows;
			for (int i = startRow; i < startRow + rows.Count; i++)
			{
				for (int k = 0; k < Colnum; k++)
				{
					cells[i, k].PutValue(rows[i - startRow][k].ToString()); //添加数据
				}
			}
			#endregion
		}

		#endregion

		#region SaveToExcel string[] columns, List<List<KeyValuePair<string, object>>> rows
		/// <summary>
		/// 保存DataTable为Excel文件
		/// </summary>
		/// <param name="columns">列名集合</param>
		/// <param name="rows">行数据集</param>
		/// <param name="file">导出文件路径</param>
		/// <param name="sheetName">Sheet名称</param>
		/// <param name="isTemplated">是否为模板</param>
		/// <param name="hideCols">隐藏的列</param>
		/// <param name="transCols">转换的列</param>
		/// <returns></returns>
		public static bool SaveToExcel(string[] columns, List<List<KeyValuePair<string, object>>> rows, string file, string sheetName = null, bool isTemplated = false, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			if (string.IsNullOrWhiteSpace(file))
			{
				Logger.Error(_TypeName + ".SaveToExcel", "File path is empty.");
				return false;
			}
			else
			{
				Workbook book = null;
				file = file.Replace("/", "\\");
				if (isTemplated)
				{
					FileHelper.CheckOrCreateFile(file, false);
					book = SaveToTemplatedExcel(columns, rows, file, sheetName, hideCols, transCols);
				}
				else
				{

					FileHelper.CheckOrCreateFile(file, false);
					book = SaveToNewExcel(columns, rows, file, sheetName, hideCols, transCols);

				}
				if (book == null) return false;

				//保存
				var format = GetSaveFormat(file);
				if (format != null) book.Save(file, format.Value);
				else book.Save(file);

				book.Dispose();
				book = null;
				GC.Collect();
				return true;
			}
		}

		private static Workbook SaveToNewExcel(string[] columns, List<List<KeyValuePair<string, object>>> rows, string file, string sheetName = null, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			try
			{
				Workbook book = new Workbook();
				book.Settings.MemorySetting = MemorySetting.MemoryPreference;
				Worksheet sheet = book.Worksheets[0];
				if (!string.IsNullOrWhiteSpace(sheetName)) sheet.Name = sheetName;
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				#region 创建样式
				Style headerStyle = GetHeaderStyle(book);
				Style cellStyle = GetCellStyle(book);
				#endregion

				//表格填充数据
				int Colnum = columns.Length;
				int Rownum = rows.Count;
				List<int> hideIndex = new List<int>();

				#region 生成表头
				for (int i = 0; i < Colnum; i++)
				{
					var col = columns[i];
					if (transCols != null && transCols.ContainsKey(col)) col = transCols[col];
					cells[0, i].PutValue(col);
					if (hideCols != null && !hideCols.Contains(col))
					{
						if (!hideIndex.Contains(i)) hideIndex.Add(i);
					}
					cells[0, i].SetStyle(headerStyle);
					cells.SetColumnWidth(i, col.Length * 2 + 1.5);
					cells.SetRowHeight(0, 30);
				}
				#endregion

				#region 生成数据行
				for (int i = 0; i < Rownum; i++)
				{
					for (int k = 0; k < Colnum; k++)
					{
						cells[1 + i, k].PutValue(rows[i][k].Value?.ToString()); //添加数据
						if (!hideIndex.Contains(k)) cells[1 + i, k].SetStyle(cellStyle); //添加样式
					}
				}
				#endregion

				#region 设置忽略数字文本的校验错误
				ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
				int optId = opts.Add();
				ErrorCheckOption opt = opts[optId];
				opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

				CellArea ca = new CellArea();
				ca.StartRow = 0;
				ca.StartColumn = 0;
				ca.EndRow = Rownum;
				ca.EndColumn = cells.MaxColumn;
				opt.AddRange(ca);
				#endregion

				sheet.AutoFitColumns(); //自适应宽
				hideIndex.ForEach(i => cells.HideColumn(i));
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".SaveToExcel", "Save datatable to excel failed.", ex);
				return null;
			}
		}
		private static Workbook SaveToExistExcel(string[] columns, List<List<KeyValuePair<string, object>>> rows, string file, string sheetName = null, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			try
			{
				LoadOptions opt1 = new LoadOptions();
				opt1.MemorySetting = MemorySetting.MemoryPreference;
				Workbook book = new Workbook(file, opt1);
				Worksheet sheet;
				if (!string.IsNullOrWhiteSpace(sheetName))
				{
					sheet = book.Worksheets[sheetName];
					if (sheet == null)
					{
						sheet = book.Worksheets[0];
						sheet.Name = sheetName;
					}
				}
				else
				{
					sheet = book.Worksheets[0];
				}
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				#region 创建样式
				Style headerStyle = GetHeaderStyle(book);
				Style cellStyle = GetCellStyle(book);
				#endregion

				//表格填充数据
				int Colnum = columns.Length;
				int Rownum = rows.Count;
				List<int> hideIndex = new List<int>();

				#region 生成表头
				for (int i = 0; i < Colnum; i++)
				{
					var col = columns[i];
					if (transCols != null && transCols.ContainsKey(col)) col = transCols[col];
					cells[0, i].PutValue(col);
					if (hideCols != null && !hideCols.Contains(col))
					{
						if (!hideIndex.Contains(i)) hideIndex.Add(i);
					}
					cells[0, i].SetStyle(headerStyle);
					cells.SetColumnWidth(i, col.Length * 2 + 1.5);
					cells.SetRowHeight(0, 30);
				}
				#endregion

				#region 生成数据行
				for (int i = 0; i < Rownum; i++)
				{
					for (int k = 0; k < Colnum; k++)
					{
						cells[1 + i, k].PutValue(rows[i][k].Value.ToString()); //添加数据
						if (!hideIndex.Contains(k)) cells[1 + i, k].SetStyle(cellStyle); //添加样式
					}
				}
				#endregion

				#region 设置忽略数字文本的校验错误
				ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
				int optId = opts.Add();
				ErrorCheckOption opt = opts[optId];
				opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

				CellArea ca = new CellArea();
				ca.StartRow = 0;
				ca.StartColumn = 0;
				ca.EndRow = Rownum;
				ca.EndColumn = cells.MaxColumn;
				opt.AddRange(ca);
				#endregion

				sheet.AutoFitColumns(); //自适应宽
				hideIndex.ForEach(i => cells.HideColumn(i));
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".SaveToExcel", "Save datatable to excel failed.", ex);
				return null;
			}
		}
		private static Workbook SaveToTemplatedExcel(string[] columns, List<List<KeyValuePair<string, object>>> rows, string file, string sheetName = null, ICollection<string> hideCols = null, Dictionary<string, string> transCols = null)
		{
			try
			{
				LoadOptions opt1 = new LoadOptions();
				opt1.MemorySetting = MemorySetting.MemoryPreference;
				Workbook book = new Workbook(file, opt1);
				Worksheet sheet = null;
				if (!string.IsNullOrWhiteSpace(sheetName))
				{
					sheet = book.Worksheets[sheetName];
					if (sheet == null)
					{
						sheet = book.Worksheets[0];
						sheet.Name = sheetName;
					}
				}
				else
				{
					sheet = book.Worksheets[0];
				}
				Cells cells = sheet.Cells;
				cells.MemorySetting = MemorySetting.MemoryPreference;

				#region 创建样式
				Style cellStyle = GetCellStyle(book);
				#endregion

				//表格填充数据
				int Colnum = columns.Length;
				List<string> cols = new List<string>();
				foreach (string c in columns)
				{
					cols.Add(c);
				}
				int Rownum = rows.Count;

				Dictionary<string, int> columnMapping = new Dictionary<string, int>();
				List<int> showIndex = new List<int>();
				List<int> hideIndex = new List<int>();
				#region 读取表头
				for (int i = 0; i <= cells.MaxColumn; i++)
				{
					var val = cells[0, i].Value;
					if (val != null)
					{
						var col = val.ToString();
						if (transCols != null)
						{
							var kv = transCols.FirstOrDefault(o => o.Value == col);
							if (!string.IsNullOrEmpty(kv.Key)) col = kv.Key;
						}

						if ((hideCols == null || !hideCols.Contains(col)) && !showIndex.Contains(i))
						{
							showIndex.Add(i);
						}
						else if (hideCols != null && hideCols.Contains(col) && !hideIndex.Contains(i))
						{
							hideIndex.Add(i);
						}

						if (cols.Contains(col))
						{
							if (!columnMapping.ContainsKey(col))
								columnMapping.Add(col, i);
						}
					}
				}
				#endregion

				#region 生成数据行
				for (int i = 0; i < Rownum; i++)
				{
					foreach (var col in cols)
					{
						if (columnMapping.ContainsKey(col))
						{
							var colIndex = columnMapping[col];
							cells[1 + i, colIndex].PutValue(rows[i].FirstOrDefault(o => o.Key == col).Value?.ToString()); //添加数据
						}
					}
				}
				#endregion

				#region 刷样式
				for (int i = 0; i < Rownum; i++)
				{
					for (int k = 0; k <= cells.MaxColumn; k++)
					{
						if (showIndex.Contains(k)) cells[1 + i, k].SetStyle(cellStyle); //添加样式
					}
				}
				#endregion


				#region 设置忽略数字文本的校验错误
				ErrorCheckOptionCollection opts = sheet.ErrorCheckOptions;
				int optId = opts.Add();
				ErrorCheckOption opt = opts[optId];
				opt.SetErrorCheck(ErrorCheckType.TextNumber, false);

				CellArea ca = new CellArea();
				ca.StartRow = 0;
				ca.StartColumn = 0;
				ca.EndRow = Rownum;
				ca.EndColumn = cells.MaxColumn;
				opt.AddRange(ca);
				#endregion

				sheet.AutoFitColumns(); //自适应宽
				hideIndex.ForEach(i => cells.HideColumn(i));
				return book;
			}
			catch (Exception ex)
			{
				Logger.Error(_TypeName + ".SaveToExcel", "Save datatable to excel failed.", ex);
				return null;
			}
		}
		#endregion

		#region LightCellsDataProvider
		internal class LightCellsProvider : LightCellsDataProvider
		{
			private int _row = -1;
			private int _column = -1;

			private int maxRows;
			private int maxColumns;

			private Workbook _workbook;
			public LightCellsProvider(Workbook workbook, int maxRows, int maxColumns)
			{
				this._workbook = workbook;
				this.maxRows = maxRows;
				this.maxColumns = maxColumns;
			}

			#region LightCellsDataProvider Members
			public bool IsGatherString()
			{
				return false;
			}

			public int NextCell()
			{
				++_column;
				if (_column < this.maxColumns)
					return _column;
				else
				{
					_column = -1;
					return -1;
				}
			}
			public int NextRow()
			{
				++_row;
				if (_row < this.maxRows)
				{
					_column = -1;
					return _row;
				}
				else
					return -1;
			}

			public void StartCell(Cell cell)
			{
				cell.PutValue(_row + _column);
				if (_row == 1)
				{
				}
				else
				{
					cell.Formula = "=Rand() + A2";
				}
			}

			public void StartRow(Row row)
			{
			}

			public bool StartSheet(int sheetIndex)
			{
				if (sheetIndex == 0)
				{
					return true;
				}
				else
					return false;
			}
			#endregion
		}
		#endregion

		#region Save to stream
		public static Stream SaveToStream(DataSet dataSet)
		{
			Workbook book = null;
			var index = 0;
			foreach (DataTable data in dataSet.Tables)
			{
				if (index == 0)
				{
					book = SaveToNewExcel(book, data.Columns, data.Select(), null, data.TableName);
					index = 1;
				}
				else
				{
					book = SaveAppendSheetToExcel(book, data.Columns, data.Select(), null, data.TableName);
				}
			}
			if (book == null) return null;
			return book.SaveToStream();
		}
		#endregion
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ExcelColumn : Attribute
	{
		public ExcelColumn(string column) { Column = column; }
		public string Column { get; set; }
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExcelSheet : Attribute
	{
		public string Sheet { get; set; }

		// 行号从1开始
		public int StartRow { get; set; } = 1;
	}
}
