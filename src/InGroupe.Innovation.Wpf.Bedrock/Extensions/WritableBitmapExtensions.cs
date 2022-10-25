using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;

namespace InGroupe.Innovation.Wpf.Bedrock.Extensions;

public static class WritableBitmapExtensions
{
    /// <summary>
    /// Writes the specified bitmap into the targeted <see cref="WriteableBitmap"/>.
    /// </summary>
    /// <param name="writeableBitmap">The writeable bitmap.</param>
    /// <param name="bitmap">The bitmap.</param>
    public static void WriteToWritableBitmap(this WriteableBitmap writeableBitmap, [DisallowNull] Bitmap bitmap)
    {
        var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly,
            bitmap.PixelFormat);

        var bytesPerPixel = (writeableBitmap.Format.BitsPerPixel + 7) / 8;
        var stride = bytesPerPixel * bitmap.Width;
        var bufferSize = bitmap.Height * stride;

        writeableBitmap.Lock();
        
        writeableBitmap.WritePixels(new Int32Rect(0, 0, bitmap.Width, bitmap.Height), bitmapData.Scan0, bufferSize, bitmapData.Stride);

        writeableBitmap.Unlock();
        
        bitmap.UnlockBits(bitmapData);
    }
}
