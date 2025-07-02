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
    public class SubjectEditViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(SubjectEditViewModel);

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

        public SubjectEditViewModel(LoginWrapper loginWrapper)
        {

            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false); ;
        }

        public SubjectEditViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            Subjects = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(Model.DefaultQuery);
        }
    }
}
