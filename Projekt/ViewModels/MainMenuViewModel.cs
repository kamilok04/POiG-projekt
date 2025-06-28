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
        private MainMenuModel? _model { get; init; }
    
        private ICommand? _changePageView;
        private ConditionalContentControlViewModel? _subView {  get; init; }

        public LoginWrapper? LoginWrapper
        {
            get => _model?.LoginWrapper;
        }

        #endregion

        #region Constructors
        public MainMenuViewModel(LoginWrapper loginWrapper)
        {
            _model = new(loginWrapper);
            CurrentPageViewModel = new CurrentProfileViewModel(_model.LoginWrapper);
          
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

        public ICommand LogoutCommand
        {
            get
            {
                return new RelayCommand(
                    param =>
                    {
                        Logout();
                    });
            }
        }
        #endregion

        #region Private Helpers
        private void Logout()
        {
            _model?.LoginWrapper?.Logout();
           
        }
        private async Task ChangePageView(object? param)
        {
            // Na pewno wolno ustawić taki widok?
            if(!await Authenticate()) return;
            if (param is string pageViewModelName && _model?.LoginWrapper != null)
                switch (pageViewModelName)
                {
                    case "UsersCreateViewModel":
                        // Prawdopobnie powoduje wyciek pamięci.
                        CurrentPageViewModel = new UsersCreateViewModel(_model.LoginWrapper);
                        break;
                    case "UsersEditViewModel":
                        CurrentPageViewModel = new UsersEditViewModel(_model.LoginWrapper);
                        break;
                    case "UsersDeleteViewModel":
                        CurrentPageViewModel = new UsersDeleteViewModel(_model.LoginWrapper);
                        break;

                    case "LessonsCreateViewModel":
                        CurrentPageViewModel = new LessonsCreateViewModel(_model.LoginWrapper);
                        break;
                    case "LessonsEditViewModel":
                        CurrentPageViewModel = new LessonsEditViewModel(_model.LoginWrapper);
                        break;
                    case "LessonsDeleteViewModel":
                        CurrentPageViewModel = new LessonsDeleteViewModel(_model.LoginWrapper);
                        break;

                    case "GroupCreateViewModel":
                        CurrentPageViewModel = new GroupCreateViewModel(_model.LoginWrapper);
                        break;
                    case "GroupEditViewModel":
                        CurrentPageViewModel = new GroupEditViewModel(_model.LoginWrapper);
                        break;
                    case "GroupDeleteViewModel":
                        CurrentPageViewModel = new GroupDeleteViewModel(_model.LoginWrapper);
                        break;

                    case "PlaceCreateViewModel":
                        CurrentPageViewModel = new PlaceCreateViewModel(_model.LoginWrapper);
                        break;
                    case "PlaceEditViewModel":
                        CurrentPageViewModel = new PlaceEditViewModel(_model.LoginWrapper);
                        break;
                    case "PlaceDeleteViewModel":
                        CurrentPageViewModel = new PlaceDeleteViewModel(_model.LoginWrapper);
                        break;

                    case "SubjectCreateViewModel":
                        CurrentPageViewModel = new SubjectCreateViewModel(_model.LoginWrapper);
                        break;
                    case "SubjectDeleteViewModel":
                        CurrentPageViewModel = new SubjectEditViewModel(_model.LoginWrapper);
                        break;
                    case "SubjectEditViewModel":
                        CurrentPageViewModel = new SubjectDeleteViewModel(_model.LoginWrapper);
                        break;


                }
        }

        private async Task<bool> Authenticate() 
        {
            return _model?.LoginWrapper != null && await _model.LoginWrapper.Authenticate();
        }
        #endregion
    }
}
