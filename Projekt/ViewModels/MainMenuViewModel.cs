using Mysqlx.Session;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class MainMenuViewModel : ConditionalContentControlViewModel, IPageViewModel
    {
        #region Fields
        string IPageViewModel.Name => "MainMenu";
        private MainMenuModel _model { get; init; }
    
        private ICommand? _changePageView;

        public LoginWrapper LoginWrapper
        {
            get => _model.LoginWrapper;
  
        }

        #endregion

        #region Constructors
        public MainMenuViewModel(LoginWrapper loginWrapper)
        {
            _model = new(loginWrapper);
            CurrentPageViewModel = new UsersViewTableViewModel(_model.LoginWrapper);
        }
        public MainMenuViewModel() { }

        #endregion

        #region Public Properties/Commands


        public ICommand ChangeCurrentPageViewCommand
        {
            get
            {
                _changePageView ??= new RelayCommand(
                    async param => await ChangePageView(param) 
                );
                return _changePageView;
            }
        }
        #endregion

        #region Private Helpers
        private async Task ChangePageView(object? param)
        {
            // Na pewno wolno ustawić taki widok?
            if(!await Authenticate()) return;
            if (param is string pageViewModelName)
                switch (pageViewModelName)
                {
                    case "UsersCreateViewModel":
                        // Prawdopobnie powoduje wyciek pamięci.
                        CurrentPageViewModel = new UsersCreateViewModel(_model.LoginWrapper);
                        break;
                }
        }

        private async Task<bool> Authenticate() 
        {
            return await _model.LoginWrapper.Authenticate();
        }
        #endregion
    }
}
