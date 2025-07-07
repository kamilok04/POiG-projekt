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
    /// <summary>
    /// ViewModel umożliwiający dynamiczne przełączanie widoków stron w aplikacji.
    /// </summary>
    public class ConditionalContentControlViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private ICommand? _changePageCommand;
        private IPageViewModel? _currentPageViewModel;
        private List<IPageViewModel>? _pageViewModels;

        /// <summary>
        /// Nazwa widoku strony.
        /// </summary>
        string IPageViewModel.Name => "ConditionalContentControl";

        /// <summary>
        /// Konstruktor domyślny.
        /// </summary>
        public ConditionalContentControlViewModel()
        {
        }

        #endregion

        #region Properties / Commands

        /// <summary>
        /// Komenda do zmiany aktualnego widoku strony.
        /// </summary>
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

        /// <summary>
        /// Lista dostępnych widoków stron.
        /// </summary>
        public List<IPageViewModel> PageViewModels
        {
            get => _pageViewModels ??= [];
        }

        /// <summary>
        /// Aktualnie wybrany widok strony.
        /// </summary>
        public IPageViewModel? CurrentPageViewModel
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

        /// <summary>
        /// Zmienia aktualny widok strony na podany.
        /// </summary>
        /// <param name="viewModel">Nowy widok strony.</param>
        public void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }

        #endregion
    }
}