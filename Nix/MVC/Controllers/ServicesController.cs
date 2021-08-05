using Nix.MVC.Views;
using Nix.Resources.Discord;

namespace Nix.MVC
{
    public sealed class ServicesController : Controller
    {
        private readonly LavalinkService lavalink;
        private readonly MinecraftService minecraft;
        private LogView lavalinkView;
        private LogView minecraftView;

        public ServicesController(LavalinkService lavalink, MinecraftService minecraft)
        {
            this.lavalink = lavalink;
            this.minecraft = minecraft;

            Menu = new View(this)
            {
                Name = "Services",
                Parent = CurrentView,
                Behaviour = new Navigation
                {
                    Options = new()
                    {
                        new() { Name = "Lavalink", View = Lavalink },
                        new() { Name = "Minecraft", View = Minecraft },
                    }
                }
            };
        }

        private IView Lavalink()
        {
            if (lavalinkView is null)
            {
                lavalinkView = new(this, lavalink.Logger)
                {
                    Name = "Lavalink",
                    Parent = Menu,
                };
            }
            return lavalinkView;
        }

        private IView Minecraft()
        {
            if (minecraftView is null)
            {
                minecraftView = new(this, minecraft.Logger)
                {
                    Name = "Minecraft",
                    Parent = Menu,
                    Behaviour = new TextInput
                    {
                        Repeat = true,
                        Action = input =>
                        {
                            if (string.IsNullOrWhiteSpace(input.UserInput))
                            {
                                input.Cancel();
                                return;
                            }
                            minecraft.Write(input.UserInput);
                            input.UserInput = "";
                        }
                    }
                };
            }
            return minecraftView;
        }
    }
}
