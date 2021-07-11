namespace Nix.MVC.Views
{
    public interface IBehaviour
    {
        IView View { get; }
        void Start(IView view);
    }
}
