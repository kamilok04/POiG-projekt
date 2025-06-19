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
    public class SubjectViewTableViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(SubjectViewTableViewModel);
        private SubjectViewTableModel Model { get; init; }

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
        public SubjectViewTableViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false);
        }
        public SubjectViewTableViewModel() { } //for designer only


        private async Task GetDataAsync()
        {
            Data = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync("SELECT * FROM .....");
        }
    }
}
