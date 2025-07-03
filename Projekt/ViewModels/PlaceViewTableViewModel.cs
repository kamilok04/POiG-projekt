using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class PlaceViewTableViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(PlaceViewTableViewModel);
     
        private PlaceViewTableModel? Model { get; init; }

        private DataTable? _places;

        public DataTable? Places
        {
            get => _places;
            private set
            {
                _places = value;
                OnPropertyChanged(nameof(Places));
            }
        }

        public PlaceViewTableViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false);
        }

        public PlaceViewTableViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            if(Model?.LoginWrapper != null)
            {
                Places = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(Model.DefaultQuery);
            }
        }
    }
}
