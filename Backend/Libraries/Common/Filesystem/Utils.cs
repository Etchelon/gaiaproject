using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ScoreSheets.Common.Filesystem
{
	public static class Utils
	{
		public static string GetExecutingDirectoryName()
		{
			var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
			var decodedPath = System.Web.HttpUtility.UrlDecode(location.AbsolutePath, Encoding.UTF8);
			return new FileInfo(decodedPath).Directory.FullName;
		}
	}
}
