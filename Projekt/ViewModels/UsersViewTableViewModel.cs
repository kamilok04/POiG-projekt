using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class UsersViewTableViewModel : IPageViewModel
    {
        string IPageViewModel.Name => nameof(UsersViewTableViewModel);
        public LoginWrapper LoginWrapper { get; init; }
        private UsersViewTableModel Model { get; init; }
        public List<Dictionary<string, object>> Data { get; set; }
        public UsersViewTableViewModel() { }
        public UsersViewTableViewModel(LoginWrapper loginWrapper) {
            LoginWrapper = loginWrapper;
            Model = new(LoginWrapper);
            GetData();
        }

        public async void GetData()
        {
            await Model.RetrieveDefaultQuery();
        }

        
    }
}
