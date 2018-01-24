//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System;


namespace commons {
	class SpreadSheetXMLParser : SpreadSheetParser {
		
		public static readonly string LINE_SEPERATOR = "<_R_>";
		public static readonly string CELL_SEPERATOR = "<_C_>";
		public static readonly string SHEET_SEPERATOR = "<_S_>";
		
		public SpreadSheetXMLParser() : base (false){
			
		}
		
		protected override Row ParseLine(string line) {
			string[] tok = Regex.Split(line, CELL_SEPERATOR);
			Cell[] cells = new Cell[tok.Length];
			for (int i=0; i<tok.Length; i++) {
				if (tok[i].Length > 0) {
					cells[i] = new Cell(tok[i]);
				}
			}
			return new Row(cells);
		}
		
		override public List<Sheet> ParseSheet(TextReader r) {
			StringBuilder str = new StringBuilder();
			List<Sheet> tempSheets = new List<Sheet>();
			
			string line;
			Sheet sheet = null;
			while ((line = r.ReadLine()) != null) {
				string l = line.Trim();
				
				if (l.EndsWithIgnoreCase(SHEET_SEPERATOR))
				{
					string[] tok = Regex.Split(l, SHEET_SEPERATOR);
					sheet = new Sheet(tok[0]);
					tempSheets.Add(sheet);
				}
				else
				{
					if (l.Length == 0 || l.StartsWith("#") || l.StartsWith("//")) {
						continue;
					}
					
					if (l.EndsWithIgnoreCase(LINE_SEPERATOR))	{
						str.Append(l.Substring(0, l.Length-LINE_SEPERATOR.Length));
						Row row = ParseLine(str.ToString());
						if (row != null) {
							sheet.AddRow(row);
						}
						str.Length = 0;
					} else {
						str.Append(l);
						str.Append('\n');
					}
				}
			}
			return tempSheets;
		}
	}
}

