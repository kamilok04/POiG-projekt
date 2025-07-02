using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class SubjectDeleteViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(SubjectDeleteViewModel);

        #region Properties
        private SubjectViewTableModel DataModel { get; init; }
        private SubjectDeleteModel DeleteModel { get; init; }

        private DataTable? _subjects;
        private DataRowView? _selectedSubject;

        public DataTable? Subjects
        {
            get => _subjects;
            private set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        public DataRowView? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
            }
        }
        #endregion

        public SubjectDeleteViewModel(LoginWrapper loginWrapper)
        {

            DataModel = new(loginWrapper);
            DeleteModel = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false); ;
        }

        public SubjectDeleteViewModel() { } //for designer only

        private ICommand? _deleteSelectedCommand;
        public ICommand? DeleteSelectedCommand
        {
            get
            {
                _deleteSelectedCommand ??= new RelayCommand(
                    p => DeleteSubject()
                    );
                return _deleteSelectedCommand;
            }
        }


        #region Private methods/helpers


        private async void DeleteSubject()
        {
            var Result = MessageBox.Show(
                "Czy na pewno chcesz usunąć przedmiot?",
                "Potwierdzenie usunięcia",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (Result != MessageBoxResult.Yes || SelectedSubject == null)
            {
                MessageBox.Show("Nie usunięto wybranego przedmiotu.", "Usuwanie przerwane", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            { 
                var parametersGet = new Dictionary<string, object>
                {
                    { "@kod", SelectedSubject["kod"] },
                    { "@nazwa", SelectedSubject["nazwa"] }
                };

                var resultGetId = await DataModel.LoginWrapper.DBHandler.ExecuteQueryAsync(
                    "SELECT id, nazwa FROM dane_przedmiotu WHERE kod = @kod AND nazwa = @nazwa LIMIT 1", parametersGet
                );
                var id = resultGetId[0]["id"];
                var name = resultGetId[0]["nazwa"];

                DeleteModel._id = (int)id;

                var resultDelete = await DeleteModel.LoginWrapper.DBHandler.ExecuteQueryAsync(
                    DeleteModel.DefaultQuery, DeleteModel.DefaultParameters
                );

                MessageBox.Show($"Przedmiot '{name}' został usunięty.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                
                await GetDataAsync(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił błąd podczas usuwania przedmiotu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task GetDataAsync()
        {
            Subjects = await DataModel.LoginWrapper.DBHandler.GenerateDatatableAsync(DataModel.DefaultQuery);
        }

        #endregion
    }
}
