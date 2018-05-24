using System;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace comunity
{
	public class WarningRemover : ScriptableWizard
	{
		[TextArea(10, 100)]
		public string warnings;
		
		public WarningRemover()
		{
			this.createButtonName = "Remove";
		}
		
		public void OnWizardCreate()
		{
			var pragma = new WarningPragma();
			pragma.Convert(warnings);
			
		}
		
		[MenuItem( "Tools/Warning Remover",false, 9999)]
		public static void OpenWindow()
		{
			GetWindow<WarningRemover>( "Warning Remover" );
		}
		
		class WarningPragma
		{
			private Regex regex;
			public WarningPragma()
			{
				this.regex = new Regex(@"(.*)\((\d+)\,\d+\)\s*:\s*warning\s*CS(\d{4})");
			}
			
			public void Convert(string warningMessage)
			{
				string currentPath = null;
				StreamReader stream = null;
				List<string> lines = new List<string>();
				int lineCount = 0;
				MatchCollection matches = this.regex.Matches(warningMessage);   
				
				foreach (Match m in matches)
				{
					string path = m.Groups[1].Value;
					int lineNo = int.Parse(m.Groups[2].Value);
					string warnNo = m.Groups[3].Value;
					Debug.LogFormat("file:{0}, line:{1}, warn:{2}", path, lineNo, warnNo);
					
					if (currentPath != path)
					{
						if (stream != null)
						{
							// flush old file
							while (!stream.EndOfStream)
							{
								lines.Add(stream.ReadLine());
								lineCount++;
							}
							stream.Close();
							File.WriteAllLines(currentPath, lines.ToArray());
						}
						stream = new StreamReader(new FileStream(path, FileMode.Open), Encoding.UTF8);
						currentPath = path;
						// Load new file
						lineCount = 0;
						lines.Clear();
					}
					// Flush lines until the warning line met
					if (lineCount < lineNo)
					{
						while (lineCount < lineNo-1)
						{
							lines.Add(stream.ReadLine());
							lineCount++;
						}
						// add #pragma between warning line
						lines.Add("#pragma warning disable "+warnNo);
						lines.Add(stream.ReadLine());
						lineCount++;
						lines.Add("#pragma warning restore "+warnNo);
					}
				}
				if (currentPath != null)
				{
					while (!stream.EndOfStream)
					{
						lines.Add(stream.ReadLine());
						lineCount++;
					}
					stream.Close();
					File.WriteAllLines(currentPath, lines.ToArray());
				}
				
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			}
			
		}
	}
}

