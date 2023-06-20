using System.Security.Cryptography;

namespace FC_Solver.Utility;

public static class Random
{
    public static string String(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[RandomNumberGenerator.GetInt32(0, s.Length)]).ToArray());
    }
}