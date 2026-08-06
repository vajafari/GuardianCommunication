using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace GuardianCommunication.Shared.ExtensionsAndUtilities
{
    public static class ImageHelper
    {
        public static byte[] ResizeImageByDimensions(byte[] imageBytes, int maxWidth, int maxHeight)
        {
            using (var ms = new MemoryStream(imageBytes))
            using (var img = Image.FromStream(ms))
            {
                if (img.Width <= maxWidth && img.Height <= maxHeight)
                    return imageBytes;

                float ratioX = (float)maxWidth / img.Width;
                float ratioY = (float)maxHeight / img.Height;
                float ratio = Math.Min(ratioX, ratioY);

                int newWidth = (int)(img.Width * ratio);
                int newHeight = (int)(img.Height * ratio);

                using (var newImage = new Bitmap(newWidth, newHeight))
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;

                    graphics.DrawImage(img, 0, 0, newWidth, newHeight);

                    using (var resultStream = new MemoryStream())
                    {
                        newImage.Save(resultStream, ImageFormat.Jpeg);
                        return resultStream.ToArray();
                    }
                }
            }
        }

        public static byte[] ReduceImageSize(byte[] imageBytes, int maxSizeInKb)
        {
            long targetBytes = maxSizeInKb * 1024L; // تبدیل کیلوبایت به بایت
            if (imageBytes.Length <= targetBytes)
                return imageBytes;

            using (var ms = new MemoryStream(imageBytes))
            using (var img = Image.FromStream(ms))
            {
                long quality = 90L; // کیفیت شروع
                byte[] currentBytes = imageBytes;

                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
                EncoderParameters encoderParameters = new EncoderParameters(1);

                // مرحله اول: کاهش کیفیت عکس (بدون تغییر ابعاد)
                while (currentBytes.Length > targetBytes && quality > 10L)
                {
                    quality -= 10L; // هر بار 10 درصد کیفیت را کم می‌کنیم
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);

                    using (var resultStream = new MemoryStream())
                    {
                        img.Save(resultStream, jpegCodec, encoderParameters);
                        currentBytes = resultStream.ToArray();
                    }
                }

                // مرحله دوم: اگر افت کیفیت کافی نبود، ابعاد را پله پله با حفظ Ratio کوچک می‌کنیم
                float scale = 0.9f;
                int currentWidth = img.Width;
                int currentHeight = img.Height;

                while (currentBytes.Length > targetBytes)
                {
                    currentWidth = (int)(currentWidth * scale);
                    currentHeight = (int)(currentHeight * scale);

                    using (var resizedImg = new Bitmap(currentWidth, currentHeight))
                    using (var graphics = Graphics.FromImage(resizedImg))
                    {
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(img, 0, 0, currentWidth, currentHeight);

                        using (var resultStream = new MemoryStream())
                        {
                            // ذخیره با کمترین کیفیت مجاز (10) در ابعاد جدید
                            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 10L);
                            resizedImg.Save(resultStream, jpegCodec, encoderParameters);
                            currentBytes = resultStream.ToArray();
                        }
                    }
                }

                return currentBytes;
            }
        }

        // متد کمکی برای دریافت انکودر JPEG
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType == mimeType)
                    return codec;
            }
            return null;
        }
    }
}
