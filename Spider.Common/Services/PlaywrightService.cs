using Microsoft.Playwright;
using Spider.Common.Services.Baidu;

namespace Spider.Common.Services;

public abstract class PlaywrightService
{
    private IPlaywright _playwright;
    public IBrowser Browser;
    public IBrowserContext Context;
    public IPage Page;

    public virtual string StorageStatePath => "./StorageState.json";

    public async Task<bool> InitializeAsync(bool useStorageState = false)
    {
        _playwright = await Playwright.CreateAsync();
        string edgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
        Browser = await _playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            Timeout = 10000,
            ExecutablePath = edgePath // 使用系统浏览器
        });

        var contextOptions = new BrowserNewContextOptions();

        if (useStorageState && File.Exists(StorageStatePath))
        {
            contextOptions.StorageStatePath = StorageStatePath;
        }

        Context = await Browser.NewContextAsync(contextOptions);
        Page = await Context.NewPageAsync();
        return useStorageState;
    }

    


}