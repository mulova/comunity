using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Core;

namespace UnilovaCoreTest
{
	[TestFixture]
	[Category ("StringExtMethod")]
	internal class StringExtMethodTest
	{
		[Test]
		public void EqualsIgnoreWhiteSpacePass()
		{
			Assert.IsTrue("".EqualsIgnoreWhiteSpace(null));
			Assert.IsTrue("".EqualsIgnoreWhiteSpace(""));
			Assert.IsTrue("".EqualsIgnoreWhiteSpace(" \n"));

			Assert.IsTrue("a".EqualsIgnoreWhiteSpace("a "));
			Assert.IsTrue("a".EqualsIgnoreWhiteSpace(" a\r\n"));
			Assert.IsTrue("a \nb".EqualsIgnoreWhiteSpace("ab  "));
			Assert.IsTrue("a bc\n".EqualsIgnoreWhiteSpace("ab c\n"));
			Assert.IsTrue("한글은 어떤가?".EqualsIgnoreWhiteSpace("한글은어떤가?"));
		}

		[Test]
		public void EqualsIgnoreWhiteSpaceFail()
		{
			Assert.IsFalse("a".EqualsIgnoreWhiteSpace(" a b"));
			Assert.IsFalse("abxx".EqualsIgnoreWhiteSpace("abx"));
			Assert.IsFalse("한글은 어떤가?".EqualsIgnoreWhiteSpace("한글은 어떠니?"));
		}

		[Test]
		public void RemoveWhiteSpaceTest()
		{
			Assert.AreEqual("".RemoveWhiteSpace(), "");
			Assert.AreEqual(" \r\n".RemoveWhiteSpace(), "");
			
			Assert.AreEqual("a", " a\r\n".RemoveWhiteSpace());
			Assert.AreEqual("ab", "ab  ".RemoveWhiteSpace());
			Assert.AreEqual("abc", "ab c\n".RemoveWhiteSpace());
			Assert.AreEqual("한글은어떤가?", "한글은어떤가?".RemoveWhiteSpace());
			Assert.AreEqual("한글은어떤가?", "한글은 \r\n어떤가\n? ".RemoveWhiteSpace());
		}

		[Test]
		public void GetSuffixNumberTest()
		{
			Assert.AreEqual("".GetSuffixNumber(), 0);
			Assert.AreEqual("12_3".GetSuffixNumber(), 3);
			Assert.AreEqual("3".GetSuffixNumber(), 3);
			Assert.AreEqual("ab12".GetSuffixNumber(), 12);
			Assert.AreEqual("ab_34".GetSuffixNumber(), 34);
		}
	}
}
