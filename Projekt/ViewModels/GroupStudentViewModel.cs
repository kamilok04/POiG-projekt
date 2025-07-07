using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using Projekt.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class GroupStudentViewModel : TwoDataGridsViewModel, ITable, IPageViewModel
    {
        string IPageViewModel.Name => nameof(GroupStudentViewModel);

        Dictionary<string, object>? ITable.DefaultParameters => SelectedGroup != null
            ? new() { { "@groupID", SelectedGroup.GroupId } }
            : null;

        string? ITable.DefaultQuery => "SELECT * FROM grupa_student WHERE id_grupy = @groupID";
        string ITable.TableName => "grupa_student";

        private List<GroupEditModel> _groups = [];
        private GroupEditModel? _selectedGroup;
        private List<FacultyModel> _faculties = [];
        private FacultyModel? _selectedFaculty;
        private List<MajorModel> _majors = [];
        private MajorModel? _selectedMajor;
        private List<int> _semesters = [1, 2, 3, 4, 5, 6, 7];
        private Int16 _selectedSemester;
        private readonly GroupStudentModel Model;

        private string _errorString = string.Empty;
        private string _successString = string.Empty;
        private HashSet<int> _assignedStudentIds = [];
        private LoginWrapper Wrapper { get; init; }

        public NotifyTaskCompletion<List<GroupEditModel>> LoadGroups { get; private set; }
        public NotifyTaskCompletion<List<FacultyModel>> LoadFaculties { get; private set; }
        public NotifyTaskCompletion<List<MajorModel>> LoadMajors { get; private set; }


        public List<GroupEditModel> Groups
        {
            get => _groups;
            set { _groups = value ?? []; OnPropertyChanged(nameof(Groups)); }
        }

        public GroupEditModel? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup != value)
                {
                    _selectedGroup = value;
                    OnPropertyChanged(nameof(SelectedGroup));
                    _ = OnSelectedGroupChangedAsync();
                }
            }
        }

        private async Task OnSelectedGroupChangedAsync()
        {
            await PrepareBoxes();
        }

        public List<FacultyModel> Faculties
        {
            get => _faculties;
            set { _faculties = value ?? []; OnPropertyChanged(nameof(Faculties)); }
        }

        public FacultyModel? SelectedFaculty
        {
            get => _selectedFaculty;
            set
            {
                _selectedFaculty = value;
                if(value != null) _ = OnFilterChanged();
                OnPropertyChanged(nameof(SelectedFaculty));
            }
        }

        public List<MajorModel> Majors
        {
            get => _majors;
            set { _majors = value ?? []; OnPropertyChanged(nameof(Majors)); }
        }

        public MajorModel? SelectedMajor
        {
            get => _selectedMajor;
            set
            {
                _selectedMajor = value;
                if(value != null) _ = OnFilterChanged();
                OnPropertyChanged(nameof(SelectedMajor));
            }
        }

        public List<int> Semesters
        {
            get => _semesters;
            set { _semesters = value ?? []; OnPropertyChanged(nameof(Semesters)); }
        }

        public Int16 SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                _selectedSemester = value;
                _ = OnFilterChanged();
                OnPropertyChanged(nameof(SelectedSemester));
            }
        }

        public string ErrorString
        {
            get => _errorString;
            set
            {
                if (_errorString != value)
                {
                    _errorString = value ?? string.Empty;
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

        private ICommand? _resetFiltersCommand { get; set; }
        public ICommand ResetFiltersCommand
        {
            get => _resetFiltersCommand ??= new RelayCommand(
                param => ResetFilters());
        }
        private ICommand? _cancelCommand { get; set; }
        public ICommand CancelCommand
        {
            get => _cancelCommand ??= new RelayCommand(
                async param => await Cancel()
            );
        }

        /// <summary>
        /// Anuluje zmiany i przywraca formularz do stanu wyjścia.
        /// </summary>
        /// <returns></returns>
        private async Task Cancel()
        {
            SelectedGroup = null;
            SelectedFaculty = null;
            SelectedMajor = null;
            SelectedSemester = 0;
            RightPaneItems.Clear();
            await PrepareBoxes();
        }

        /// <summary>
        /// Przygotowuje obie listy (lewa i prawa) z studentami.
        /// </summary>
        /// <returns></returns>
        private async Task PrepareBoxes()
        {
            await PrepareRightPane();
            await PrepareLeftPane();
        }

        /// <summary>
        /// Przygotowuje prawą listę z studentami przypisanymi do grupy.
        /// </summary>
        /// <returns></returns>
        private async Task PrepareRightPane()
        {
            if (SelectedGroup == null) return;
            RightPaneItems = new(await Model.GetCurrentGroupStudents(SelectedGroup.GroupId));
        }

        /// <summary>
        /// Reaguje na zmiany filtrów (wydział, kierunek, semestr) i aktualizuje listę kierunków.
        /// </summary>
        /// <returns></returns>
        private async Task OnFilterChanged()
        {
            await PrepareLeftPane();
        }

        /// <summary>
        /// Przygotowuje lewą listę z dostępnych (spełniających filtry) studentów, którzy nie są przypisani do grupy.
        /// </summary>
        /// <returns></returns>
        private async Task PrepareLeftPane()
        {
            var students = await Model.GetFilteredStudents(SelectedFaculty, SelectedSemester, SelectedMajor);
            _assignedStudentIds = [.. RightPaneItems
                .OfType<ExtendedStudentModel>()
                .Select(s => s.StudentID)];

            var filteredStudents = students
                .Where(s => !_assignedStudentIds.Contains(s.StudentID))
                .ToList();

            LeftPaneItems = [.. filteredStudents];
        }

        /// <summary>
        /// Resetuje filtry (wydział, kierunek, semestr) do wartości domyślnych.
        /// </summary>
        private void ResetFilters()
        {
            SelectedFaculty = null;
            SelectedMajor = null;
            SelectedSemester = 0;
        }

        /// <summary>
        /// Próbuje zapisać zmiany w przypisaniach studentów do grupy.
        /// </summary>
        /// <returns></returns>
        private async Task Save()
        {
            var rightPaneStudentIds = RightPaneItems
                .OfType<ExtendedStudentModel>()
                .Select(s => s.StudentID)
                .ToHashSet();

            var toAdd = rightPaneStudentIds.Except(_assignedStudentIds).ToList();
            var toRemove = _assignedStudentIds.Except(rightPaneStudentIds).ToList();

            // Use null-conditional operator for SelectedGroup
            var groupId = SelectedGroup?.GroupId ?? 0;
            var commands = Model.CreateGroupTransactionCommands(toAdd, toRemove, groupId);

            bool success = await Model.ExecuteAssignments(commands);

            if (success)
            {
                ErrorString = "";
                SuccessString = "Edycja zakończona pomyślnie!";
            }
            else
            {
                SuccessString = "";
                ErrorString = "Wystąpił błąd podczas edycji!";
            }

            await PrepareBoxes();
        }

        /// <summary>
        /// Sprawdza, czy formularz jest poprawnie wypełniony.
        /// Wybór grupy to jedyne wymaganie (puste grupy są dozwolone).
        /// </summary>
        /// <returns></returns>
        private bool IsFormValid() => SelectedGroup != null;

        public string SuccessString
        {
            get => _successString;
            set
            {
                if (_successString != value)
                {
                    _successString = value ?? string.Empty;
                    _errorString = string.Empty;
                    OnPropertyChanged(nameof(SuccessString));
                }
            }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="wrapper">LoginWrapper</param>
        public GroupStudentViewModel(LoginWrapper wrapper)
        {
            Groups = [];
            Faculties = [];
            Majors = [];
            Semesters = [1, 2, 3, 4, 5, 6, 7];
            Wrapper = wrapper;
            Model = new(Wrapper);

            LeftPaneHeader = "Dostępni studenci";
            RightPaneHeader = "Wybrani studenci";

            LoadGroups = new NotifyTaskCompletion<List<GroupEditModel>>(LoadGroupsAsync());
            LoadFaculties = new NotifyTaskCompletion<List<FacultyModel>>(LoadFacultiesAsync());
            LoadMajors = new NotifyTaskCompletion<List<MajorModel>>(LoadMajorsAsync());

            _ = PrepareBoxes();
        }

        /// <summary>
        /// Wczytaj grupy z bazy. Funkcja jest prywatna, do binda wystawiamy NotifyTaskCompletion<T>.
        /// </summary>
        /// <returns></returns>
        private async Task<List<GroupEditModel>> LoadGroupsAsync()
        {
            // Return empty list if null
            return await RetrieveService.GetAllAsync<GroupEditModel>(Wrapper) ?? [];
        }

        /// <summary>
        /// Wczytaj wydziały z bazy. Funkcja jest prywatna, do binda wystawiamy NotifyTaskCompletion<T>.
        /// </summary>
        private async Task<List<FacultyModel>> LoadFacultiesAsync()
        {
            return await RetrieveService.GetAllAsync<FacultyModel>(Wrapper) ?? [];
        }

        /// <summary>
        /// Wczytaj kierunki z bazy. Funkcja jest prywatna, do binda wystawiamy NotifyTaskCompletion<T>.
        /// </summary>
        private async Task<List<MajorModel>> LoadMajorsAsync()
        {
            List<MajorModel> result = await RetrieveService.GetAllAsync<MajorModel>(Wrapper) ?? [];
            foreach (var model in result)
                if (model != null)
                    model.LoginWrapper = Wrapper;
            return result;
        }


        /// <summary>
        /// Próuje przenieść zaznaczony element (tutaj: studenta) z jednej listy do drugiej:
        /// lewo -> prawo: zawsze się da
        /// prawo <- lewo: da się tylko, jeśli przenoszony student spełnia warunki filtrów (wydział, kierunek, semestr).
        /// </summary>
        /// <param name="target">Lista docelowa (LeftPaneItems albo RightPaneItems)</param>
        public override void Move(ObservableCollection<object> target)
        {
            var source = target == LeftPaneItems ? RightPaneItems : LeftPaneItems;

            if (target == RightPaneItems)
            {
                var item = GetSelectedItem(source);
                if (item is not ExtendedStudentModel student) return;
                source.Remove(student);
                int id = student.StudentID;
                if (!_assignedStudentIds.Contains(id))
                {
                    target.Add(student);
                }
                return;
            }

            var leftItem = GetSelectedItem(source);
            if (leftItem is not ExtendedStudentModel leftStudent) return;
            source.Remove(leftStudent);

            bool matchesFaculty = SelectedFaculty == null || (leftStudent.FacultyName == SelectedFaculty.Name);
            bool matchesMajor = SelectedMajor == null || (leftStudent.MajorName == SelectedMajor.Name);
            bool matchesSemester = SelectedSemester == 0 || (leftStudent.Semester == SelectedSemester);

            if (matchesFaculty && matchesMajor && matchesSemester)
            {
                target.Add(leftStudent);
            }
        }

        /// <summary>
        /// Przenosi wszystkie elementy z jednej listy do drugiej.
        /// </summary>
        /// <param name="target">Lista docelowa (LeftPaneItems albo RightPaneItems)</param>

        public override void MoveAll(ObservableCollection<object> target)
        {
            var source = target == LeftPaneItems ? RightPaneItems : LeftPaneItems;
            var itemsToMove = source.OfType<object>().ToList();
            foreach (var item in itemsToMove)
            {
                SetSelectedItem(source, item);
                Move(target);
            }
        }

        public override bool CanMove()
         => SelectedGroup != null;
    }
}
