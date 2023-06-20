using System.Drawing;

namespace FC_Solver.Machine_Learning.Utility;

public static class BoundingBoxTools
{
    public static bool IsOverlapping(RectangleF rectA, RectangleF rectB)
    {
        return Math.Max(rectA.Left, rectB.Left) < Math.Min(rectA.Right, rectB.Right) &&
               Math.Max(rectA.Top, rectB.Top) < Math.Min(rectA.Bottom, rectB.Bottom);
    }

    public static Point GetCenter(RectangleF rect)
    {
        return new Point((int)(rect.Left + rect.Width / 2),
            (int)(rect.Top + rect.Height / 2));
    }
    
    private static int Distance2D(int x1, int y1, int x2, int y2)
    {

        int result = 0;
        double part1 = Math.Pow((x2 - x1), 2);

        double part2 = Math.Pow((y2 - y1), 2);
        double underRadical = part1 + part2;
        result = (int)Math.Sqrt(underRadical);

        return result;
    }

    public static RectangleF GetNearest(RectangleF rect, RectangleF[] rectangles)
    {
        RectangleF nearest = default;
        int nearestDist = int.MaxValue;
        foreach (var rectangleF in rectangles)
        {
            if (rectangleF == rect)
                continue; // prolly forgot to remove the original from the array

            //var center = GetCenter(rectangleF);

            var dist = Distance2D((int)rect.X, (int)rect.Y, (int)rectangleF.X, (int)rectangleF.Y);

            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = rectangleF;
            }
        }

        return nearest;
    }
}