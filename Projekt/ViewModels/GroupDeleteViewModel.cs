using Mysqlx.Crud;
using Org.BouncyCastle.Utilities;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class GroupDeleteViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(GroupDeleteViewModel);

        private GroupViewTableModel? DataModel { get; init; }
        private GroupDeleteModel? DeleteModel { get; init; }

        private DataTable? _groups;
        public DataTable? Groups
        {
            get => _groups;
            private set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }

        private DataRowView? _selectedGroup;
        public DataRowView? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }

        public GroupDeleteViewModel(LoginWrapper loginWrapper)
        {
            DataModel = new(loginWrapper);
            DeleteModel = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false); ;
        }

        public GroupDeleteViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            Groups = await DataModel.LoginWrapper.DBHandler.GenerateDatatableAsync(DataModel.DefaultQuery);
        }


        private ICommand? _deleteSelectedCommand;
        public ICommand? DeleteSelectedCommand
        {
            get
            {
                _deleteSelectedCommand ??= new RelayCommand(
                    p => DeleteGroup()
                    );
                return _deleteSelectedCommand;
            }
        }


        #region Private methods/helpers


        private async void DeleteGroup()
        {
            var Result = MessageBox.Show(
                "Czy na pewno chcesz usunąć grupę?",
                "Potwierdzenie usunięcia",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (Result != MessageBoxResult.Yes || SelectedGroup == null)
            {
                MessageBox.Show("Nie usunięto wybranej grupy.", "Usuwanie przerwane", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                var parametersGet = new Dictionary<string, object>
                {
                    //{ "@groupId", SelectedGroup["ID grupy"] },
                    { "@groupNumber", SelectedGroup["Numer Grupy"] },
                    { "@faculty", SelectedGroup["Wydział"] },
                    { "@degree", SelectedGroup["Kierunek"] },
                    { "@semester", SelectedGroup["Semestr"] }
                };

                var resultGetId = await DataModel.LoginWrapper.DBHandler.ExecuteQueryAsync(
                    "CALL GetGroupId(@groupNumber, @faculty, @degree, @semester);", parametersGet
                );
                var id = resultGetId[0]["id"];

                DeleteModel.GroupId = (int)id;

                var resultDelete = await DeleteModel.LoginWrapper.DBHandler.ExecuteQueryAsync(
                    DeleteModel.DefaultQuery, DeleteModel.DefaultParameters
                );

                MessageBox.Show($"Grupa z ID {DeleteModel.GroupId} została usunięta.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                await GetDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania przedmiotu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            #endregion
        }
    }
}