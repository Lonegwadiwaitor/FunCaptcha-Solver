using System.Drawing;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using FC_Solver.FunCaptcha.Core;
using FC_Solver.FunCaptcha.Objects;
using FC_Solver.Utility;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using RestSharp;
using Random = FC_Solver.Utility.Random;

namespace FC_Solver.FunCaptcha.Solver;

public partial class Solver
{
    public static async Task<FunCaptchaResult> Solve(IBrowserContext browserCtx, string blob, string captchaId, string session)
    {
        JObject? gcft = null;

        var page = await browserCtx.NewPageAsync();

        page.Response += async (_, response) =>
        {
            if (response.Url.Contains("gfct"))
            {
                gcft = JObject.Parse((await response.JsonAsync()).ToString() ?? string.Empty);
            }
        };

        var random = new System.Random(RandomNumberGenerator.GetInt32(0, 2147483647));
        
        var scores = new List<(int, float, byte[], string)>();

        // removed a few lines here because it contained some bypasses & BDA modifications of ours that arkose have yet to patch

        // might add them back, dunno

        var captchaFrame = (await GetCaptchaFrame(frame!))!;

        var inframe = (await captchaFrame.ContentFrameAsync())!;

        var btn = (await inframe.WaitForSelectorAsync("#home_children_button", new FrameWaitForSelectorOptions
        {
            Timeout = 2000
        }))!;

        await btn.ClickAsync();

        List<CaptchaBase> variants = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CaptchaBase)))
            .Select(Activator.CreateInstance)
            .Cast<CaptchaBase>()
            .ToList();

        if (!variants.Any(x => x.CanSolve(gameVariant!)))
        {
            // Not currently compatible!
            Console.WriteLine($"Incompatible variant: {gameVariant}");
            await page.CloseAsync();
            return FunCaptchaResult.Failed();
        }

        var solver = variants.FirstOrDefault(x => x.CanSolve(gameVariant!));

        for (int i = 0; i < gameData?["waves"]?.ToObject<int>(); i++)
        {
            Thread.Sleep(2000);

            var captchaImage = await GetCaptchaImage(inframe);

            var description = await inframe.WaitForSelectorAsync("#game_children_text > h2");

            var instructions = await description.InnerTextAsync();

            var captchaResult = solver?.Solve(new CaptchaContext
            {
                Instructions = instructions,
                GameVariant = gameVariant!,
                ImageRaw = captchaImage,
                Images = ImageProcessing
                    .GetImagesFromFunCaptchaImage((Bitmap)Image.FromStream(new MemoryStream(captchaImage)))
                    .ToArray(),
                SolverContext = new SolverContext { BrowserContext = browserCtx }
            });

            Console.WriteLine($"Answer: {captchaResult?.Answer} Score: {captchaResult?.Score}%");

            if (captchaResult?.Answer == -1)
            {
                // Failed to solve
                var img = await inframe?.WaitForSelectorAsync($"#image{RandomNumberGenerator.GetInt32(1, 6)} > a")!;

                await img?.ClickAsync()!;

                if (!Directory.Exists("Unsolved"))
                    Directory.CreateDirectory("Unsolved");

                if (!Directory.Exists(Path.Combine("Unsolved", gameVariant!)))
                    Directory.CreateDirectory(Path.Combine("Unsolved", gameVariant!));

                await File.WriteAllBytesAsync(Path.Combine("Unsolved", gameVariant!, $"{captchaId}_{i}.png"),
                    captchaImage);
            }
            else
            {
                var img = await inframe?.WaitForSelectorAsync($"#image{captchaResult?.Answer} > a")!;

                await img?.ClickAsync()!;
            }

            Console.WriteLine("Answer submitted.");
        }

        await page.WaitForFunctionAsync("window.successToken");
        var token = await page.EvaluateAsync<string>("window.successToken");

        return FunCaptchaResult.Success(token);
    }

    private static async Task<IElementHandle?> GetCaptchaFrame(IFrame funcaptchaFrame)
    {
        IElementHandle? cpframe;
        TimeSpan timeout = TimeSpan.FromSeconds(15);

        for (;;)
        {
            cpframe = await funcaptchaFrame.QuerySelectorAsync("#CaptchaFrame");

            if (cpframe != null)
                return cpframe;

            cpframe = await funcaptchaFrame.QuerySelectorAsync("#CaptchaFrame2");

            if (cpframe != null)
                return cpframe;

            timeout = timeout.Subtract(TimeSpan.FromMilliseconds(100));
            Thread.Sleep(100);

            if (timeout.TotalSeconds < 1)
                break;
        }

        return null;
    }

    public static async Task<byte[]> GetCaptchaImage(IFrame captchaFrame)
    {
        IElementHandle? cpframe;
        TimeSpan timeout = TimeSpan.FromSeconds(15);

        for (;;)
        {
            cpframe = await captchaFrame.QuerySelectorAsync("#game_challengeItem_image");

            if (cpframe != null)
                break;

            cpframe = await captchaFrame.Locator("#game_challengeItem_image").ElementHandleAsync();

            if (cpframe != null)
                break;

            timeout = timeout.Subtract(TimeSpan.FromMilliseconds(100));
            Thread.Sleep(100);

            if (timeout.TotalSeconds < 1)
                return null!;
        }

        var src = await cpframe.GetAttributeAsync("src");
        var b64 = src?.Split("base64,")[1];

        return Convert.FromBase64String(b64 ?? "");
    }

    
}