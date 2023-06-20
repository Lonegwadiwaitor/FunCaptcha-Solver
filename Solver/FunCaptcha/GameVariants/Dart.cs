using System.Drawing;
using System.Security.Cryptography;
using FC_Solver.FunCaptcha.Core;
using FC_Solver.FunCaptcha.Objects;
using FC_Solver.Machine_Learning;
using FC_Solver.Machine_Learning.Models;
using FC_Solver.Machine_Learning.Utility;

namespace FC_Solver.FunCaptcha.GameVariants;

public class Dart : CaptchaBase
{
    public override bool CanSolve(string gameVariant)
    {
        return gameVariant is "dart01" or "dart02" or "dart03" or "dart04";
    }

    private (int, byte[], float) GetDartAnswer(CaptchaContext context)
    {
        var weights = Path.Combine(Directory.GetCurrentDirectory(), "ML Models", "dart.onnx");

        var number = context.Instructions.Split(' ')[^1];
        
        var officialTotal = int.Parse(number);

        for (var index = 0; index < context.Images.Length; index++)
        {
            var captchaImage = context.Images[index];
            var image = Image.FromStream(new MemoryStream(captchaImage));

            var predictions = YOLOv5.Predict<YoloModelDart>(image, weights);

            var darts = predictions.Where(x => x.Label.Name == "dart").ToList();
            var numbers = predictions.Where(x => x.Label.Name != "dart").ToList();

            int imageTotal = darts
                .Select(dart => BoundingBoxTools.GetNearest(dart.Rectangle, numbers.Select(x => x.Rectangle).ToArray()))
                .Select(nearest => int.Parse(numbers.GetPredictionByRect(nearest)?.Label.Name)).Sum();

            float score = predictions.Average(prediction => prediction.Score); // how sure the AI was

            if (imageTotal == officialTotal)
                return (index+1, captchaImage, score); // found our answer
        }

        return (-1, Array.Empty<byte>(), 0); // couldn't find it
    }

    public override CaptchaResult Solve(CaptchaContext context)
    {
        var (answer, image, score) = GetDartAnswer(context);
        
        return new CaptchaResult(answer, image, score, string.Empty);
    }
}