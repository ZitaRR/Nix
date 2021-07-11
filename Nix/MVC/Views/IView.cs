namespace Nix.MVC
{
    public interface IView
    {
        Controller Controller { get; }
        string Name { get; }
        IView Parent { get; }
        void Display();
    }
}
