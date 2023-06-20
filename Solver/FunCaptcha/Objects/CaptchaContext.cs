namespace FC_Solver.FunCaptcha.Objects;

public struct CaptchaContext
{
    public string GameVariant { get; set; }
    public byte[][] Images { get; set; }
    
    public byte[] ImageRaw { get; set; }

    public SolverContext? SolverContext { get; set; }
    
    public string Instructions { get; set; }
}