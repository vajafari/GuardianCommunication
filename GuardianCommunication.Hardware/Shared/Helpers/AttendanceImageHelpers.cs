using System;
using System.IO;
using GuardianCommunication.Shared.ExtensionsAndUtilities;
using GuardianCommunication.Shared.HardwareDefinition;

namespace GuardianCommunication.Hardware.Shared.Helpers
{
	public static class AttendanceImageHelpers
	{
		// Resolved once (no per-image Process.MainModule call, which allocates an undisposed Process
		// and can throw on locked-down hosts). Equivalent to the exe directory for a Windows service.
		private static readonly string CaptureBaseDirectory =
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Capture");
        
        public static string GetAttendanceImageExtension(ProducerEnumeration producer)
		{
			return "jpg";
		}

		public static void SaveAttendanceImage(
			long employeeNumber
			, Guid deviceId
			, DateTime attendanceDateTime
			, string imageFormat
			, byte[] imageContent)
		{

			var path = CaptureBaseDirectory;
			var deviceSubDirectory = Path.Combine(path, deviceId.ToString());
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

		public static string GetImagePath(long employeeNumber, Guid deviceId, DateTime attendanceDateTime, string imageFormat)
		{
			return Path.Combine(CaptureBaseDirectory, deviceId.ToString(), GetImageName(employeeNumber, attendanceDateTime, imageFormat));
		}

		public static byte[] GetImageContent(long employeeNumber, Guid deviceId, DateTime attendanceDateTime, string imageFormat)
		{
			var imagePath = GetImagePath(employeeNumber, deviceId, attendanceDateTime, imageFormat);
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
