using Nix.Infrastructure.Shl.Settings.Internal.Private;
using System.Threading.Tasks;

namespace Nix.Infrastructure.Shl.Settings.Internal;

internal interface ISettingsService
{
    public Task<SettingsDto> GetConfigAsync();
}
