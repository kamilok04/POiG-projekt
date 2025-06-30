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
    public class GroupViewTableViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(GroupViewTableViewModel);
        private GroupViewTableModel? Model { get; init; }
        private DataTable? _data;
        public DataTable? Data
        {
            get => _data;
            private set
            {
                _data = value;
                OnPropertyChanged(nameof(Data));
            }
        }
        public GroupViewTableViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false);
        }
        public GroupViewTableViewModel() { } //for designer only
        private async Task GetDataAsync()
        {
            if (Model?.LoginWrapper != null)
            {
                Data = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync("SELECT * FROM grupa");
            }
        }
    }
}
