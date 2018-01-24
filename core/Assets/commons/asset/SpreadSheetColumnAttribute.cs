
namespace commons {
	public class SpreadSheetColumnAttribute : System.Attribute {
		public readonly string column;
		public readonly int columnIndex;
		public readonly SSColumnType columnType;
		public readonly object defaultValue;

		public SpreadSheetColumnAttribute(string column) : this(column, null, SSColumnType.None) { }

		public SpreadSheetColumnAttribute(string column, object defValue) : this(column, defValue, SSColumnType.None) {
		}
		public SpreadSheetColumnAttribute(string column, SSColumnType colType) : this(column, null, colType) {
		}

		public SpreadSheetColumnAttribute(string column, object defaultValue, SSColumnType columnType) {
			this.column = column;
			this.columnIndex = GetColumnIndex(column);
			this.columnType = columnType;
			this.defaultValue = defaultValue;
		}

		private int GetColumnIndex(string col) {
			string uppercase = col.ToUpper();
			int index = uppercase[0]-'A'+1;
			for (int i=1; i<uppercase.Length; i++) {
				index*=26;
                index += (uppercase[i]-'A')+1;
			}
			return index-1;
		}
	}
}