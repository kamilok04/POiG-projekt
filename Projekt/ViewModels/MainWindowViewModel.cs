using Projekt.Miscellaneous;
using Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class MainWindowViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private ICommand _changePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        string IPageViewModel.Name => "MainWindow";



        #endregion
        #region Constructors

        public MainWindowViewModel()
        {

            LoginViewModel loginViewModel = new();
            loginViewModel.Authenticated += LoginViewModel_Authenticated;
            ChangeViewModel(loginViewModel);
        }

      







        #endregion
        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
                _changePageCommand ??= new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);

                return _changePageCommand;
            }
        }


        public List<IPageViewModel> PageViewModels
        {
            get => _pageViewModels ??= [];

        }



        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged(nameof(CurrentPageViewModel));
                }
            }
        }


        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

      

        private void LoginViewModel_Authenticated()
        {
            MainMenuViewModel mainmenu = new();

            ChangeViewModel(mainmenu);
        }
        #endregion
    }
}