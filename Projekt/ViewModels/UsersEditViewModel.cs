using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class UsersEditViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(UsersEditViewModel);
     
        private UsersViewTableModel Model { get; init; }

        private DataTable? _data;
        public DataTable? TableData
        {
            get => _data;
            private set
            {
                _data = value;
                OnPropertyChanged(nameof(TableData));
            }
        }

        public UsersEditViewModel(LoginWrapper loginWrapper)
        {
   
            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false); ;
        }

        public UsersEditViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            TableData = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync("SELECT * FROM dane_uzytkownika");
        }

    }
}
