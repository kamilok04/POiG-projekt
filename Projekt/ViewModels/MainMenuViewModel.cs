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
        private ConditionalContentControlViewModel? _subView {  get; init; }

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
        private async Task ChangePageView(object? param)
        {
            // Na pewno wolno ustawić taki widok?
            if(!await Authenticate()) return;
            if (param is string pageViewModelName && _model?.LoginWrapper != null) 
            {
                switch (pageViewModelName)
                {

                    // TODO: zbombardować tego switcha
                    // Możliwe rozwiązanie:
                    //      Type type = Type.GetType("pageViewModelName");
                    //      CurrentPageViewModel = new type(_model.LoginWrapper);

                    // Problem: każdy panel ma inne uprawnienia dostępu
                    //      przeniesienie weryfikacji do konstruktora może mijać się z celem? (TODO: sprawdzić)

                    case "UsersCreateViewModel":

                        if (!await Authenticate(PermissionHelper.CanManageUsers))
                          return;
                        CurrentPageViewModel = new UsersCreateViewModel(_model.LoginWrapper);
                        break;
                    case "UsersEditViewModel":
                        if (!await Authenticate(PermissionHelper.CanManageUsers))
                           return;
                        CurrentPageViewModel = new UsersEditViewModel(_model.LoginWrapper);
                        break;
                    
                    case "UsersDeleteViewModel":
                        if (!await Authenticate(PermissionHelper.CanManageUsers))
                            return;
                        CurrentPageViewModel = new UsersDeleteViewModel(_model.LoginWrapper);
                        break;
                    case "LessonsViewTableViewModel":
                        CurrentPageViewModel = new LessonsViewTableViewModel(_model.LoginWrapper);
                        break;
                    case "LessonsCreateViewModel":
                        if (!await Authenticate(
                            PermissionHelper.CanSeeOwnSchedule,
                            PermissionHelper.CanEditOwnSchedule,
                            PermissionHelper.CanSeeOtherSchedules,
                            PermissionHelper.CanEditOtherSchedules))
                            return;
                        CurrentPageViewModel = new LessonsCreateViewModel(_model.LoginWrapper);
                        break;
                    case "LessonsEditViewModel":
                        if (!await Authenticate(
                           PermissionHelper.CanSeeOwnSchedule,
                           PermissionHelper.CanEditOwnSchedule,
                           PermissionHelper.CanSeeOtherSchedules,
                           PermissionHelper.CanEditOtherSchedules))
                            return;
                        CurrentPageViewModel = new LessonsEditViewModel(_model.LoginWrapper);
                        break;
                    case "LessonsDeleteViewModel":
                        if (!await Authenticate(
                           PermissionHelper.CanSeeOwnSchedule,
                           PermissionHelper.CanEditOwnSchedule,
                           PermissionHelper.CanSeeOtherSchedules,
                           PermissionHelper.CanEditOtherSchedules))
                            return;
                        CurrentPageViewModel = new LessonsDeleteViewModel(_model.LoginWrapper);
                        break;
                    case "GroupsViewTableViewModel":
                        CurrentPageViewModel = new GroupViewTableViewModel(_model.LoginWrapper);
                        break;
                    case "GroupCreateViewModel":
                        if (!await Authenticate(
                            PermissionHelper.CanManageGroups,
                            PermissionHelper.CanSeeOtherProfiles))
                            return;
                        CurrentPageViewModel = new GroupCreateViewModel(_model.LoginWrapper);
                        break;
                    case "GroupEditViewModel":
                        if (!await Authenticate(
                           PermissionHelper.CanManageGroups,
                           PermissionHelper.CanSeeOtherProfiles))
                            return;
                        CurrentPageViewModel = new GroupEditViewModel(_model.LoginWrapper);
                        break;
                    case "GroupDeleteViewModel":
                        if (!await Authenticate(
                           PermissionHelper.CanManageGroups,
                           PermissionHelper.CanSeeOtherProfiles))
                            return;
                        CurrentPageViewModel = new GroupDeleteViewModel(_model.LoginWrapper);
                        break;
                    case "PlacesViewTableViewModel":
                        CurrentPageViewModel = new PlaceViewTableViewModel(_model.LoginWrapper);
                        break;
                    case "PlaceCreateViewModel":
                        if (!await Authenticate(PermissionHelper.CanModifyData))
                            return;
                        CurrentPageViewModel = new PlaceCreateViewModel(_model.LoginWrapper);
                        break;
                    case "PlaceEditViewModel":
                        if (!await Authenticate(PermissionHelper.CanModifyData))
                            return;
                        CurrentPageViewModel = new PlaceEditViewModel(_model.LoginWrapper);
                        break;
                    case "PlaceDeleteViewModel":
                        if (!await Authenticate(PermissionHelper.CanModifyData))
                            return;
                        CurrentPageViewModel = new PlaceDeleteViewModel(_model.LoginWrapper);
                        break;
                    case "SubjectViewTableViewModel":
                        CurrentPageViewModel = new SubjectViewTableViewModel(_model.LoginWrapper);
                        break;
                    case "SubjectCreateViewModel":
                        if (!await Authenticate(PermissionHelper.CanModifyData))
                            return;
                        CurrentPageViewModel = new SubjectCreateViewModel(_model.LoginWrapper);
                        break;
                    case "SubjectDeleteViewModel":
                        if (!await Authenticate(PermissionHelper.CanModifyData))
                            return;
                        CurrentPageViewModel = new SubjectEditViewModel(_model.LoginWrapper);
                        break;
                    case "SubjectEditViewModel":
                        if (!await Authenticate(PermissionHelper.CanModifyData))
                            return;
                        CurrentPageViewModel = new SubjectDeleteViewModel(_model.LoginWrapper);
                        break;
                    case "CurrentProfileViewModel":
                        CurrentPageViewModel = new CurrentProfileViewModel(_model.LoginWrapper);
                        break;

                    case "FacultyEditViewModel":
                        if (!await Authenticate(PermissionHelper.CanModifyData))
                            return;
                        CurrentPageViewModel = new FacultyEditViewModel(_model.LoginWrapper);
                        break;


                }
            }
        }

        private async Task<bool> Authenticate(params int[] requiredPermissions)
        {
            return _model?.LoginWrapper != null && await _model.LoginWrapper.Authenticate();
        }
        #endregion
    }
}
