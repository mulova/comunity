//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

namespace commons {
	
	/**
	 * text asset을 읽어 String List로 변경한다.
	 */
	public class LineParser : TextRowParser<string> {
		
		public LineParser(bool includeEmptyLine) : base(includeEmptyLine) {} 
		
		protected override string ParseLine(string line) {
			return line;
		}
	}
	
	
}