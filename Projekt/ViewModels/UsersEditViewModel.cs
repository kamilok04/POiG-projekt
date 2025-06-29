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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

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
            RowKey = "Login";
            GetDataAsync().ConfigureAwait(false); ;
        }

        private ICommand? _tableSaveCommand;
        public override ICommand TableSaveCommand
        {
            get => _tableSaveCommand ??= new RelayCommand(
                param => Model.CommitTransaction());
        
        }

        private ICommand? _tableCancelCommand;
        public override ICommand TableCancelCommand
        {
            get => _tableCancelCommand ??= new RelayCommand(
                param => GetDataAsync().Wait());
        }

        private ICommand _tableDeleteCommand;
        public override ICommand TableDeleteCommand
        {
            get => _tableDeleteCommand ??= new RelayCommand(
                // TODO: uzupełnić
                param => { }
                );
        }

        private ICommand _tableCreateCommand;
        public override ICommand TableCreateCommand
        {
            get => _tableCreateCommand ??= new RelayCommand(
                param => SendToCreator());
                
        }

        public UsersEditViewModel() { } //for designer only

        private void SendToCreator()
        {

            var mainMenuVm = Application.Current.MainWindow.DataContext as MainMenuViewModel;
            if (mainMenuVm != null)
            {
                mainMenuVm.CurrentPageViewModel = new UsersCreateViewModel(Model.LoginWrapper);
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

        }

        public override bool ConfirmExit()
        {
            MessageBoxResult result = MessageBox.Show(
                "Zmiany nie zostały zapisane. Zapisać?", 
                "Uwaga, niezapisane zmiany!",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question,
                MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    Model.CommitTransaction();
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
