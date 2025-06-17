using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class MainMenuViewModel : IPageViewModel
    {

        #region Fields
        string IPageViewModel.Name => "MainMenu";
        private MainMenuModel _model { get; init; }
        public LoginWrapper LoginWrapper { get; init; }
        private IPageViewModel _pageViewModel { get; set; }
 
        #endregion

        #region Constructors
        public MainMenuViewModel(LoginWrapper loginWrapper) {
            _model = new();
            LoginWrapper = loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper));
            PageViewModel = new UsersViewTableViewModel(LoginWrapper);
        }
        public MainMenuViewModel() { }

        #endregion

        #region Public Properties/Commands
        public IPageViewModel PageViewModel
        {
            get { return _pageViewModel; }
            set { _pageViewModel = value; }
        }
        #endregion

        #region Private Helpers
        #endregion
    }
}
