//----------------------------------------------
// Unity3D common libraries and editor tools
// License: The MIT License ( http://opensource.org/licenses/MIT )
// Copyright © 2013- mulova@gmail.com
//----------------------------------------------

using System;
using System.IO;

namespace commons {
	public static class DirectoryInfoEx
	{
		public static FileStream CreateUniqueFile( this DirectoryInfo dir , string rootName )
		{
			if (!dir.Exists) {
				dir.Create();
			}
			FileStream fs = dir.TryCreateFile( rootName ) ; // try the simple name first
			
			// if that didn't work, try mixing in the date/time
			if ( fs == null )
			{
				string date = DateTime.Now.ToString( "yyyy-MM-ddTHHmmss" ) ;
				string stem = Path.GetFileNameWithoutExtension(rootName) ;
				string ext  = Path.GetExtension(rootName) ?? ".dat" ;
				
				ext = ext.Substring(1);
				
				string fn = string.Format( "{0}.{1}.{2}" , stem , date , ext ) ;
				fs = dir.TryCreateFile( fn ) ;
				
				// if mixing in the date/time didn't work, try a sequential search
				if ( fs == null )
				{
					int seq = 0 ;
					do
					{
						fn = string.Format( "{0}.{1}.{2:0000}.{3}" , stem , date , ++seq , ext ) ;
						fs = dir.TryCreateFile( fn ) ;
					} while ( fs == null ) ;
				}
				
			}
			
			return fs ;
		}
		
		private static FileStream TryCreateFile(this DirectoryInfo dir , string fileName )
		{
			FileStream fs = null ;
			try
			{
				string fqn = Path.Combine( dir.FullName , fileName ) ;
				
				fs = new FileStream(fqn , FileMode.CreateNew , FileAccess.Write , FileShare.None ) ;
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
				fs = null ;
			}
			return fs ;
		}
		
	}
}
