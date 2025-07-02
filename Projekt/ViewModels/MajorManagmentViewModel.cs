using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class MajorManagmentViewModel : ObservableObject, IPageViewModel, ITable
    {
        string ITable.DefaultQuery => "SELECT dk.nazwa AS kierunek, w.nazwa AS wydział FROM kierunek k JOIN dane_kierunku dk ON k.id_danych_kierunku = dk.id JOIN wydzial w ON k.id_wydzialu = w.nazwa_krotka;";
        string ITable.TableName => "kierunek";
        Dictionary<string, object>? ITable.DefaultParameters => null;
        string IPageViewModel.Name => nameof(MajorManagmentViewModel);

        private UsersEditModel Model { get; init; }

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

        public MajorManagmentViewModel(LoginWrapper loginWrapper)
        {

            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false); ;
        }

        public MajorManagmentViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            TableData = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(((ITable)this).DefaultQuery);
        }
    }
}
