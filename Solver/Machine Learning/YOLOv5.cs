using System.Drawing;
using Tensorflow.Keras;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models.Abstract;

namespace FC_Solver.Machine_Learning;

public class YOLOv5
{
    public static List<YoloPrediction> Predict<TYoloModel>(Image image, string weights) where TYoloModel : YoloModel
    {
        using var scorer = new YoloScorer<TYoloModel>(weights);

        List<YoloPrediction> predictions = scorer.Predict(image);

        return predictions;
    }
}