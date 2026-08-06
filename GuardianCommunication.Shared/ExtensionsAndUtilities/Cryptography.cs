using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
    public static class Cryptography
    {

        public static string ToSha256Hash(this string valueToHash)
        {
            var result = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var enc = Encoding.UTF8;
                var bytesOfValue = hash.ComputeHash(enc.GetBytes(valueToHash));
                foreach (var b in bytesOfValue)
                {
                    result.Append(b.ToString("x2"));
                }

                return result.ToString();
            }
        }

        public static string ToSha512Hash(this string valueToHash)
        {
            var result = new StringBuilder();
            using (var hash = SHA512.Create())
            {
                var enc = Encoding.UTF8;
                var bytesOfValue = hash.ComputeHash(enc.GetBytes(valueToHash));
                foreach (var b in bytesOfValue)
                {
                    result.Append(b.ToString("x2"));
                }
                return result.ToString();
            }
        }

        public static string ToSha256CheckSum(this string valueToHash)
        {
            var sb = new StringBuilder(valueToHash);
            sb.AppendLine(valueToHash.ToMd5Hash());
            sb.AppendLine(valueToHash.ToSha256Hash());
            return ToSha256Hash(sb.ToString());
        }

        public static string ToMd5Hash(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string Encrypt(string clearText, string encryptionKey)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (var encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(encryptionKey,
                    new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 },
                    40, HashAlgorithmName.SHA512);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
                return clearText;
            }
        }

        public static string Decrypt(string encryptedText, string encryptionKey)
        {
            encryptedText = encryptedText.Replace(" ", "+");
            var encryptedTextBytes = Convert.FromBase64String(encryptedText);
            using (var encryptor = Aes.Create())
            {
                using (var pdb = new Rfc2898DeriveBytes(encryptionKey,
                           new byte[]
                           {
                               0x50, 0x40, 0x64, 0x69, 0x73, 0x20, 0x47, 0x72, 0x6f, 0x75, 0x70, 0x20, 0x31, 0x33, 0x39,
                               0x39
                           },
                           40, HashAlgorithmName.SHA512))
                {
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
                            cs.Close();
                        }

                        return Encoding.Unicode.GetString(ms.ToArray());
                    }
                }

            }
        }

        public static ushort ToCrc16XModem(string text)
        {
            return ToCrc16XModem(Encoding.ASCII.GetBytes(text));
        }

        public static ushort ToCrc16XModem(byte[] data)
        {
            const ushort polynomial = 0x1021;
            ushort crc = 0x0000;

            foreach (byte b in data)
            {
                crc ^= (ushort)(b << 8);

                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ polynomial);
                    else
                        crc <<= 1;
                }
            }

            return crc;
        }
    }
}
