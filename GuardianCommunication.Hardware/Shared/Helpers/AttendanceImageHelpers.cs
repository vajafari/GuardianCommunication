using System;
using System.IO;

namespace GuardianCommunication.Hardware.Shared.Helpers
{
	public static class AttendanceImageHelpers
	{
		// Resolved once (no per-image Process.MainModule call, which allocates an undisposed Process
		// and can throw on locked-down hosts). Equivalent to the exe directory for a Windows service.
		private static readonly string CaptureBaseDirectory =
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Capture");

		public static string GetAttendanceImageExtention(ProducerEnumeration producer)
		{
			return "jpg";
		}

		public static void SaveAttendanceImage(
			long employeeNumber
			, int deviceNumber
			, DateTime attendanceDateTime
			, string imageFormat
			, byte[] imageContent)
		{

			var path = CaptureBaseDirectory;
			var deviceSubDirectory = Path.Combine(path, deviceNumber.ToString());
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			if (!Directory.Exists(deviceSubDirectory))
			{
				Directory.CreateDirectory(deviceSubDirectory);
			}

			path = Path.Combine(deviceSubDirectory, GetImageName(employeeNumber, attendanceDateTime, imageFormat));
			File.WriteAllBytes(path, imageContent);
		}

		public static string GetImagePath(long employeeNumber, int deviceNumber, DateTime attendanceDateTime, string imageFormat)
		{
			return Path.Combine(CaptureBaseDirectory, deviceNumber.ToString(), GetImageName(employeeNumber, attendanceDateTime, imageFormat));
		}

		public static byte[] GetImageContent(long employeeNumber, int deviceNumber, DateTime attendanceDateTime, string imageFormat)
		{
			var imagePath = GetImagePath(employeeNumber, deviceNumber, attendanceDateTime, imageFormat);
			if (imagePath.IsNotNullOrEmpty() && File.Exists(imagePath))
			{
				return File.ReadAllBytes(imagePath);
			}

			return null;
		}

		public static string GetImageName(long employeeNumber, DateTime attendanceDateTime, string imageFormat)
		{
			var timeString = attendanceDateTime.ToString("yyyyMMddHHmmss");
			return $"{timeString}-{employeeNumber}.{imageFormat}";
		}

	}
}
