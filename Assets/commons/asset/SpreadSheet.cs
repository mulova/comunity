//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Reflection;

namespace commons
{
	public class SpreadSheet
	{
        public static Loggerx log = LogManager.GetLogger(typeof(SpreadSheet));
		protected List<Sheet> sheets = new List<Sheet>();
		private int sheetNo = 0;
		private int rowNo = -1;
		private int colNo = -1;
		public bool trimSpace = false;
		public bool allowNullRow = false;
		private static Cell NULL_CELL = new Cell("");
        private Encoding encoding;

		private Func<string, string> preprocessor = (src) =>
		{
			return src.Replace("\\n", "\n");
		};

		private Sheet Sheet
		{
			get { return sheets[sheetNo]; }
		}

		public SpreadSheet(byte[] bytes, SpreadSheetSourceType srcType = SpreadSheetSourceType.CSV) : this(bytes, Encoding.UTF8, srcType)
		{
		}

		public SpreadSheet(byte[] bytes, Encoding encoding, SpreadSheetSourceType srcType = SpreadSheetSourceType.CSV)
		{
			SpreadSheetParser parser = null;
			if (srcType == SpreadSheetSourceType.CSV)
			{
				parser = new SpreadSheetCsvParser(true);
			} else if (srcType == SpreadSheetSourceType.XML)
			{
				parser = new SpreadSheetXMLParser();
			}
            this.encoding = encoding;
			parser.preprocessor = preprocessor;
			sheets.AddRange(parser.ParseSheet(bytes, encoding));
			SetSheet(0);
		}

		public virtual void AddSheet(string sheetName, byte[] bytes)
		{
			SpreadSheetCsvParser parser = new SpreadSheetCsvParser(true);
			Row[] rows = parser.Parse(bytes, encoding).ToArray();
			Sheet s = new Sheet(sheetName, rows);
			sheets.Add(s);
		}

		public virtual void AddSheet(string sheetName, string content)
		{
			SpreadSheetCsvParser parser = new SpreadSheetCsvParser(true);
			Row[] rows = parser.Parse(content).ToArray();
			Sheet s = new Sheet(sheetName, rows);
			sheets.Add(s);
		}

		public Sheet GetSheet(int index)
		{
			if (index > sheets.Count-1)
				return null;
			return sheets[index];
		}

		public int GetSheetCount()
		{
			return sheets.Count;		
		}

		public Sheet GetSheet()
		{
			return sheets[sheetNo];
		}

		public Sheet GetNextSheet()
		{
			sheetNo++;
			return sheets[sheetNo];
		}

		public bool HasNextRow()
		{
			return HasRow(rowNo+1);
		}

		public bool HasRow(int rowNumber)
		{
			return Sheet.GetRow(rowNumber) != null;
		}

		public Row GetRow(int rowNumber)
		{
			return Sheet.GetRow(rowNumber);
		}

		public Dictionary<K, R> GetRowIndexer<K, R>(Converter<R, K> conv, int beginRow, int size = -1) where R:class, new()
		{
			List<R> rows = GetRows<R>(beginRow, size);
			return rows.ToDictionary(conv);
		}

		public Dictionary<K, R> GetRowIndexer<K, R>(RowParser<R> parser, Converter<R, K> conv, int beginRow, int size = -1) where R:class, new()
		{
			List<R> rows = GetRows<R>(parser, beginRow, size);
			return rows.ToDictionary(conv);
		}

		private Dictionary<FieldInfo, A> GetAttributeMap<A>(Type t, BindingFlags flags) where A:Attribute
		{
			Dictionary<FieldInfo, A> fields = new Dictionary<FieldInfo, A>();
			foreach (FieldInfo f in t.GetFields(flags))
			{
                A attr = ReflectionUtil.GetAttribute<A>(f);
				if (attr != null)
				{
					fields[f] = attr;
				}
			}
			return fields;
		}

		/// <summary>
		/// Converts the rows to instance of class T. class 'T' fields must have ExcelColumnAttribute
		/// </summary>
		/// <returns>The rows.</returns>
		/// <param name="beginRow">Begin row no. 0-based. inclusive</param>
		/// <param name="size">row size to read. if -1, read unitil the empty row is found</param>
		/// <typeparam name="R">The 1st type parameter.</typeparam>
		public List<R> GetRows<R>(int beginRow, int size = -1) where R:class, new()
		{
			BindingFlags flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.FlattenHierarchy;
			Type t = typeof(R);
			Dictionary<FieldInfo, SpreadSheetColumnAttribute> columnAttrs = GetAttributeMap<SpreadSheetColumnAttribute>(t, flags);

			List<R> rows = new List<R>();
			int r = beginRow;
			bool nullRow = false;
			while (r < Sheet.rows.Length&&((size < 0&&(!nullRow||allowNullRow))||r < beginRow+size))
			{
				R row = new R();
				nullRow = true;
				foreach (KeyValuePair<FieldInfo, SpreadSheetColumnAttribute> entry in columnAttrs)
				{
					SpreadSheetColumnAttribute col = entry.Value;
					string s = GetString(r, col.columnIndex);
					// pre filter
					if (s.IsNotEmpty())
					{
						if (trimSpace)
						{
							s = s.Trim();
						}
						if (col.columnType == SSColumnType.LowerCase)
						{
							s = s.ToLower();
						} else if (col.columnType == SSColumnType.UpperCase)
						{
							s = s.ToUpper();
						}
					}
					FieldInfo f = entry.Key;
					nullRow &= s.IsEmpty();
					if (f.FieldType == typeof(string))
					{
						string val = s.IsEmpty()? col.defaultValue as string : s;
						f.SetValue(row, val);
					} else if (f.FieldType.IsEnum)
					{
						if (col.defaultValue != null)
						{
							f.SetValue(row, GetEnum(f.FieldType, r, col.columnIndex, col.defaultValue));
						} else
						{
							f.SetValue(row, GetEnum(f.FieldType, r, col.columnIndex));
						}
					} else if (f.FieldType == typeof(int))
					{
						if (col.defaultValue != null&&s.IsEmpty())
						{
							f.SetValue(row, (int)col.defaultValue);
						} else
						{
							f.SetValue(row, GetInt(r, col.columnIndex));
						}
					} else if (f.FieldType == typeof(long))
					{
						if (col.defaultValue != null&&s.IsEmpty())
						{
							f.SetValue(row, (long)col.defaultValue);
						} else
						{
							f.SetValue(row, GetLong(r, col.columnIndex));
						}
					} else if (f.FieldType == typeof(bool))
					{
						if (col.defaultValue != null&&s.IsEmpty())
						{
							f.SetValue(row, (bool)col.defaultValue);
						} else
						{
							f.SetValue(row, GetBool(r, col.columnIndex));
						}
					} else if (f.FieldType == typeof(double))
					{
						if (col.defaultValue != null&&s.IsEmpty())
						{
							f.SetValue(row, (double)col.defaultValue);
						} else
						{
							f.SetValue(row, GetDouble(r, col.columnIndex));
						}
					} else if (f.FieldType == typeof(float))
					{
						if (col.defaultValue != null&&s.IsEmpty())
						{
							f.SetValue(row, (float)col.defaultValue);
						} else
						{
							f.SetValue(row, GetFloat(r, col.columnIndex));
						}
					}
				}
				if (!nullRow)
				{
					rows.Add(row);
				}
				++r;
			}
			return rows;
		}

		public delegate T RowParser<T>(Row r);

		/// <summary>
		/// Gets the rows.
		/// </summary>
		/// <returns>The rows.</returns>
		/// <param name="parser">Parser. returns null if end of the row meets</param>
		/// <param name="beginRow">Begin row.</param>
		/// <param name="size">Size.</param>
		/// <typeparam name="R">The 1st type parameter.</typeparam>
		public List<R> GetRows<R>(RowParser<R> parser, int beginRow, int size = -1) where R:class
		{
			List<R> rows = new List<R>();
			int r = beginRow;
			bool nullRow = false;
			while ((size < 0&&!nullRow)||r < beginRow+size)
			{
				Row raw = Sheet.GetRow(r);
				R row = default(R);
				if (raw != null)
				{
					row = parser(raw);
				}
				if (row != null)
				{
					rows.Add(row);
				} else
				{
					nullRow = true;
				}
				++r;
			}
			return rows;
		}

		private Cell GetCell(int rowNo, int colNo)
		{
			this.rowNo = rowNo;
			this.colNo = colNo;
			Row row = GetRow(rowNo);
			Cell cell = row.GetCell(colNo);
			return cell;
		}

		private Cell GetCellOrNull(int rowNo, int colNo)
		{
			this.rowNo = rowNo;
			this.colNo = colNo;
			Row r = Sheet.GetRow(rowNo);
			if (r == null)
			{
				return NULL_CELL;
			}
			Cell cell = r.GetCell(colNo);
			if (cell == null)
			{
				return NULL_CELL;
			}
			return cell;
		}

		public void NextCell()
		{
			colNo++;
		}

		public void NextRow()
		{
			rowNo++;
			colNo = -1;
		}

		public void SetSheet(int index)
		{
			this.sheetNo = index;
		}

		public void NextSheet()
		{
			sheetNo++;
		}

		public void SetSheet(string name)
		{
			for (int i = 0; i < sheets.Count; ++i)
			{
				Sheet s = sheets[i];
				if (s.name == name)
				{
					sheetNo = i;
					return;
				}
			}
			log.Error(name);
		}

		/**
		 * @return index of column s such as 'AA', 'BC'
		 */
		private int GetColumnIndex(string col)
		{
			int index = col[0]-'A';
			for (int i = 1; i < col.Length; i++)
			{
				index *= 26;
				index += col[i]-'A';
			}
			return index;
		}

		public double[][] GetDoubleArray1(int rowNo, char colNo, int rowSize, int colSize)
		{
			return GetDoubleArray(rowNo-1, colNo-'A', rowSize, colSize);
		}

		public double[][] GetDoubleArray1(int rowNo, string col, int rowSize, int colSize)
		{
			return GetDoubleArray(rowNo-1, GetColumnIndex(col), rowSize, colSize);
		}

		private double[][] GetDoubleArray(int rowNo, int colNo, int rowSize, int colSize)
		{
            double[][] ret = ArrayUtil.createDoubleArray(rowSize, colSize);
			return GetDoubleArray(rowNo, colNo, ret);
		}

		public int[][] GetIntArray1(int rowNo, char colNo, int rowSize, int colSize)
		{
			return GetIntArray(rowNo-1, colNo-'A', rowSize, colSize);
		}

		public int[][] GetIntArray1(int rowNo, string col, int rowSize, int colSize)
		{
			return GetIntArray(rowNo-1, GetColumnIndex(col), rowSize, colSize);
		}

		private int[][] GetIntArray(int rowNo, int colNo, int rowSize, int colSize)
		{
			double[][] ret = ArrayUtil.createDoubleArray(rowSize, colSize);
			return ArrayUtil.ToInt(GetDoubleArray(rowNo, colNo, ret));
		}

		public float[][] GetFloatArray1(int rowNo, char colNo, int rowSize, int colSize)
		{
			return GetFloatArray(rowNo-1, colNo-'A', rowSize, colSize);
		}

		public float[][] GetFloatArray1(int rowNo, string col, int rowSize, int colSize)
		{
			return GetFloatArray(rowNo-1, GetColumnIndex(col), rowSize, colSize);
		}

		private float[][] GetFloatArray(int rowNo, int colNo, int rowSize, int colSize)
		{
			double[][] arr = ArrayUtil.createDoubleArray(rowSize, colSize);
			return ArrayUtil.ToFloat(GetDoubleArray(rowNo, colNo, arr));
		}

		public float[] GetFloatArray1(int rowNo, char colNo, int colSize)
		{
			return GetFloatArray(rowNo-1, colNo-'A', colSize);
		}

		public float[] GetFloatArray1(int rowNo, string col, int colSize)
		{
			return GetFloatArray(rowNo-1, GetColumnIndex(col), colSize);
		}

		/**
		 * get 1-dimensional array
		 * @param rowNo
		 * @param colNo
		 * @param rowCount
		 * @return
		 */
		private float[] GetFloatArray(int rowNo, int colNo, int colSize)
		{
			double[][] arr = ArrayUtil.createDoubleArray(1, colSize);
			return ArrayUtil.ToFloat(GetDoubleArray(rowNo, colNo, arr)[0]);
		}

		public double[][] GetDoubleArray1(int rowNo, char colNo, double[][] store)
		{
			return GetDoubleArray(rowNo-1, colNo-'A', store);
		}

		public double[][] GetDoubleArray1(int rowNo, string col, double[][] store)
		{
			return GetDoubleArray(rowNo-1, GetColumnIndex(col), store);
		}

		private double[][] GetDoubleArray(int rowNo, int colNo, double[][] store)
		{
			int rowSize = store.Length;
			int colSize = store[0].Length;
			for (int r = 0; r < rowSize; r++)
			{
				Row row = Sheet.GetRow(r+rowNo);
				for (int c = 0; c < colSize; c++)
				{
					if (row != null)
					{
						Cell cell = row.GetCell(c+colNo);
						if (cell != null)
						{
							store[r][c] = cell.GetNumericCellValue();
						}
					}
				}
			}
			return store;
		}

		public string GetString(int row, int col)
		{
			Cell cell = GetCellOrNull(row, col);
			if (cell == null)
			{
				return null;
			}
			return cell.str;
		}

		public string GetString1(int row, string col)
		{
			return GetString(row-1, GetColumnIndex(col));
		}

		public string GetString1(int row, char col)
		{
			return GetString(row-1, col-'A');
		}

		public string GetNextCellString()
		{
			NextCell();
			return GetString(rowNo, colNo);
		}

		public string GetNextRowString()
		{
			NextRow();
			return GetString(rowNo, colNo);
		}

		public bool HasCellValue1(int row, char col)
		{
			return !string.IsNullOrEmpty(GetString1(row, col).Trim());
		}

		public bool HasCellValue1(int row, string col)
		{
			return !string.IsNullOrEmpty(GetString1(row, col).Trim());
		}

		private bool GetBool(int row, int col)
		{
			Cell cell = GetCellOrNull(row, col);
			if (cell == null)
			{
				return false;
			}
			return cell.GetBooleanCellValue();
		}

		public bool GetBool1(int row, string col)
		{
			return GetBool(row-1, GetColumnIndex(col));
		}

		public bool GetBool1(int row, char col)
		{
			return GetBool(row-1, col-'A');
		}

		public bool GetNextCellBool()
		{
			NextCell();
			return GetBool(rowNo, colNo);
		}

		public bool GetNextRowBool()
		{
			NextRow();
			return GetBool(rowNo, colNo);
		}

		private double GetDouble(int row, int col)
		{
			Cell cell = GetCellOrNull(row, col);
			return cell.GetNumericCellValue();
		}

		public double GetDouble1(int row, char col)
		{
			return GetDouble(row-1, col-'A');
		}

		public double GetDouble1(int row, string col)
		{
			return GetDouble(row-1, GetColumnIndex(col));
		}

		public double getNextCellDouble()
		{
			NextCell();
			return GetDouble(rowNo, colNo);
		}

		public double GetNextRowDouble()
		{
			NextRow();
			return GetDouble(rowNo, colNo);
		}

		private float GetFloat(int row, int col)
		{
			double d = GetDouble(row, col);
			if (double.IsNaN(d))
			{
				return 0F;
			}
			return (float)d;
		}

		public float GetFloat1(int row, char col)
		{
			return GetFloat(row-1, col-'A');
		}

		public float GetFloat1(int row, string col)
		{
			return GetFloat(row-1, GetColumnIndex(col));
		}

		public float GetNextCellFloat()
		{
			NextCell();
			return GetFloat(rowNo, colNo);
		}

		public float GetNextRowFloat()
		{
			NextRow();
			return GetFloat(rowNo, colNo);
		}

		private int GetInt(int row, int col)
		{
			double d = GetDouble(row, col);
			if (double.IsNaN(d))
			{
				return 0;
			}
			return (int)d;
		}

		public int GetInt1(int row, char col)
		{
			return GetInt(row-1, col-'A');
		}

		public int GetInt1(int row, string col)
		{
			return GetInt(row-1, GetColumnIndex(col));
		}

		public int GetNextCellInt()
		{
			NextCell();
			return GetInt(rowNo, colNo);
		}

		public int GetNextRowInt()
		{
			NextRow();
			return GetInt(rowNo, colNo);
		}

		public long GetLong(int row, int col)
		{
			double d = GetDouble(row, col);
			if (double.IsNaN(d))
			{
				return 0;
			}
			return (long)d;
		}

		public long GetLong1(int row, char col)
		{
			return GetLong(row-1, col-'A');
		}

		public long GetLong1(int row, string col)
		{
			return GetInt(row-1, GetColumnIndex(col));
		}

		public long GetNextCellLong()
		{
			NextCell();
			return GetLong(rowNo, colNo);
		}

		public long GetNextRowLong()
		{
			NextRow();
			return GetLong(rowNo, colNo);
		}

		private T GetEnum<T>(int row, int col) where T: struct, IComparable, IConvertible, IFormattable
		{
			return (T)Enum.Parse(typeof(T), GetString(row, col));
		}

		public T GetEnum1<T>(int row, char col) where T: struct, IComparable, IConvertible, IFormattable
		{
			return GetEnum<T>(row-1, col-'A');
		}

		public T GetEnum1<T>(int row, string col) where T: struct, IComparable, IConvertible, IFormattable
		{
			return GetEnum<T>(row-1, GetColumnIndex(col));
		}

		private Enum GetEnum(Type type, int row, int col)
		{
			return (Enum)EnumUtil.Parse(type, GetString(row, col));
		}

		private Enum GetEnum(Type type, int row, int col, object defaultValue)
		{
			return (Enum)EnumUtil.Parse(type, GetString(row, col), defaultValue);
		}

		public Enum GetEnum1(Type type, int row, char col)
		{
			return GetEnum(type, row-1, col-'A');
		}

		public Enum GetEnum1(Type type, int row, string col)
		{
			return GetEnum(type, row-1, GetColumnIndex(col));
		}

		public T GetNextCellEnum<T>() where T: struct, IComparable, IConvertible, IFormattable
		{
			NextCell();
			return GetEnum<T>(rowNo, colNo);
		}

		public T GetNextRowEnum<T>() where T: struct, IComparable, IConvertible, IFormattable
		{
			NextRow();
			return GetEnum<T>(rowNo, colNo);
		}

		/**
		 * 지정된 column 의 값이 str인 row number를 찾는다.  
		 * @param col
		 * @param str
		 * @return rowNumber 찾지 못하면 -1을 반환한다.
		 * @author mulova
		 */
		public int FindRow(int col, string str, int beginRow = 0)
		{
			return Sheet.FindRow(col, str);
		}

		public int FindRow1(string col, string str, int beginRow = 1)
		{
			int row = FindRow(GetColumnIndex(col), str, beginRow-1);
			if (row >= 0)
			{
				return row+1;
			}
			return row;
		}

		public int FindRow1(char col, string str, int beginRow = 1)
		{
			int row = FindRow(col-'A', str, beginRow-1);
			if (row >= 0)
			{
				return row+1;
			}
			return row;
		}

		/**
		 * 지정된 row의 column값이 str인 column number를 찾는다.
		 * @param rowNo
		 * @param str
		 * @return columnNumber 찾지 못하면 -1을 반환한다.
		 * @author mulova
		 */
		public int FindColumn(int rowNo, string str)
		{
			return Sheet.FindColumn(rowNo, str);
		}

		/**
		 * @param row
		 * @return row의 마지막 column을 반환한다.
		 * @author mulova
		 */
		public int GetLastColumn(int row)
		{
			Row r = Sheet.GetRow(row);
			return r.GetLastCellNum();
		}

		public int FindInt(int titleCol, string title, char valueCol)
		{
			int statRowNo = FindRow(titleCol, title, 50);
			if (statRowNo < 0)
			{
				log.Error("Can't find {0}", titleCol);
			}
			return GetInt(statRowNo+1, valueCol);
		}

		public int FindInt(char titleCol, string title, char valueCol)
		{
			return FindInt(titleCol-'A', title, valueCol);
		}

		public int FindInt(string titleCol, string title, char valueCol)
		{
			return FindInt(GetColumnIndex(titleCol), title, valueCol);
		}

		
		/**
		 * string - value table을 Map으로 만들어 반환한다.
		 * @param row
		 * @param col
		 * @param size
		 * @return
		 */
		public Dictionary<string, int> getMappedInt(int row, int c, int size)
		{
			int r = row-1;
			Dictionary<string, int> map = new Dictionary<string, int>();
			for (int i = 0; i < size; i++)
			{
				map[GetString(r, c+i).Trim()] = GetInt(r+1, c+i);
			}
			return map;
		}

		public Dictionary<string, int> getMappedInt(int row, char col, int size)
		{
			return getMappedInt(row, col-'A', size);
		}

		public Dictionary<string, int> getMappedInt(int row, string col, int size)
		{
			return getMappedInt(row, GetColumnIndex(col), size);
		}

		public void SetPreprocessor(Func<string, string> preprocessor)
		{
			this.preprocessor = preprocessor;
		}
	}

	public class Sheet
	{
		public string name;
		public Row[] rows = new Row[]{ };
		private Dictionary<string, int> rowIndexer;
		private Dictionary<string, int> colIndexer;
		private int indexCol;
		private int indexRow;

		public Sheet(string name)
		{
			this.name = name;
		}

		public Sheet(string name, Row[] rows)
		{
			this.name = name;
			this.rows = rows;
		}

		public Row GetRow(int rowNo)
		{
			if (rowNo < rows.Length)
			{
				return rows[rowNo];
			}
			return null;
		}

		public int GetFirstRowNum()
		{
			return 0;
		}

		public int GetLastRowNum()
		{
			return rows.Length-1;
		}

		public void AddRow(Row row)
		{
			Array.Resize(ref rows, rows.Length+1);
			rows[rows.Length-1] = row;
		}

		public int FindRow(int column, string key)
		{
			if (rowIndexer == null||column != indexCol)
			{
				this.indexCol = column;
				rowIndexer = new Dictionary<string, int>();
				for (int i = 0; i < rows.Length; ++i)
				{
					Row r = rows[i];
					Cell c = r.GetCell(column);
					if (c.GetString().IsNotEmpty())
					{
						rowIndexer[c.GetString()] = i;
					}
				}
			}
			if (rowIndexer.ContainsKey(key))
			{
				return rowIndexer.Get(key);
			}
			return -1;
		}

		public int FindColumn(int row, string key)
		{
			if (colIndexer == null||row != indexRow)
			{
				this.indexRow = row;
				colIndexer = new Dictionary<string, int>();
				Row r = rows[row];
				for (int i = 0; i < r.cells.Length; ++i)
				{
					Cell c = r.GetCell(i);
					if (c.GetString().IsNotEmpty())
					{
						colIndexer[c.GetString()] = i;
					}
				}
			}
			if (colIndexer.ContainsKey(key))
			{
				return colIndexer.Get(key);
			}
			return -1;
		}

	}

	public class Row
	{
		public Cell[] cells;
		private static Cell NULL_CELL = new Cell("");

		public Row(Cell[] cells)
		{
			this.cells = cells;
		}

		public Cell GetCell(int colNo)
		{
			if (colNo < cells.Length&&cells[colNo] != null)
			{
				return cells[colNo];
			}
			return NULL_CELL;
		}

		public int GetFirstCellNum()
		{
			return 0;
		}

		public int GetLastCellNum()
		{
			return cells.Length-1;
		}
	}

	public class Cell
	{
		public string str;
		public BoolType b = BoolType.Null;
		public double d = Double.NaN;

		public Cell(string s)
		{
			this.str = s;
		}

		public int GetInt()
		{
			double d = GetNumericCellValue();
			if (double.IsNaN(d))
			{
				return 0;
			}
			return (int)d;
		}

		public float GetFloat()
		{
			double d = GetNumericCellValue();
			if (double.IsNaN(d))
			{
				return 0;
			}
			return (float)d;
		}

		public long GetLong()
		{
			double d = GetNumericCellValue();
			if (double.IsNaN(d))
			{
				return 0;
			}
			return (long)d;
		}

		public double GetDouble()
		{
			return GetNumericCellValue();
		}

		public string GetString()
		{
			return GetStringCellValue();
		}

		public T GetEnum<T>(T defaultValue) where T: struct, IComparable, IConvertible, IFormattable
		{
			return EnumUtil.Parse<T>(GetString(), defaultValue);
		}

		public double GetNumericCellValue()
		{
			if (Double.IsNaN(d))
			{
				if (str.Length > 0)
				{
					if (!double.TryParse(str, out d))
					{
						d = 0;
					}
				} else
				{
					d = 0;
				}
			}
			return d;
		}

		public bool GetBooleanCellValue()
		{
			if (b == BoolType.Null)
			{
				b = BoolTypeEx.Parse(str);
			}
			return b.IsTrue();
		}

		public string GetStringCellValue()
		{
			return str;
		}
	}

}

