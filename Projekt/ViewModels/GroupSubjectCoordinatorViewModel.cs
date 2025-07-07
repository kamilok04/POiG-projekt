using Projekt.Miscellaneous;
using Projekt.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    /// <summary>
    /// ViewModel do koordynowania przypisywania prowadzących do grup i przedmiotów.
    /// </summary>
    public class GroupSubjectCoordinatorViewModel : TwoListBoxesViewModel, IPageViewModel
    {
        /// <summary>
        /// Nazwa widoku strony.
        /// </summary>
        string IPageViewModel.Name => nameof(GroupSubjectCoordinatorViewModel);

        private GroupEditModel? _selectedGroup;
        private SubjectModel? _selectedSubject;

        private string _errorString = "";
        private string _successString = "";

        private List<GroupEditModel> _groups { get; set; } = new();
        private List<SubjectModel> _subjects { get; set; } = new();
        private LoginWrapper wrapper { get; init; }

        private GroupSubjectCoordinatorModel Model { get; init; }

        /// <summary>
        /// Inicjalizuje nową instancję GroupSubjectCoordinatorViewModel.
        /// </summary>
        /// <param name="wrapper">Obiekt logowania.</param>
        public GroupSubjectCoordinatorViewModel(LoginWrapper wrapper)
        {
            this.wrapper = wrapper;
            LeftPaneHeader = "Dostępni prowadzący";
            RightPaneHeader = "Przypisani prowadzący";
            Model = new(wrapper);
        }

        /// <summary>
        /// Czy trwa ładowanie danych.
        /// </summary>
        public bool IsLoading { get; set; } = false;

        /// <summary>
        /// Komunikat o błędzie.
        /// </summary>
        public string ErrorString
        {
            get => _errorString;
            set
            {
                _errorString = value;
                OnPropertyChanged(nameof(ErrorString));
            }
        }

        /// <summary>
        /// Komunikat o sukcesie.
        /// </summary>
        public string SuccessString
        {
            get => _successString;
            set
            {
                _successString = value;
                OnPropertyChanged(nameof(SuccessString));
            }
        }

        private ICommand? _saveCommand { get; set; }
        /// <summary>
        /// Komenda zapisu zmian.
        /// </summary>
        public ICommand SaveCommand
        {
            get => _saveCommand ??= new RelayCommand(
                async param => await Save(),
                param => IsFormValid());
        }

        private ICommand? _cancelCommand { get; set; }
        /// <summary>
        /// Komenda anulowania zmian.
        /// </summary>
        public ICommand CancelCommand
        {
            get => _cancelCommand ??= new RelayCommand(
                async param => await Cancel()
            );
        }

        /// <summary>
        /// Anuluje zmiany i czyści wybory.
        /// </summary>
        private async Task Cancel()
        {
            SelectedGroup = null;
            SelectedSubject = null;
            await PrepareBoxes();
        }

        /// <summary>
        /// Zapisuje przypisania prowadzących do grupy/przedmiotu.
        /// </summary>
        private async Task Save()
        {
            Model.Group = SelectedGroup;
            Model.Subject = SelectedSubject;
            foreach (CoordinatorModel coordinator in RightPaneItems)
            {
                Model.AssignCoordinator(coordinator);
            }
            bool success = await Model.ExecuteAssignments();

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
        }

        /// <summary>
        /// Sprawdza poprawność formularza.
        /// </summary>
        /// <returns>True, jeśli formularz jest poprawny.</returns>
        private bool IsFormValid() => !(SelectedGroup == null || SelectedSubject == null);

        /// <summary>
        /// Lista dostępnych grup.
        /// </summary>
        public List<GroupEditModel> Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }

        /// <summary>
        /// Lista dostępnych przedmiotów.
        /// </summary>
        public List<SubjectModel> Subjects
        {
            get => _subjects;
            set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        /// <summary>
        /// Aktualnie wybrana grupa.
        /// </summary>
        public GroupEditModel? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
                Model.Group = value;
                _ = PrepareBoxes();
            }
        }

        /// <summary>
        /// Aktualnie wybrany przedmiot.
        /// </summary>
        public SubjectModel? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
                Model.Subject = value;
                _ = PrepareBoxes();
            }
        }

        /// <summary>
        /// Wczytuje dane grup i przedmiotów oraz przygotowuje listy.
        /// </summary>
        public async Task LoadDataAsync()
        {
            Groups = await LoadGroups();
            Subjects = await LoadSubjects();
            await PrepareBoxes();
        }

        /// <summary>
        /// Przygotowuje listy prowadzących.
        /// </summary>
        private async Task PrepareBoxes()
        {
            LeftPaneItems.Clear();
            RightPaneItems.Clear();
            var coordinators = await LoadCoordinators();
            LeftPaneItems = new ObservableCollection<object>(coordinators?.Cast<object>().ToList() ?? new List<object>());

            List<CoordinatorModel>? assignedCoordinators = null;
            if (IsFormValid())
            {
                var result = await Model.GetAssignedCoordinators();
                if (result != null)
                {
                    assignedCoordinators = new List<CoordinatorModel>();
                    foreach (var row in result)
                    {
                        var coordinator = await RetrieveService.GetAsync<CoordinatorModel>(wrapper, (string)row["id_prowadzacego"]);
                        if (coordinator != null)
                        {
                            assignedCoordinators.Add(coordinator);
                        }
                    }
                }
            }

            if (assignedCoordinators != null && assignedCoordinators.Count > 0)
            {
                foreach (CoordinatorModel coordinator in assignedCoordinators)
                {
                    LeftPaneItems.Remove(coordinator);
                    RightPaneItems.Add(coordinator);
                }
            }
        }

        /// <summary>
        /// Wczytuje listę prowadzących.
        /// </summary>
        private async Task<List<CoordinatorModel>> LoadCoordinators()
        {
            IsLoading = true;
            var coordinators = await RetrieveService.GetAllAsync<CoordinatorModel>(wrapper);
            IsLoading = false;
            return coordinators ?? [];
        }

        /// <summary>
        /// Wczytuje listę grup.
        /// </summary>
        private async Task<List<GroupEditModel>> LoadGroups()
        {
            IsLoading = true;
            GroupEditModel dummy = new(wrapper);
            var groups = await RetrieveService.GetAllAsync<GroupEditModel>(wrapper);
            IsLoading = false;
            return groups ?? [];
        }

        /// <summary>
        /// Wczytuje listę przedmiotów.
        /// </summary>
        private async Task<List<SubjectModel>> LoadSubjects()
        {
            IsLoading = true;
            var subjects = await RetrieveService.GetAllAsync<SubjectModel>(wrapper);
            IsLoading = false;
            return subjects ?? [];
        }
    }
}
