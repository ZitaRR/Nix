using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Nix.Rendering;

public interface IRenderEngine
{
    Task<Stream> RenderSingleAsync(string html, string css, string capture, string identifier = "");

    Task<IEnumerable<Stream>> RenderManyAsync(string html, string css, string capture, string identifier = "");
}
