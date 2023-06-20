using System.Drawing;
using Yolov5Net.Scorer;

namespace FC_Solver.Machine_Learning.Utility;

public static class YoloPredictionTools
{
    public static YoloPrediction? GetPredictionByRect(this List<YoloPrediction?> predictions, RectangleF rect)
    {
        return predictions.FirstOrDefault(yoloPrediction => yoloPrediction != null && yoloPrediction.Rectangle == rect);
    }
}