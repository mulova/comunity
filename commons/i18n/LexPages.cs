using System;
using System.Collections.Generic;
using System.Text;

namespace commons {
	/// <summary>
	/// Message pool.
	/// Language type is described in the column name of the first row of each sheet.
	/// </summary>
	
	public class LexPages {
		private List<LexPage> pages = new List<LexPage>();
		private LexPage currentPage;
        private object lang;
		
        public static Loggerx log = LogManager.GetLogger(typeof(LexPages));
		
		/// <summary>
		/// The first row of each sheet is considered as a title row
		/// </summary>
		/// <param name='asset'>
		/// Asset.
		/// </param>
		public void Add(byte[] bytes, SpreadSheetSourceType sourceType = SpreadSheetSourceType.CSV) {
			SpreadSheet xls = new SpreadSheet(bytes, sourceType);
			for (int i=0; i<xls.GetSheetCount(); ++i) {
				xls.SetSheet(i);
                LexPage p = new LexPage(xls, pages.Count);
                p.SetLanguage(lang);
				pages.Add(p);
			}
			PageNo = 0;
		}
		
		public int PageNo {
			set {
				this.currentPage = pages[value];
			}
			get {
				return this.currentPage.no;
			}
		}
		
		public string Page {
			set {
				PageNo = GetPageNo(value);
			}
			get {
				return currentPage.name;
			}
		}

		private List<string> _keys;
		public List<string> keys {
			get {
				CheckInitialization();
				if (_keys == null) {
					_keys = new List<string>();
					foreach (LexPage p in pages) {
						_keys.AddRange(p.keys);
					}
				}
				return _keys;
			}
		}

        public ICollection<string> columnNames {
            get {
                CheckInitialization();
                if (pages.Count == 0)
                {
                    return null;
                }
                return pages[0].columnNames;
            }
        }

		public string FindKey(string msg) {
			foreach (LexPage p in pages) {
				string k = p.FindKey(msg);
				if (k.IsNotEmpty()) {
					return k;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// Finds the unique key for the message 
		/// </summary>
		/// <returns>The key strict.</returns>
		/// <param name="msg">Message.</param>
		public string FindUniqueKey(string msg) {
			string key = null;
			foreach (LexPage p in pages) {
				string k = p.FindKey(msg);
				if (k.IsNotEmpty()) {
					if (key.IsNotEmpty())
					{
						return string.Empty; // duplicate
					} else {
						key = k;
					}
				}
			}
			return key;
		}
		
		public LexPage GetPage(string pageName) {
			int pageNo = GetPageNo(pageName);
			if (pageNo < 0) {
				log.Warn("No excel sheet name '{0}'", pageName);
				return null;
			}
			return pages[pageNo];
		}
		
		public int GetPageNo(string pageName) {
			for (int i=0; i<pages.Count; i++) {
				if (pages[i].name == pageName) {
					return i;
				}
			}
			return -1;
		}

		public void AddPage(LexPage page) {
			pages.Add(page);
		}
		
		/// <summary>
		/// Get message from the current page
		/// </summary>
		/// <param name="id">Identifier.</param>
		/// <param name="replace">Replace.</param>
		public string Get(string id) {
			return currentPage.Get(id);
		}
		
		public string Get(string id, int col) {
            CheckInitialization();
            foreach (LexPage p in pages) {
                string msg = p.Get(id, col);
                if (msg.IsNotEmpty()) {
                    return msg;
                }
            }
			return string.Empty;
		}

        public string Get(string id, object colKey) {
            CheckInitialization();
            foreach (LexPage p in pages) {
                string msg = p.Get(id, colKey);
                if (msg.IsNotEmpty()) {
                    return msg;
                }
            }
            return string.Empty;
        }

		public bool ContainsKey(string key) {
			foreach (LexPage p in pages) {
				if (p.ContainsKey(key)) {
					return true;
				}
			}
			return false;
		}

		private bool warning;
		private void CheckInitialization() {
			if (pages.IsEmpty()&& !warning) {
				warning = true;
				log.Warn("MessagePool is not initialized yet");
			}
		}
		
		/// <summary>
		/// Get value for key from any pages
		/// </summary>
		/// <param name="m">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string GetAny(string id) {
			CheckInitialization();
			foreach (LexPage p in pages) {
				string msg = p.Get(id);
				if (msg.IsNotEmpty()) {
					return msg;
				}
			}
			log.Info("No Message for key '{0}'", id);
			return id;
		}

        public string[] GetRow(string id) {
            CheckInitialization();
            foreach (LexPage p in pages) {
                string[] msg = p.GetRow(id);
                if (msg != null) {
                    return msg;
                }
            }
            return null;
        }
		
		public string Translate(string m) {
			CheckInitialization();
			foreach (LexPage p in pages) {
				string msg = p.Translate(m);
				if (!Object.ReferenceEquals(msg, m)) {
					return msg;
				}
			}
			return m;
		}
		
		/// <summary>
		/// Sets the language.
		/// </summary>
		/// <returns>
		/// true if all sheets have the column name with 'lang'
		/// </returns>
		/// <param name='lang'>
		/// </param>
		public bool SetLanguage(object langKey) {
			CheckInitialization();
            this.lang = langKey;
			bool success = true;
			foreach (LexPage page in pages) {
				success &= page.SetLanguage(langKey);
			}
			return success;
		}
		
		public void SetColumn(int column) {
			CheckInitialization();
			foreach (LexPage page in pages) {
				page.SetColumn(column);
			}
		}
		
		public bool SetMotherLanguage(object lang) {
			CheckInitialization();
			bool success = true;
			foreach (LexPage page in pages) {
				success &= page.SetMotherLanguage(lang);
			}
			return success;
		}

		public void Clear() {
			pages.Clear();
			currentPage = null;
		}

		public bool IsEmpty() {
			return pages.IsEmpty();
		}
	}
}
