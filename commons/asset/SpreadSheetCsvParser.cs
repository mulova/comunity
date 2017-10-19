//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using Ideafixxxer.CsvParser;
using System.Text.RegularExpressions;
using System;

namespace commons {

	class SpreadSheetCsvParser : SpreadSheetParser {
		private VarReplacer replace = new VarReplacer();
		public SpreadSheetCsvParser(bool includeEmptyLine) : base(includeEmptyLine) {} 
        public bool enableComments = true;
        public const string COMMENT = "###";

		protected override Row ParseLine(string line) {
			line = replace.Replace(line);
			string[] tok = line.Split(',');
			Cell[] cells = new Cell[tok.Length];
			for (int i=0; i<tok.Length; i++) {
				if (tok[i].Length > 0) {
					cells[i] = new Cell(tok[i]);
				}
			}
			return new Row(cells);
		}
		
		override public List<Sheet> ParseSheet(TextReader r) {
			CsvParser p = new CsvParser();
			List<Row> rows = new List<Row>();
			foreach (string[] line in p.Parse(r)) {
                if (enableComments && (line.Length > 0&&line[0].StartsWith(COMMENT, StringComparison.Ordinal)))
                {
                    continue;
                }
				Cell[] cells = new Cell[line.Length];
				for (int i=0; i<line.Length; ++i) {
					cells[i] = new Cell(replace.Replace(line[i]));
				}
				rows.Add(new Row(cells));
			}
			Sheet s = new Sheet("sheet1", rows.ToArray());
			List<Sheet> sheets = new List<Sheet>();
			sheets.Add(s);
			return sheets;
		}
	}


	// legacy compliant.
	public class VarReplacer : TextReplacer {
		private int varCount;

		public VarReplacer() {
			AddReplaceString(@"\n", "\n");
			AddReplaceString("{STRING}", "{0}");
			AddReplaceString("{NUMBER[0-9]}", "{0}");
			AddReplaceString("%%", "%");
			AddReplaceString("%s", "{0}");
			AddReplaceString(@"%[0-9\.]*f", "{0}");
			AddReplaceString(@"%[0-9\.]*d", "{0:N0}");
		}
		
		public override string Replace(string str) {
			varCount = -1;
			return base.Replace(str);
		}
		
		protected override string MatchExp(Match m) {
			if (m.Value.StartsWith("{STRING}")) {
				return string.Concat("{", ++varCount, "}");
			} else if (m.Value.StartsWith("{NUMBER")) {
				// assume just one character
				return string.Concat("{", ++varCount, ":N", m.Value["{NUMBER".Length], "}");
			} else if (m.Value == "%s") {
				return string.Concat("{", ++varCount, "}");
			} else if (m.Value.StartsWithIgnoreCase("%") && m.Value.EndsWithIgnoreCase("d")) {
				return string.Concat("{", ++varCount, ":N0}");
			} else if (m.Value.StartsWithIgnoreCase("%") && m.Value.EndsWithIgnoreCase("f")) {
				return string.Concat("{", ++varCount, ":N0}");
			} else {
				return base.MatchExp(m);
			}
		}
	}
}

