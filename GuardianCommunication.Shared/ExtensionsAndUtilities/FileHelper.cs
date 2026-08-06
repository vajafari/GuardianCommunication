using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
    public static class FileHelper
    {
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public static string CreateFileOnCurrentExecutingPath(string relativePath, byte[] fileBytes)
        {
            var finalFilePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, relativePath);
            var directoryPath = Path.GetDirectoryName(finalFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllBytes(finalFilePath, fileBytes);
            return finalFilePath;
        }

        private const int BytesToRead = sizeof(Int64);
        public static bool CompareFiles(string firstFile, string secondFile, bool compareName)
        {

            if (!File.Exists(firstFile) || !File.Exists(secondFile))
            {
                return false;
            }

            var firstFileInfo = new FileInfo(firstFile);
            var secondFileInfo = new FileInfo(secondFile);

            if (firstFileInfo.Length != secondFileInfo.Length)
                return false;
            if (compareName)
            {
                if (string.Equals(firstFileInfo.FullName, secondFileInfo.FullName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            var iterations = (int)Math.Ceiling((double)firstFileInfo.Length / BytesToRead);

            using (var fsFirst = firstFileInfo.OpenRead())
            using (var fsSecond = secondFileInfo.OpenRead())
            {
                var one = new byte[BytesToRead];
                var two = new byte[BytesToRead];

                for (var i = 0; i < iterations; i++)
                {
                    _ = fsFirst.Read(one, 0, BytesToRead);
                    _ = fsSecond.Read(two, 0, BytesToRead);
                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }
    }
}
