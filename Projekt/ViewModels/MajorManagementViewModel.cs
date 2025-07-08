using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Org.BouncyCastle.Crypto;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class MajorManagementViewModel : ObservableObject, IPageViewModel
    {

        string IPageViewModel.Name => nameof(MajorManagementViewModel);


        private MajorManagementModel Model { get; init; }
        private LoginWrapper Wrapper { get; init; }
        private int? _id;
        private string? _name;
        private FacultyModel? _faculty;
        private string _successString = "";
        private string _errorString = "";
        private MajorModel? _selectedMajor;
        public MajorModel? SelectedMajor
        {
            get => _selectedMajor;
            set
            {
                _selectedMajor = value;
                LoadMajor();
                OnPropertyChanged(nameof(SelectedMajor));
            }
        }

        public int? Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        public string? Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public FacultyModel? SelectedFaculty
        {
            get => _faculty;
            set
            {
                _faculty = value;
                OnPropertyChanged(nameof(SelectedFaculty));
            }
        }

        public string SuccessString
        {
            get => _successString;
            set
            {
                _successString = value;
                _errorString = "";
                OnPropertyChanged(nameof(SuccessString));
                OnPropertyChanged(nameof(ErrorString));
            }
        }

        public string ErrorString
        {
            get => _errorString;
            set
            {
                _errorString = value;
                _successString = "";
                OnPropertyChanged(nameof(ErrorString));
                OnPropertyChanged(nameof(SuccessString));

            }
        }

        private ICommand? _saveCommand;
        public ICommand? SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                    async param => await HandleSave(),
                    param => SelectedMajor != null);
            }
        }

        private ICommand? _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand ??= new RelayCommand(
                    param => Cancel());
            }
        }


        private ICommand? _deleteCommand;
        public ICommand? DeleteCommand
        {
            get
            {
                return _deleteCommand ??= new RelayCommand(
                    async param => await HandleDelete(),
                    param => SelectedMajor != null);
            }
        }


        private async Task HandleSave()
        {
            if (SelectedMajor == null)
            {
                ErrorString = "Wystąpił błąd, spróbuj ponownie.";
                return;
            }

            int success = await Model.HandleSave(
                SelectedMajor.Id,
                SelectedMajor.NameId,
                Name ?? string.Empty,
                SelectedFaculty?.Id ?? string.Empty);

            if (success == 1)
            {
                SuccessString = "Edytowano pomyślnie.";
                Refresh();
            }
            if (success == 0)
            {
                ErrorString = "Nie udało się edytować!";
            }

        }

        private async Task HandleDelete()
        {
            if (SelectedMajor == null)
            {
                ErrorString = "Wystąpił błąd, spróbuj ponownie.";
                return;
            }
            int success = await Model.HandleDelete(SelectedMajor.Id);
            if (success == 1)
            {
                SuccessString = "Usunięto pomyślnie.";
                Refresh();
            }
            if (success == 0)
            {
                ErrorString = "Nie udało się usunąć!";
            }

            else
            {
                ErrorString = "Nie możesz usunąć kierunku, do którego istnieją przypisane roczniki.\r\nUsuń wszystkie roczniki przypisane do przedmiotu, zanim spróbujesz ponownie";
            }
        
        }

        private void Cancel()
        {
            SelectedMajor = null;
            Name = null;
            SelectedFaculty = null;
        }

        private void Refresh()
        {
            NotifyMajors = new NotifyTaskCompletion<ObservableCollection<MajorModel>>(GetMajors());
            OnPropertyChanged(nameof(NotifyMajors));
            Cancel();
        }

        public MajorManagementViewModel(LoginWrapper loginWrapper)
        {

            Wrapper = loginWrapper;
            Model = new(Wrapper);
            NotifyFaculties = new(GetFaculties());
            NotifyMajors = new(GetMajors());
        }
        private void LoadMajor()
        {
            Name = SelectedMajor?.Name;

            if (SelectedMajor?.Faculty != null && NotifyFaculties?.Result != null)
            {
                SelectedFaculty = NotifyFaculties.Result.FirstOrDefault(f => f.Id == SelectedMajor.Faculty.Id);
            }
            else
            {
                SelectedFaculty = null;
            }
        }

        public MajorManagementViewModel() { } //for designer only




        public NotifyTaskCompletion<ObservableCollection<FacultyModel>> NotifyFaculties { get; private set; }
        private async Task<ObservableCollection<FacultyModel>> GetFaculties()
        {
            if (Wrapper == null || Wrapper.DBHandler == null)
                throw new InvalidOperationException("LoginWrapper or DBHandler is not initialized.");

            List<FacultyModel>? faculties = await RetrieveService.GetAllAsync<FacultyModel>(Wrapper);
            if (faculties == null)
                return new ObservableCollection<FacultyModel>();

            return new ObservableCollection<FacultyModel>(faculties);
        }
        public NotifyTaskCompletion<ObservableCollection<MajorModel>> NotifyMajors { get; private set; }

        private async Task<ObservableCollection<MajorModel>> GetMajors()
        {
            if (Wrapper == null || Wrapper.DBHandler == null)
                throw new InvalidOperationException("LoginWrapper or DBHandler is not initialized.");
            List<MajorModel>? majors = await RetrieveService.GetAllAsync<MajorModel>(Wrapper);
            if (majors == null)
                return new ObservableCollection<MajorModel>();
            return new ObservableCollection<MajorModel>(majors);
        }


    }
}
