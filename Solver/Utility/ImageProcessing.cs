using System.Drawing;
using System.Drawing.Imaging;

namespace FC_Solver.Utility;

public class ImageProcessing
{
    public static List<byte[]> GetImagesFromFunCaptchaImage(Bitmap bitmap)
    {
        List<Bitmap> images = new List<Bitmap>();

        Rectangle[] cropRectangles =
        {
            new(3, 0, 94, 96),
            new(104, 2, 94, 96),
            new(203, 0, 94, 96),
            new(0, 102, 94, 96),
            new(102, 100, 94, 96),
            new(198, 99, 94, 96),
        };


        foreach (var rectangle in cropRectangles)
        {
            var target = new Bitmap(rectangle.Width, rectangle.Height);

            using var g = Graphics.FromImage(target);
            g.DrawImage(bitmap, new Rectangle(0, 0, 94, 96),
                rectangle,
                GraphicsUnit.Pixel);
            images.Add(target);
        }

        return images.Select(x => {
            var MS = new MemoryStream();
            
            x.Save(MS, ImageFormat.Png);
            return MS.ToArray();
        }).ToList();
    }
}