using FC_Solver.FunCaptcha.Objects;

namespace FC_Solver.FunCaptcha.Core;

public abstract class CaptchaBase
{
    public virtual bool CanSolve(string gameVariant) { throw new NotImplementedException(); }

    public virtual CaptchaResult Solve(CaptchaContext context) { return default; }
}