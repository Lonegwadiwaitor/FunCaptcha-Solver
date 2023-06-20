using FC_Solver.FunCaptcha.Core;
using FC_Solver.FunCaptcha.Objects;

namespace FC_Solver.FunCaptcha.GameVariants;

public class GenericML : CaptchaBase
{
    private readonly string[] _compatibleVariants = /* this handled a majority of arkose's shittier captchas */
    {
        "alien", "anchor", "ant", "apple", "aquarium", "ball", "banana", "bat", "bear", "bee", "bike", "boat", "bread",
        "burger", "butterfly", "cactus", "calculator", "camel", "camera", "car", "cat", "cheese", "chicken", "computer",
        "controller", "couch", "cow", "crab", "crown", "deer", "dinosaur", "dog", "dolphin", "donut", "duck",
        "elephant", "fan", "fax", "fence", "fire", "flower", "fridge", "frog", "gazelle", "giraffe", "goat", "grapes",
        "guitar", "helicopter", "helmet", "horse", "hotdog", "ice cream", "kangaroo", "key", "koala", "ladybug", "lamp",
        "lion", "lobster", "lock", "money", "monkey", "mouse", "mushroom", "octopus", "owl", "panda", "pants", "parrot",
        "pencil", "pig", "pineapple", "pizza", "plane", "printer", "rabbit", "ram", "rhino", "ring", "scissors", "seal",
        "shark", "sheep", "shirt", "shoe", "snail", "snake", "snowman", "sock", "spaceship", "stapler", "starfish",
        "sunglasses", "toaster", "toater", "toilet", "train", "trophy", "turtle", "umbrella", "watch", "watermelon",
        "zebra"
    };

    public override bool CanSolve(string gameVariant)
        => _compatibleVariants.Contains(gameVariant);

    public override CaptchaResult Solve(CaptchaContext context) // TODO: train data for obj detection via YOLO
    {
        var scores = new List<(int, float, byte[], string)>();

        for (var index = 0; index < context.Images.Length; index++)
        {
            var contextImage = context.Images[index];
            var prediction = Machine_Learning.Tensorflow.Predict(contextImage);

            var score = prediction.GetScore(context.GameVariant);

            scores.Add((index + 1, score, contextImage, prediction.PredictedLabel));
        }

        scores = scores.OrderByDescending(x => x.Item2).ToList();

        var mostLikely = scores.FirstOrDefault();

        return new CaptchaResult(mostLikely.Item1, mostLikely.Item3, mostLikely.Item2, mostLikely.Item4);
    }
}