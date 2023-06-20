// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using FC_Solver.Roblox;
using FC_Solver.Utility;
using Microsoft.Playwright;

Console.WriteLine("FunCaptcha Solving Utility v0.1.3c - by Lonegladiator");

Console.WriteLine("Loading libraries...");


FC_Solver.Machine_Learning.Tensorflow.Init();

var playwright = await Playwright.CreateAsync();

var chrome = await playwright.Firefox.LaunchAsync();

var (server, port, user, pass) = ("insert", "proxies", "here", "!");

var account = await AuthAPI.CreateAccount(Utility.Random.String(15), Utility.Random.String(15), await chrome.NewContextAsync(
    new BrowserNewContextOptions
    {
        Proxy = new Proxy {
            Server = $"https://{server}:{port}",
            Username = user,
            Password = pass
        }
    }));

Thread.Sleep(-1);