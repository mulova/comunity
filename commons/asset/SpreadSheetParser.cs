//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace commons {
	abstract class SpreadSheetParser : TextRowParser<Row> {
		public SpreadSheetParser(bool includeEmptyLine) : base(includeEmptyLine) { }
		
		public List<Sheet> ParseSheet(byte[] bytes, Encoding e) {
			return ParseSheet(new MemoryStream(bytes), e);
		}

		public List<Sheet> ParseSheet(Stream s, Encoding e) {
			return ParseSheet(new StreamReader(s, e));
		}
		
		public abstract List<Sheet> ParseSheet(TextReader r);
	}
}

