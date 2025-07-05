using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Mozilla;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class GroupSubjectCoordinatorViewModel: TwoListBoxesViewModel, IPageViewModel
    {
        string IPageViewModel.Name => nameof(GroupSubjectCoordinatorViewModel);

        private GroupEditModel? _selectedGroup;
        private SubjectModel? _selectedSubject;

        private string _errorString = "";
        private string _successString = "";


        private List<GroupEditModel> _groups { get; set; } = new();
        private List<SubjectModel> _subjects { get; set; } = new();
        private LoginWrapper wrapper { get; init; }

        private GroupSubjectCoordinatorModel Model { get; init; }

        public GroupSubjectCoordinatorViewModel(LoginWrapper wrapper)
        {
           
            this.wrapper = wrapper;
            LeftPaneHeader = "Dostępni prowadzący";
            RightPaneHeader = "Przypisani prowadzący";

            Model = new(wrapper);
        }



        public bool IsLoading { get; set; } = false;
        public string ErrorString
        {
            get => _errorString;
            set
            {
                _errorString = value;
                OnPropertyChanged(nameof(ErrorString));
            }
        }
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
            SelectedGroup  = null;
            SelectedSubject = null;
            await PrepareBoxes();
        }

        private  async Task Save()
        {
            Model.Group = SelectedGroup;
            Model.Subject = SelectedSubject;
            foreach (CoordinatorModel coordinator in RightPaneItems)
            {
                Model.AssignCoordinator(coordinator);
            }
            bool success = await Model.ExecuteAssignments();

            if (success) {
                ErrorString = "";
                SuccessString = "Edycja zakończona pomyślnie!";
            }
            else
            {
                SuccessString = "";
                ErrorString = "Wystąpił błąd podczas edycji!";
            }
        }

        private bool IsFormValid() => !(SelectedGroup == null || SelectedSubject == null);

        public List<GroupEditModel> Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }

        public List<SubjectModel> Subjects
        {
            get => _subjects;
            set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

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



        public async Task LoadDataAsync()
        {
            Groups = await LoadGroups();
            Subjects = await LoadSubjects();

            await PrepareBoxes();
        }

     
        private async Task PrepareBoxes()
        {
            LeftPaneItems.Clear();
            RightPaneItems.Clear();
            var coordinators = await LoadCoordinators();
            LeftPaneItems = new ObservableCollection<object>(coordinators?.Cast<object>().ToList() ?? new List<object>());

            List<CoordinatorModel>? assignedCoordinators = null;
            if (IsFormValid())
            {
                // Select already assigned coordinators
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

        private async Task<List<CoordinatorModel>> LoadCoordinators()
        {
            IsLoading = true;
            var coordinators = await RetrieveService.GetAllAsync<CoordinatorModel>(wrapper);
            IsLoading = false;
            return coordinators;
        }

        private async Task<List<GroupEditModel>> LoadGroups()
        {
            IsLoading = true;

            GroupEditModel dummy = new(wrapper);
            var groups =  await RetrieveService.GetAllAsync<GroupEditModel>(wrapper);
            IsLoading = false;
            return groups;

        }

        private async Task<List<SubjectModel>> LoadSubjects()
        {
            IsLoading = true;
            var subjects = await RetrieveService.GetAllAsync<SubjectModel>(wrapper);
            IsLoading = false;
            return subjects;
        }
    }
}
