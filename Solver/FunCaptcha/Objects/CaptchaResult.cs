namespace FC_Solver.FunCaptcha.Objects;

public struct CaptchaResult
{
    public CaptchaResult(int answer, byte[] image, float score, string prediction)
    {
        Answer = answer;
        Image = image;
        Score = score;
        Prediction = prediction;
    }

    public int Answer { get; set; }
    public byte[] Image { get; set; }

    public float Score { get; set; }
    
    public string Prediction { get; set; }
}