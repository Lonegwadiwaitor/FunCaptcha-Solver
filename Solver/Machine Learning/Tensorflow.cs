using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace FC_Solver.Machine_Learning;

public class Tensorflow
{
    private static IHost? host;

    private class ModelInput
    {
        [LoadColumn(0)] [ColumnName(@"Label")] public string Label { get; set; }

        [LoadColumn(1)]
        [ColumnName(@"ImageSource")]
        public byte[] ImageSource { get; set; }
    }

    public class ModelOutput
    {
        [ColumnName(@"Label")] public uint Label { get; set; }

        [ColumnName(@"ImageSource")] public byte[] ImageSource { get; set; }

        [ColumnName(@"PredictedLabel")] public string PredictedLabel { get; set; }

        [ColumnName(@"Score")] public float[] Score { get; set; }

        public float GetScore(string obj)
        {
            var names = GetSlotNames("Score");

            for (var index = 0; index < names.Length; index++)
            {
                var name = names[index];

                if (name == obj)
                {
                    return Score[index];
                }
            }

            return 0;
        }
    }

    private static string[] GetSlotNames(string name)
    {
        var predictionEngine = host?.Services.GetService<PredictionEnginePool<ModelInput, ModelOutput>>();
        DataViewSchema.Column? column = predictionEngine?.GetPredictionEngine().OutputSchema.GetColumnOrNull(name);

        VBuffer<ReadOnlyMemory<char>> slotNames = new VBuffer<ReadOnlyMemory<char>>();
        column?.GetSlotNames(ref slotNames);
        string[]? names = new string[slotNames.Length];
        int num = 0;

        foreach (var denseValue in slotNames.DenseValues())
        {
            names[num++] = denseValue.ToString();
        }

        return names;
    }

    public static void Init()
    {
        Console.WriteLine("Loading Tensorflow . . .");

        host ??= Host.CreateDefaultBuilder().ConfigureServices((_, services) =>
        {
            services.AddPredictionEnginePool<ModelInput, ModelOutput>().FromFile($"{Directory.GetCurrentDirectory()}\\ML Models\\model.zip");
        }).Build();
        
        var testPrediction = Predict(new HttpClient().GetByteArrayAsync("https://i.imgur.com/rhcPokC.png").GetAwaiter().GetResult());
        
        Console.WriteLine($"Test prediction: {testPrediction.PredictedLabel} ({testPrediction.GetScore(testPrediction.PredictedLabel)}%)");
    }

    public static ModelOutput Predict(byte[] image)
    {
        var predictionEngine = host?.Services.GetService<PredictionEnginePool<ModelInput, ModelOutput>>();

        var model = new ModelInput
        {
            ImageSource = image
        };

        return predictionEngine.Predict(model);
    }
}