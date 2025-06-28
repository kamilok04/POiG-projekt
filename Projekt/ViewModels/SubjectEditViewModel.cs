using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class SubjectEditViewModel : ObservableObject, IPageViewModel, ITable
    {
        string IPageViewModel.Name => nameof(SubjectEditViewModel);
        public string TableName => "dane_przedmiotu";

        public string? DefaultQuery => "SELECT * FROM dane_przedmiotu";

        private SubjectViewTableModel Model { get; init; }

        private DataTable? _subjects;
        public DataTable? Subjects
        {
            get => _subjects;
            private set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        

        public Dictionary<string, object>? DefaultParameters => throw new NotImplementedException();

        public SubjectEditViewModel(LoginWrapper loginWrapper)
        {

            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false); ;
        }

        public SubjectEditViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            Subjects = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(((ITable)this).DefaultQuery);
        }
    }
}
