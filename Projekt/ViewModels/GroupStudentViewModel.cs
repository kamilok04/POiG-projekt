using Projekt.Miscellaneous;
using Projekt.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class GroupStudentViewModel: TwoDataGridsViewModel, ITable, IPageViewModel
    {
        string IPageViewModel.Name => nameof(GroupStudentViewModel);
        Dictionary<string, object>? ITable.DefaultParameters => new() { { "@groupID", SelectedGroup.GroupId } };
        string? ITable.DefaultQuery => "SELECT * FROM grupa_student WHERE id_grupy = @groupID";
        string ITable.TableName => "grupa_student";
        // Private fields
        private ObservableCollection<GroupEditModel> _groups;
        private GroupEditModel _selectedGroup;
        private ObservableCollection<FacultyModel> _faculties;
        private FacultyModel _selectedFaculty;
        private ObservableCollection<string> _majors;
        private string _selectedMajor;
        private ObservableCollection<int> _semesters;
        private int _selectedSemester;
        
        // Private fields
        private string _errorString;
        private string _successString;
        private LoginWrapper Wrapper { get; init; }


        // Public properties
        public NotifyTaskCompletion<ObservableCollection<GroupEditModel>> LoadGroups { get; private set; }
        public NotifyTaskCompletion<ObservableCollection<FacultyModel>> LoadFaculties { get; private set; }
        public NotifyTaskCompletion<ObservableCollection<MajorModel>> LoadMajors { get; private set; }

        public ObservableCollection<GroupEditModel> Groups
        {
            get => _groups;
            set { _groups = value; OnPropertyChanged(nameof(Groups)); }
        }

        public GroupEditModel SelectedGroup
        {
            get => _selectedGroup;
            set { _selectedGroup = value; OnPropertyChanged(nameof(SelectedGroup)); }
        }

        public ObservableCollection<FacultyModel> Faculties
        {
            get => _faculties;
            set { _faculties = value; OnPropertyChanged(nameof(Faculties)); }
        }

        public FacultyModel SelectedFaculty
        {
            get => _selectedFaculty;
            set { _selectedFaculty = value; OnPropertyChanged(nameof(SelectedFaculty)); }
        }

        public ObservableCollection<string> Majors
        {
            get => _majors;
            set { _majors = value; OnPropertyChanged(nameof(Majors)); }
        }

        public string SelectedMajor
        {
            get => _selectedMajor;
            set { _selectedMajor = value; OnPropertyChanged(nameof(SelectedMajor)); }
        }

        public ObservableCollection<int> Semesters
        {
            get => _semesters;
            set { _semesters = value; OnPropertyChanged(nameof(Semesters)); }
        }

        public int SelectedSemester
        {
            get => _selectedSemester;
            set { _selectedSemester = value; OnPropertyChanged(nameof(SelectedSemester)); }
        }

        public string ErrorString
        {
            get => _errorString;
            set
            {
                if (_errorString != value)
                {
                    _errorString = value;
                    
                        _successString = string.Empty;
                    
                    OnPropertyChanged(nameof(ErrorString));
                }
            }
        }

        private ICommand? _saveCommand { get; set; }
        public ICommand SaveCommand
        {
            get => _saveCommand ??= new RelayCommand(
                async param => await Save(),
                param => IsFormValid());

        }

        private ICommand? _cancelCommand { get; set; }
        public ICommand CancelCommand
        {
            get => _cancelCommand ??= new RelayCommand(
                async param => await Cancel()
            );
        }

        private async Task Cancel()
        {
            SelectedGroup = null;
            SelectedFaculty = null;
            SelectedMajor = null;
            SelectedSemester = 0;
            await PrepareBoxes();
        }

        private async Task PrepareBoxes()
        {
            LeftPaneItems.Clear();
            RightPaneItems.Clear();

           
            Dictionary<string, object> parameters = new()
            {
                { "@facultyId", SelectedFaculty?.Id ?? "%" },
                { "@majorName", SelectedMajor ?? "%" },
                { "@semester", SelectedSemester == 0 ? "%" : SelectedSemester } 
            };

            string query = "SELECT * FROM dane_studenta " +
                "WHERE IFNULL(Wydział, 1) LIKE @facultyId " +
                "AND IFNULL(Kierunek, 1) LIKE @majorName " +
                "AND IFNULL(Semestr, 1) LIKE @semester";
           

            var dbHandler = Wrapper.DBHandler;
            if (dbHandler == null)
                return;

            var studentRows = await dbHandler.ExecuteQueryAsync(query, parameters);
            var allStudents = studentRows.Select(row => new ExtendedStudentModel(row)).ToList();

            // Get students already in the selected group
            List<int> groupStudentIds = new();
            if (SelectedGroup != null)
            {
                var groupParams = new Dictionary<string, object> { { "@groupID", SelectedGroup.GroupId } };
                var groupQuery = "SELECT id_studenta FROM grupa_student WHERE id_grupy = @groupID";
                var groupRows = await dbHandler.ExecuteQueryAsync(groupQuery, groupParams);
                groupStudentIds = groupRows.Select(r => Convert.ToInt32(r["id_studenta"])).ToList();
            }

            foreach (var student in allStudents)
            {
                if (groupStudentIds.Contains(student.StudentID))
                    RightPaneItems.Add(student);
                else
                    LeftPaneItems.Add(student);
            }
        }

        private async Task Save()
        {
            //Model.Group = SelectedGroup;
            //Model.Subject = SelectedSubject;
            //foreach (CoordinatorModel student in RightPaneItems)
            //{
            //    Model.AssingStudent(student);
            //}
            //bool success = await Model.ExecuteAssignments();

            //if (success)
            //{
            //    ErrorString = "";
            //    SuccessString = "Edycja zakończona pomyślnie!";
            //}
            //else
            //{
            //    SuccessString = "";
            //    ErrorString = "Wystąpił błąd podczas edycji!";
            //}
        }

        private bool IsFormValid() => !(SelectedGroup == null);


        public string SuccessString
        {
            get => _successString;
            set
            {
                if (_successString != value)
                {
                    _successString = value;
                   _errorString = string.Empty;
                    
                    OnPropertyChanged(nameof(SuccessString));
                }
            }
        }


  
        public GroupStudentViewModel(LoginWrapper wrapper)
        {
            Groups = new ObservableCollection<GroupEditModel>();
            Faculties = new ObservableCollection<FacultyModel>();
            Majors = new ObservableCollection<string>();
            Semesters = new ObservableCollection<int> { 1, 2, 3, 4, 5, 6, 7 };
            Wrapper = wrapper;

            LeftPaneHeader = "Dostępni studenci";
            RightPaneHeader = "Wybrani studenci";

            LoadGroups = new NotifyTaskCompletion<ObservableCollection<GroupEditModel>>(LoadGroupsAsync());
            LoadFaculties = new NotifyTaskCompletion<ObservableCollection<FacultyModel>>(LoadFacultiesAsync());
            LoadMajors = new NotifyTaskCompletion<ObservableCollection<MajorModel>>(LoadMajorsAsync());

            _ = PrepareBoxes();
        }


        private async Task<ObservableCollection<GroupEditModel>> LoadGroupsAsync()
        {
            return new(await RetrieveService.GetAllAsync<GroupEditModel>(Wrapper));
        }

        private async Task<ObservableCollection<FacultyModel>> LoadFacultiesAsync()
        {
            return new(await RetrieveService.GetAllAsync<FacultyModel>(Wrapper));
        }

        private async Task<ObservableCollection<MajorModel>> LoadMajorsAsync()
        {
            ObservableCollection<MajorModel> result = new(await RetrieveService.GetAllAsync<MajorModel>(Wrapper));
            foreach (var model in result) model.LoginWrapper = Wrapper;
            return result;
        }

    }
}
