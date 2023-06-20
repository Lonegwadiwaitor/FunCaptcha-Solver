using System.Net;
using System.Security.Cryptography;
using FC_Solver.FunCaptcha.Objects;
using FC_Solver.FunCaptcha.Solver;
using Microsoft.Playwright;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace FC_Solver.Roblox;

public class AuthAPI
{
    private static RestClient _restClient;

    public static async Task<(bool, string?)> CreateAccount(string username, string password, IBrowserContext browserContext)
    {
        _restClient = new RestClient(new RestClientOptions {
        });
        
        var authReqeust = new RestRequest("https://auth.roblox.com/v2/signup", Method.Post);

        var resp1 = await _restClient.ExecuteAsync(authReqeust);
        
        var xref = (resp1).Headers?.FirstOrDefault(x => x.Name == "x-csrf-token")?.Value as string;
        
        {
            while (true) {
                var validateRequest = new RestRequest("https://auth.roblox.com/v1/usernames/validate", Method.Post);

                validateRequest.AddJsonBody(new
                    { username, context = "Signup", birthday = "1986-05-06T23:00:00.000Z" });

                validateRequest.AddHeader("X-CSRF-TOKEN", xref);

                var Vresponse = (await _restClient.ExecuteAsync(validateRequest)).Content;

                var json = JObject.Parse(Vresponse);

                if (json["code"]?.ToObject<int>() == 0) break;

                username += Utility.Random.String(1);
            }
        }
        
        authReqeust.AddJsonBody(new {
            username, password, birthday = 1, gender = 1, isTosAgreementBoxChecked = true,
            context = "Rollercoaster2SignupForm", captchaProvider = "PROVIDER_ARKOSE_LABS"
        });

        authReqeust.AddHeader("X-CSRF-TOKEN", xref);

        var resp = await _restClient.ExecuteAsync(authReqeust);

        var gjson = JObject.Parse(resp.Content);

        var captchaInfoI = gjson["failureDetails"];
        var captchaInfo = captchaInfoI?[0]?["fieldData"]?.ToString().Split(",");
        var _captchaId = captchaInfo?[0];
        var captchaBlob = captchaInfo?[1];

        if (captchaBlob is null || _captchaId is null)
            return (false, null);

        FunCaptchaResult captcha = await Solver.Solve(browserContext, captchaBlob, _captchaId, "");

        if (captcha.CaptchaResult == FunCaptchaResultEnum.Success)
        {
            authReqeust = new RestRequest("https://auth.roblox.com/v2/signup", Method.Post);

            authReqeust.AddHeader("X-CSRF-TOKEN", xref);

            authReqeust.AddJsonBody(new
            {
                username, password, birthday = "1986-05-06T23:00:00.000Z", gender = 1,
                isTosAgreementBoxChecked = true, context = "Rollercoaster2SignupForm",
                captchaProvider = "PROVIDER_ARKOSE_LABS", captchaId = _captchaId, captchaToken = captcha.Token
            });

            var resp3 = await _restClient.ExecuteAsync(authReqeust);

            if (resp3.Cookies!.Any(x => x.Name.Contains("ROBLOSECURITY")))
            {
                Console.WriteLine("wow it work");

                // store your account / cookie
            }
        }
        
        var (server, port, user, pass) = ("dont forget", "to", "insert", "your proxies!");

        return await CreateAccount(username, password, await browserContext.Browser!.NewContextAsync(new BrowserNewContextOptions
        {
            Proxy = new Proxy {
                Server = $"http://{server}:{port}",
                Username = user,
                Password = pass
            }
        }));
    }
}