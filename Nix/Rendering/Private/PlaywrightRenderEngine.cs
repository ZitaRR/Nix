using Microsoft.Playwright;
using Nix.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Nix.Rendering.Private;

internal class PlaywrightRenderEngine : CacheBase<string, IEnumerable<byte[]>>, IRenderEngine
{
    private static readonly IPlaywright playwright;
    private static readonly IBrowser browser;
    private static readonly IPage page;

    static PlaywrightRenderEngine()
    {
        playwright = Playwright.CreateAsync().GetAwaiter().GetResult();
        browser = playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true,
            Args = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? ["--no-sandbox"] : []
        }).GetAwaiter().GetResult();
        page = browser.NewPageAsync().GetAwaiter().GetResult();
    }

    public async Task<Stream> RenderSingleAsync(string html, string css, string capture, string identifier = "")
    {
        if (!string.IsNullOrEmpty(identifier) && TryGet(identifier, out var value))
        {
            return new MemoryStream(value.First());
        }

        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        await page.AddStyleTagAsync(new PageAddStyleTagOptions { Content = css });
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var elements = await page.QuerySelectorAsync(capture);
        var bytes = await elements.ScreenshotAsync(new ElementHandleScreenshotOptions { OmitBackground = true });

        if (!string.IsNullOrEmpty(identifier))
        {
            Set(identifier, [bytes]);
        }

        return new MemoryStream(bytes);
    }

    public async Task<IEnumerable<Stream>> RenderManyAsync(string html, string css, string capture, string identifier = "")
    {
        if (!string.IsNullOrEmpty(identifier) && TryGet(identifier, out var value))
        {
            return value.Select(b => new MemoryStream(b));
        }

        await page.SetContentAsync(html);
        await page.AddStyleTagAsync(new PageAddStyleTagOptions { Content = css });
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var elements = await page.QuerySelectorAllAsync(capture);
        var tasks = elements.Select(e => e.ScreenshotAsync(new ElementHandleScreenshotOptions { OmitBackground = true }));
        var bytes = await Task.WhenAll(tasks);

        if (!string.IsNullOrEmpty(identifier))
        {
            Set(identifier, bytes);
        }

        return bytes.Select(b => new MemoryStream(b));
    }
}
