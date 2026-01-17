using Microsoft.Playwright;
using Spider.Common.Services.Baidu;

namespace Spider.Common.Services;

public abstract class PlaywrightService
{
    public IPlaywright _playwright;
    public IBrowser _browser;
    public IPage _page;
    


    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        _browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            Timeout = 10000,
            ExecutablePath = edgePath // 使用系统浏览器
        });
        _page = await _browser.NewPageAsync();
    }

}