//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System.Globalization;
using System;
using System.IO;
using System.Text;

public static class EncodingUtil {

	public static string ConvertThreeLetterNameToTwoLetterName(string name)
	{
		if (name.Length != 3)
		{
			throw new ArgumentException("name must be three letters.");
		}
		
		name = name.ToUpper();
		
		CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
		foreach (CultureInfo culture in cultures)
		{
			RegionInfo region = new RegionInfo(culture.LCID);
			if (region.ThreeLetterISORegionName.ToUpper() == name)
			{
				return region.TwoLetterISORegionName;
			}
		}
		
		return null;
	}

	private static char[] initialConsonant = {'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ'};
	private static char[] middleConsonant = {'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ', 'ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ', 'ㅣ'};
	private static char[] finalConsonant = { (char)0, 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ', 'ㄻ', 'ㄼ', 'ㄽ', 'ㄾ',
		'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ', 'ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
	public static char[] DisassembleSyllable(char ch) {
		int c = ch-0xAC00;
		if (c < 0) {
			return null;
		}
		int final = c%28;
		c /= 28;
		int middle = c%21;
		c /= 21;
		int initial = c;
		return new char[] { initialConsonant[initial], middleConsonant[middle], finalConsonant[final] };
	}
	
	public static bool IsUTF8KoreanConsonant(char ch) {
		return (ch >= 0x1100 && ch <= 0x11FF) // Hangul Jamo
			|| (ch >= 0x3130 && ch <= 0x318F); // Hangul Compatibility Jamo
	}
	
	public static bool IsUTF8Korean(char ch) {
		return ch >= 0xAC00 && ch <= 0xD7AF;
	}
	
	/**
		 * 목적어 조사
		 */
	public static string AttachObjectPostPosition(string s) {
		return s+(IsPostPositionType1(s)? "을 ": "를 ");
	}
	
	public static string AttachSubjectPostPosition(string s) {
		return s+(IsPostPositionType1(s)? "이 ": "가 ");
	}
	
	public static string AttachByPostPosition(string s) {
		return s+(IsPostPositionType1(s)? "으로 ": "로 ");
	}
	
	public static string GetCausePostPosition(string s) {
		return s+(IsPostPositionType1(s)? "으로 인해 ": "로 인해 ");
	}
	
	/**
		 * @return 받침이 있으면 true
		 */
	public static bool IsPostPositionType1(string s) {
		char[] syllable = DisassembleSyllable(s[s.Length-1]);
		return syllable!=null && syllable[2]!=0;
	}

	public static string Encode(Stream stream, Encoding enc) {
		StreamReader reader = new StreamReader(stream, enc);
		string str = reader.ReadToEnd();
		reader.Close();
		return str;
	}
}
