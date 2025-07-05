using Mysqlx.Session;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class MainMenuViewModel : ConditionalContentControlViewModel, IPageViewModel
    {
        #region Fields
        string IPageViewModel.Name => "MainMenu";
        private MainMenuModel? _model { get; init; }

        private ICommand? _changePageView;
        private ConditionalContentControlViewModel? _subView { get; init; }

        public LoginWrapper? LoginWrapper
        {
            get => _model?.LoginWrapper;
        }

        #endregion

        #region Constructors
        public MainMenuViewModel(LoginWrapper loginWrapper)
        {
            _model = new(loginWrapper);
            CurrentPageViewModel = new CurrentProfileViewModel(_model.LoginWrapper);

        }
        public MainMenuViewModel() { }

        #endregion

        #region Public Properties/Commands

        private bool _currentProfileIsSelected;
        private bool _usersIsSelected;
        private bool _groupIsSelected;
        private bool _lessonsIsSelected;
        private bool _placeIsSelected;
        private bool _subjectIsSelected;

        public bool UsersIsSelected
        {
            get => _usersIsSelected;
            set
            {
                if (_usersIsSelected != value)
                {
                    _usersIsSelected = value;
                    OnPropertyChanged(nameof(UsersIsSelected));
                    if (_usersIsSelected)
                    {
                        _ = ChangePageView("UsersViewTableViewModel");
                    }
                }
            }
        }

        public bool LessonsIsSelected
        {
            get => _lessonsIsSelected;
            set
            {
                if (_lessonsIsSelected != value)
                {
                    _lessonsIsSelected = value;
                    OnPropertyChanged(nameof(LessonsIsSelected));
                    if (_lessonsIsSelected)
                    {
                        _ = ChangePageView("LessonsViewTableViewModel");
                    }
                }
            }
        }

        public bool GroupIsSelected
        {
            get => _groupIsSelected;
            set
            {
                if (_groupIsSelected != value)
                {
                    _groupIsSelected = value;
                    OnPropertyChanged(nameof(GroupIsSelected));
                    if (_groupIsSelected)
                    {
                        _ = ChangePageView("GroupViewTableViewModel");
                    }
                }
            }
        }

        public bool PlaceIsSelected
        {
            get => _placeIsSelected;
            set
            {
                if (_placeIsSelected != value)
                {
                    _placeIsSelected = value;
                    OnPropertyChanged(nameof(PlaceIsSelected));
                    if (_placeIsSelected)
                    {
                        _ = ChangePageView("PlaceViewTableViewModel");
                    }
                }
            }
        }

        public bool SubjectIsSelected
        {
            get => _subjectIsSelected;
            set
            {
                if (_subjectIsSelected != value)
                {
                    _subjectIsSelected = value;
                    OnPropertyChanged(nameof(SubjectIsSelected));
                    if (_subjectIsSelected)
                    {
                        _ = ChangePageView("SubjectViewTableViewModel");
                    }
                }
            }
        }

        public bool CurrentProfileIsSelected
        {
            get => _currentProfileIsSelected;
            set
            {
                if (_currentProfileIsSelected != value)
                {
                    _currentProfileIsSelected = value;
                    OnPropertyChanged(nameof(CurrentProfileIsSelected));
                    if (_currentProfileIsSelected)
                    {
                        _ = ChangePageView("CurrentProfileViewModel");
                    }
                }
            }
        }

        public ICommand ChangeCurrentPageViewCommand
        {
            get
            {
                _changePageView ??= new RelayCommand(
                    async param => await ChangePageView(param)
                );
                return _changePageView;
            }
        }

        public ICommand LogoutCommand
        {
            get
            {
                return new RelayCommand(
                    param =>
                    {
                        Logout();
                    });
            }
        }
        #endregion

        #region Private Helpers
        private void Logout()
        {
            _model?.LoginWrapper?.Logout();

        }

        private async Task<bool> CheckPageModelPermissions(object? param)
        {

            if (param is not string pageViewModelName || _model?.LoginWrapper == null) return false;

            switch (pageViewModelName)
            {

                case "UsersCreateViewModel": return await Authenticate(PermissionHelper.CanManageUsers);
                case "UsersEditViewModel": return await Authenticate(PermissionHelper.CanManageUsers);
                case "UsersDeleteViewModel": return await Authenticate(PermissionHelper.CanManageUsers);
                case "LessonsViewTableViewModel": return await Authenticate(PermissionHelper.CanSeeOwnSchedule);
                case "LessonsCreateViewModel":
                    return await Authenticate(
                        PermissionHelper.CanSeeOwnSchedule,
                        PermissionHelper.CanEditOwnSchedule,
                        PermissionHelper.CanSeeOtherSchedules,
                        PermissionHelper.CanEditOtherSchedules);
                case "LessonsEditViewModel":
                    return await Authenticate(
                       PermissionHelper.CanSeeOwnSchedule,
                       PermissionHelper.CanEditOwnSchedule,
                       PermissionHelper.CanSeeOtherSchedules,
                       PermissionHelper.CanEditOtherSchedules);
                case "LessonsDeleteViewModel":
                    return await Authenticate(
                       PermissionHelper.CanSeeOwnSchedule,
                       PermissionHelper.CanEditOwnSchedule,
                       PermissionHelper.CanSeeOtherSchedules,
                       PermissionHelper.CanEditOtherSchedules);
                case "GroupsViewTableViewModel":
                    return await Authenticate(
                        PermissionHelper.CanManageGroups,
                        PermissionHelper.CanManageUsers);
                case "GroupCreateViewModel":
                    return await Authenticate(
                        PermissionHelper.CanManageGroups,
                        PermissionHelper.CanSeeOtherProfiles);
                case "GroupEditViewModel":
                    return await Authenticate(
                       PermissionHelper.CanManageGroups,
                       PermissionHelper.CanSeeOtherProfiles);
                case "GroupDeleteViewModel":
                    return await Authenticate(
                       PermissionHelper.CanManageGroups,
                       PermissionHelper.CanSeeOtherProfiles);
                case "GroupSubjectCoordinatorViewModel":
                    return await Authenticate(
                        PermissionHelper.CanEditOtherSchedules,
                        PermissionHelper.CanEditOwnSchedule);
                case "PlacesViewTableViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "PlaceCreateViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "PlaceEditViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "PlaceDeleteViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "SubjectViewTableViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "SubjectCreateViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "SubjectEditViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "SubjectDeleteViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "CurrentProfileViewModel": return await Authenticate(PermissionHelper.CanSeeOwnProfile);
                case "FacultyCreateViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "FacultyEditViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                case "MajorManagementViewModel": return await Authenticate(PermissionHelper.CanModifyData);
                default: return false;
            }
        }


        private async Task ChangePageView(object? param)
        {
            if (CurrentPageViewModel is TableViewModel table)
            {
                if (!table.ConfirmExit()) return;
            }

            if (!await CheckPageModelPermissions(param)) return;

            if (param is not string pageViewModelName || _model?.LoginWrapper == null) return;

         
            Type? viewModelType = Type.GetType($"Projekt.ViewModels.{pageViewModelName}");
            if (viewModelType == null) return;

            CurrentPageViewModel = Activator.CreateInstance(viewModelType, _model.LoginWrapper) as IPageViewModel;


            // przypadki szczególne - gdzie samo zbudowanie ViewModela nie wystarczy
            switch (pageViewModelName)
            {
                case "LessonsCreateViewModel":
                    if (CurrentPageViewModel is LessonsCreateViewModel lessonsCreateModel)
                    {
                        await lessonsCreateModel.LoadAll();
                    }
                    break;
                case "GroupSubjectCoordinatorViewModel":
                    if (CurrentPageViewModel is GroupSubjectCoordinatorViewModel groupSubjectCoordinatorModel)
                    {
                        await groupSubjectCoordinatorModel.LoadDataAsync();
                    }
                    break;
                default: break;
            }
        }
        

        private async Task<bool> Authenticate(params int[] requiredPermissions)
        {
            return _model?.LoginWrapper != null && await _model.LoginWrapper.Authenticate();
        }
        #endregion
    }
}
