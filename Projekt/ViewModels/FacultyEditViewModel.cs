using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class FacultyEditViewModel : TableViewModel, IPageViewModel, ITable
    {
        #region Interface Implementations
        string IPageViewModel.Name => nameof(FacultyEditViewModel);
        string ITable.DefaultQuery => "SELECT nazwa Nazwa, nazwa_krotka Skrót FROM wydzial";
        string ITable.TableName => "wydzial";
        Dictionary<string, object>? ITable.DefaultParameters => null;



        #endregion

        #region Private Fields
        private FacultyEditModel Model { get; init; }
        private bool ChangesPending = false; 
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



        public FacultyEditViewModel(LoginWrapper loginWrapper)
        {

            Model = new(loginWrapper);
            RowKey = "Skrót"; // NAzwa widoczna dla użytkownika, NIE kolumna z bazy
            GetDataAsync().ConfigureAwait(false); ;
        }

        private ICommand? _tableSaveCommand;
        public override ICommand TableSaveCommand
        {
            get => _tableSaveCommand ??= new RelayCommand(
                async param => { await Model.CommitTransaction(); await GetDataAsync(); });

        }

        private ICommand? _tableCancelCommand;
        public override ICommand TableCancelCommand
        {
            get => _tableCancelCommand ??= new RelayCommand(
                param => GetDataAsync().Wait());
        }


        private ICommand _tableCreateCommand;
        public override ICommand TableCreateCommand
        {
            get => _tableCreateCommand ??= new RelayCommand(
                param => SendToCreator());

        }

        public FacultyEditViewModel() { } //for designer only

        private void SendToCreator()
        {

            var mwwm = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            if (mwwm != null)
            {
                mwwm.SetMainMenuItem(new FacultyCreateViewModel(Model.LoginWrapper));
            }
        }
        #endregion
        #region Private Methods



        #endregion

        #region abstract stuff
        public override async Task GetDataAsync()
        {
            TableData = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(((ITable)this).DefaultQuery);

        }



        public override void CreateTransactionCommand(string? columnName, string? RowKey, string? oldValue, string? newValue)
        {
            Model.CreateTransactionCommand(columnName, RowKey, oldValue, newValue);
            ChangesPending = true;
        }

        public override bool ConfirmExit()
        {
            if (!ChangesPending) return true; //no changes, no need to confirm
            MessageBoxResult result = MessageBox.Show(
                "Zmiany nie zostały zapisane. Zapisać?",
                "Uwaga, niezapisane zmiany!",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Model.CommitTransaction().Wait(); ;
                    return true;

                case MessageBoxResult.No:
                    return true;

                case MessageBoxResult.Cancel:
                default:
                    return false;

            }
        }

        #endregion

    }
}
