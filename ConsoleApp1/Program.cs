using System.Drawing;
using System.Drawing.Imaging;
using ConsoleApp1;

class Program
{
    static float[,] ArrayFromImage(string path)
    {
        var bitmap = new Bitmap(path, true);
        var imgary = new float[bitmap.Width,bitmap.Height];

        int x,y;

        for (x = 0; x < bitmap.Width; x++)
        {
            for (y = 0; y < bitmap.Height; y++)
            {
                imgary[x,y] = bitmap.GetPixel(x,y).ToArgb();
            }
        }

        return imgary;
    }

    static void WriteToImage(string path, float[,] data)
    {
        // Create 2D array of integers
        var width = data.GetLength(1);
        var height = data.GetLength(0);
        var stride = width * 4;
        var integers = new int[width,height];
        
        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                integers[x, y] = (int)data[x, y];
            }
        }
        
        Bitmap bitmap;
        unsafe
        {
            fixed (int* intPtr = &integers[0,0])
            {
                bitmap = new Bitmap(width, height, stride, PixelFormat.Format32bppPArgb, new IntPtr(intPtr));
            }
        }

        ToGrayScale(bitmap);
        bitmap.Save(path);
    }
    
    public static void ToGrayScale(Bitmap bmp)
    {
        for (int y = 0; y < bmp.Height; y++)
        for (int x = 0; x < bmp.Width; x++)
        {
            var c = bmp.GetPixel(x, y);
            var rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
            bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
        }
    }

    static void Main(string[] args)
    {
        var originalImage = ArrayFromImage(@"C:\Users\invar\Pictures\lena.bmp");
        const int widthСoefficient = 4;
        const int heightСoefficient = 4;
        
        var interpolated = originalImage.BicubicInterpolation(
            widthСoefficient * originalImage.GetLength(1),
            heightСoefficient * originalImage.GetLength(0));
        var outputPath = @"C:\Users\invar\Pictures\interpolated.bmp";
        WriteToImage(outputPath, interpolated);
    }
}