namespace FC_Solver.FunCaptcha.Objects;

public enum FunCaptchaResultEnum
{
    Success,
    Failed,
}

public class FunCaptchaResult
{
    public FunCaptchaResultEnum CaptchaResult;
    public string? Token;

    public static FunCaptchaResult Success(string token) => new()
    {
        CaptchaResult = FunCaptchaResultEnum.Success,
        Token = token,
    };

    public static FunCaptchaResult Failed() => new()
    {
        CaptchaResult = FunCaptchaResultEnum.Failed,
        Token = null
    };
}