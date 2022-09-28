namespace ConsoleApp1;

public static class InterpolationExtension
{
    public static float[,] BicubicInterpolation(this float[,] data, int outWidth, int outHeight)
    {
        // разбиваем на чанки
        int rowsPerChunk = 6000 / outWidth; 
        if (rowsPerChunk == 0)
        {
            rowsPerChunk = 1;
        }

        int chunkCount = (outHeight / rowsPerChunk) 
                         + (outHeight % rowsPerChunk != 0 ? 1 : 0);

        var width = data.GetLength(1);
        var height = data.GetLength(0);
        var ret = new float[outHeight, outWidth];

        // src https://www.paulinternet.nl/?page=bicubic
        float InterpolateCubic(float v0, float v1, float v2, float v3, float fraction)
        {
            return (float) (v1 + 0.5 * fraction*(v2 - v0 + fraction*(2.0*v0 - 5.0*v1 + 4.0*v2 - v3 + fraction*(3.0*(v1 - v2) + v3 - v0))));
        }
        
        // интерполируем по чанкам сверху вниз, слева направо
        Parallel.For(0, chunkCount, (chunkNumber) =>
        {
            int yStart = chunkNumber * rowsPerChunk;
            int yStop = yStart + rowsPerChunk;
            if (yStop > outHeight)
            {
                yStop = outHeight;
            }

            for (int y = yStart; y < yStop; ++y)
            {
                float yLocationFraction = y / (float)outHeight;
                var yFloatPosition = height * yLocationFraction;
                
                var y2 = (int)yFloatPosition;
                var yFraction = yFloatPosition - y2;
                var y1 = y2 > 0 ? y2 - 1 : y2;
                var y3 = y2 < height - 1 ? y2 + 1 : y2;
                var y4 = y3 < height - 1 ? y3 + 1 : y3;
                
                for (int x = 0; x < outWidth; ++x)
                {
                    float xLocationFraction = x / (float)outWidth;
                    var xFloatPosition = width * xLocationFraction;
                    
                    var x2 = (int)xFloatPosition;
                    var xFraction = xFloatPosition - x2;
                    var x1 = x2 > 0 ? x2 - 1 : x2;
                    var x3 = x2 < width - 1 ? x2 + 1 : x2;
                    var x4 = x3 < width - 1 ? x3 + 1 : x3;
                    
                    float yValue1 = InterpolateCubic(
                        data[y1, x1], data[y1, x2], data[y1, x3], data[y1, x4], xFraction);
                    float yValue2 = InterpolateCubic(
                        data[y2, x1], data[y2, x2], data[y2, x3], data[y2, x4], xFraction);
                    float yValue3 = InterpolateCubic(
                        data[y3, x1], data[y3, x2], data[y3, x3], data[y3, x4], xFraction);
                    float yValue4 = InterpolateCubic(
                        data[y4, x1], data[y4, x2], data[y4, x3], data[y4, x4], xFraction);
                    
                    ret[x, y] = InterpolateCubic(
                        yValue1, yValue2, yValue3, yValue4, yFraction);
                }
            }
        });

        return ret;
    }
}