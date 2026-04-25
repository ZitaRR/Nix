using Nix.Shared;
using SkiaSharp;
using Svg.Skia;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Graphics.Internal;

internal class SvgConverter(IHttpClientFactory httpClientFactory) : ISvgConverter
{
    private const float MAX_W = 256;
    private const float MAX_H = 256;

    private readonly HttpClient client = httpClientFactory.CreateClient(ShlConstants.CLIENT);

    public async Task<byte[]> ToPngAsync(Uri uri)
    {
        using var stream = await client.GetStreamAsync(uri);
        var svg = new SKSvg();
        var picture = svg.Load(stream);
        var rect = picture.CullRect;

        var scale = Math.Min(MAX_W / rect.Width, MAX_H / rect.Height);
        var width = (int)(rect.Width * scale);
        var height = (int)(rect.Height * scale);

        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);

        canvas.Clear(SKColors.Transparent);
        canvas.Scale(scale);
        canvas.DrawPicture(picture);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
