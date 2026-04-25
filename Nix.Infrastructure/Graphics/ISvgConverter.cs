using System;
using System.IO;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Graphics;

public interface ISvgConverter
{
    public Task<byte[]> ToPngAsync(Uri uri);
}
