using System;
using Gazeus.DesafioMatch3.Views;

namespace Gazeus.DesafioMatch3.Controllers
{
    public class MainMenuController : IDisposable
    {
        public event Action PlayRequested;
        public event Action ExitRequested;

        private readonly MainMenuView view;

        public MainMenuController(MainMenuView view)
        {
            this.view = view;
            view.PlayButtonClicked += OnPlayClicked;
            view.ExitButtonClicked += OnExitClicked;
        }
        
        public void Dispose()
        {
            view.PlayButtonClicked -= OnPlayClicked;
            view.ExitButtonClicked -= OnExitClicked;
        }

        private void OnPlayClicked()
        {
            PlayRequested?.Invoke();
        }
        
        private void OnExitClicked()
        {
            ExitRequested?.Invoke();
        }
    }
}