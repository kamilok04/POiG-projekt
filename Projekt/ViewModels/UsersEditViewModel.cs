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
using System.Windows.Controls;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class UsersEditViewModel : TableViewModel, IPageViewModel, ITable
    {
        #region Interface Implementations
        string IPageViewModel.Name => nameof(UsersEditViewModel);
        string ITable.DefaultQuery => "SELECT * FROM dane_uzytkownika";
        string ITable.TableName => "dane_uzytkownika";
        Dictionary<string, object>? ITable.DefaultParameters => null;

        #endregion

        #region Private Fields
        private UsersEditModel Model { get; init; }

        private DataTable? _data;

   
        #endregion
        #region Public Members / Commands



      


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

        private void ActiveCell_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
          
        }

        public UsersEditViewModel() { } //for designer only
        #endregion
        #region Private Methods

        private async Task GetDataAsync()
        {
            TableData = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(((ITable)this).DefaultQuery);

        }

       

        #endregion

        public ITable ToITable() => this;

     }
}
