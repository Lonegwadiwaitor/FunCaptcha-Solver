using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models.Abstract;

namespace FC_Solver.Machine_Learning.Models;

public class YoloModelDart : YoloModel
{
    public override int Width { get; set; } = 640;

    public override int Height { get; set; } = 640;

    public override int Depth { get; set; } = 3;

    public override int Dimensions { get; set; } = 14;


    public override int[] Strides { get; set; } = new[]
    {
        8,
        16,
        32
    };

    public override int[][][] Anchors { get; set; } = new int[3][][]
    {
        new int[3][]
        {
            new int[2]{ 10, 13 },
            new int[2]{ 16, 30 },
            new int[2]{ 33, 23 }
        },
        new int[3][]
        {
            new int[2]{ 30, 61 },
            new int[2]{ 62, 45 },
            new int[2]{ 59, 119 }
        },
        new int[3][]
        {
            new int[2]{ 116, 90 },
            new int[2]{ 156, 198 },
            new int[2]{ 373, 326 }
        }
    };

    public override int[] Shapes { get; set; } = new int[4]
    {
        1,
        3,
        640,
        640
    };

    public override float Confidence { get; set; } = 0.2f;

    public override float MulConfidence { get; set; } = 0.25f;

    public override float Overlap { get; set; } = 0.45f;

    public override string[] Outputs { get; set; } = new string[1]
    {
        "output0"
    };
    public override List<YoloLabel> Labels { get; set; } = new List<YoloLabel>()
    {
        new() { Id = 0, Name = "2" },
        new() { Id = 1, Name = "3" },
        new() { Id = 2, Name = "4" },
        new() { Id = 3, Name = "5" },
        new() { Id = 4, Name = "6" },
        new() { Id = 5, Name = "7" },
        new() { Id = 6, Name = "8" },
        new() { Id = 7, Name = "9" },
        new() { Id = 8, Name = "dart" },
    };
    public override bool UseDetect { get; set; } = true;
}