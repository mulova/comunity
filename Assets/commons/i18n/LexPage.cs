using System;
using System.Collections.Generic;
using System.Collections.Generic.Ex;
using System.Ex;
using System.Text.Ex;

namespace commons {
	public class LexPage
	{
		public readonly int no;
		public readonly string name;
		private Dictionary<string, string[]> sheet = new Dictionary<string, string[]>();  // key column('A') -> messages ('B'~)
		private Dictionary<string, string> translator = new Dictionary<string, string>(); // mother language column -> key ('A')
		private Dictionary<string, int> columnTitles = new Dictionary<string, int> ();
		private int motherLangCol = -1;
		private int selectedCol = 0;
        private static Loggerx log = LogManager.GetLogger (typeof(LexPages));

		public LexPage(string name, int no) {
			this.no = no;
			this.name = name;
		}
		
        public LexPage(SpreadSheet xls, int no) : this(xls.GetSheet().name, no)
		{
            char endCol = (char)('A'+xls.GetRow(0).GetLastCellNum());
			for (char c='B'; c<=endCol; ++c) {
				columnTitles[xls.GetString1(1, c)] = c-'B';
			}
			List<string> list = new List<string> ();
			for (int r=1; r<=xls.GetSheet().GetLastRowNum(); ++r) {
				list.Clear ();
				string key = xls.GetString1(r+1, 'A').Trim();
				if (!key.IsEmpty()) {
					for (char c='B'; c<=endCol; ++c) {
						list.Add (xls.GetString1(r+1, c));
						// TODOM use filter instead of Replace
//						list.Add (xls.GetString(r+1, c).Replace("\\n", "\n"));
					}
					if (list.Count > 0) {
						string[] arr = list.ToArray();
						if (sheet.ContainsKey(key)) {
							log.Error("Duplicate key: {0}.{1}", name, key);
						}
						sheet[key] = arr;
					}
				}
			}
		}
		
		public bool SetColumn(object columnName)
		{
			return columnTitles.TryGetValue(columnName.ToText(), out selectedCol);
		}

		public void AddColumn(object columnName) {
			columnTitles.Add(columnName.ToText(), columnTitles.Count);
		}

		public int GetColumnNo(object columnName) {
			return columnTitles.Get(columnName.ToText(), -1);
		}

		public void SetValue(string rowKey, string columnName, string val) {
			int colNo = GetColumnNo(columnName);
			string[] row = sheet.Get(rowKey);
			if (row == null) {
				row = new string[colNo+1];
				sheet.Add(rowKey, row);
			}
			row[colNo] = val;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="i">
		/// 0 is the column 'B'
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public void SetColumn(int i)
		{
			selectedCol = i;
		}
		
		/// <summary>
		/// Translate language from -> to
		/// </summary>
		/// <param name="langFrom">
		/// A <see cref="object"/>
		/// </param>
		/// <param name="langTo">
		/// A <see cref="object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool SetMotherLanguage(object langFrom)
		{
			HashSet<string> conflict = new HashSet<string>();
			if (columnTitles.TryGetValue(langFrom.ToText(), out motherLangCol)) {
				translator.Clear();
				foreach (KeyValuePair<string, string[]> pair in sheet) {
					if (motherLangCol < pair.Value.Length)
					{
						string transKey = pair.Value[motherLangCol].OnlyLetterDigit();
						if (transKey.IsEmpty() || conflict.Contains(transKey)) { continue; }
						if (translator.ContainsKey(transKey))
						{
							conflict.Add(transKey);
							translator.Remove(transKey);
							log.Info("Same value exists. Translation fails for {0}", transKey);
						} else 
						{
							translator[transKey] = pair.Key;
						}
					}
				}
				return true;
			} else {
				return false;
			}
		}
		
		public bool SetLanguage(object langKey)
		{
			return SetColumn(langKey);
		}
		
		public string this [string key] {
			get {
				return Get(key);
			}
		}
		
		public string Translate(string text) {
			string msg = text;
			if (translator.TryGetValue(text.OnlyLetterDigit(), out msg)) {
				return Get(msg);
			}
			return text;
		}

		public string FindKey(string msg) {
			return translator.Get(msg.OnlyLetterDigit());
		}

		public ICollection<string> keys {
			get {
				return sheet.Keys;
			}
		}

        public ICollection<string> columnNames {
            get {
                return columnTitles.Keys;
            }
        }

		public bool ContainsKey(string key) {
			return key.IsNotEmpty() && sheet.ContainsKey(key);
		}

        public string[] GetRow(string key)
        {
            if (key.IsEmpty() || sheet == null) {
                return null;
            }
            return sheet.Get(key);
        }
		
		public string Get(string key)
		{
			return Get(key, selectedCol);
		}

        public string Get(string key, object colKey)
        {
            return Get(key, GetColumnNo(colKey));
        }
		
		public string Get(string key, int col)
		{
			if (key.IsEmpty() || sheet == null) {
				return string.Empty;
			}
			string[] slot = null;
			if (!sheet.TryGetValue(key, out slot)) {
				return string.Empty;
			}
			if (col > slot.Length) {
				log.Warn("No Message for {0} at column {1} in page '{2}'", key, col, name);
				return string.Empty;
			}
			return slot[col];
		}
		
		public override string ToString()
		{
			return name;
		}
	}
}

