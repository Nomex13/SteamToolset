using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SteamToolset
{
	public static class Util
	{
		public static void ConsoleClearLinePrevious()
		{
			Console.SetCursorPosition(0, Console.CursorTop - 1);
			ConsoleClearLineCurrent();
		}
		public static void ConsoleClearLineCurrent()
		{
			int currentLineCursor = Console.CursorTop;
			Console.SetCursorPosition(0, Console.CursorTop);
			Console.Write(new string(' ', Console.WindowWidth)); 
			Console.SetCursorPosition(0, currentLineCursor);
		}
		public static DateTime GetLinkerTime(this Assembly param_assembly, TimeZoneInfo param_target = null)
		{
			var filePath = param_assembly.Location;
			const int c_PeHeaderOffset = 60;
			const int c_LinkerTimestampOffset = 8;

			var buffer = new byte[2048];

			using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			stream.Read(buffer, 0, 2048);

			var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
			var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

			var tz = param_target ?? TimeZoneInfo.Local;
			var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

			return localTime;
		}
	}
}
