using Nix.Models;

namespace Nix.Resources
{
    public interface IRegister
    {
        void RegisterGuild(NixGuild guild);
        void UnRegisterGuild(NixGuild guild);
        void RegisterUser(NixUser user);
        void UnregisterUser(NixUser user);
        void RegisterChannel(NixChannel channel);
        void UnregisterChannel(NixChannel channel);
    }
}
