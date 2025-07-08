using Projekt.Miscellaneous;
using Projekt.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public record PermissionLoading(NotifyTaskCompletion<bool> complete);
    public class MainMenuViewModel : ConditionalContentControlViewModel, IPageViewModel
    {
        #region Fields
        string IPageViewModel.Name => "MainMenu";
        private MainMenuModel? _model { get; init; }

        private ICommand? _changePageView;

        public LoginWrapper Wrapper { get; set; }
        


        public MainMenuViewModel(LoginWrapper loginWrapper)
        {
            Wrapper = loginWrapper;
            _model = new(Wrapper);
            CurrentPageViewModel = new CurrentProfileViewModel(Wrapper);

          
        }
  

       
        #endregion

        #region Public Properties/Commands

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

        public PermissionLoading this[string index]
        {
            get=> new(new(CheckPageModelPermissions(index)));
        }

        #endregion

        #region Private Helpers
        private void Logout()
        {
            Wrapper.Logout();

        }

       
        

        private async Task<bool> CheckPageModelPermissions(string name)
        {

            
            return name switch
            {
                
                "UsersCreateViewModel" => await Authenticate(PermissionHelper.CanManageUsers),
                "UsersEditViewModel" => await Authenticate(PermissionHelper.CanManageUsers),
                "UsersDeleteViewModel" => await Authenticate(PermissionHelper.CanManageUsers),
                "LessonsViewTableViewModel" => await Authenticate(PermissionHelper.CanSeeOwnSchedule),
                "LessonsCreateViewModel" => await Authenticate(
                                        PermissionHelper.CanSeeOwnSchedule,
                                        PermissionHelper.CanEditOwnSchedule,
                                        PermissionHelper.CanSeeOtherSchedules,
                                        PermissionHelper.CanEditOtherSchedules),
                "LessonsEditViewModel" => await Authenticate(
                                       PermissionHelper.CanSeeOwnSchedule,
                                       PermissionHelper.CanEditOwnSchedule,
                                       PermissionHelper.CanSeeOtherSchedules,
                                       PermissionHelper.CanEditOtherSchedules),
                "LessonsDeleteViewModel" => await Authenticate(
                                       PermissionHelper.CanSeeOwnSchedule,
                                       PermissionHelper.CanEditOwnSchedule,
                                       PermissionHelper.CanSeeOtherSchedules,
                                       PermissionHelper.CanEditOtherSchedules),
                "GroupsViewTableViewModel" => await Authenticate(
                                        PermissionHelper.CanManageGroups,
                                        PermissionHelper.CanManageUsers),
                "GroupCreateViewModel" => await Authenticate(
                                        PermissionHelper.CanManageGroups,
                                        PermissionHelper.CanSeeOtherProfiles),
                "GroupEditViewModel" => await Authenticate(
                                       PermissionHelper.CanManageGroups,
                                       PermissionHelper.CanSeeOtherProfiles),
                "GroupDeleteViewModel" => await Authenticate(
                                       PermissionHelper.CanManageGroups,
                                       PermissionHelper.CanSeeOtherProfiles),
                "GroupSubjectCoordinatorViewModel" => await Authenticate(
                                        PermissionHelper.CanEditOtherSchedules,
                                        PermissionHelper.CanEditOwnSchedule),
                "GroupStudentViewModel" => await Authenticate(
                                        PermissionHelper.CanEditOwnSchedule,
                                        PermissionHelper.CanEditOtherSchedules,
                                        PermissionHelper.CanEditOtherProfiles,
                                        PermissionHelper.CanEditOwnProfile),
                "PlacesViewTableViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "PlaceCreateViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "PlaceEditViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "PlaceDeleteViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "SubjectViewTableViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "SubjectCreateViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "SubjectEditViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "SubjectDeleteViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "CurrentProfileViewModel" => await Authenticate(PermissionHelper.CanSeeOwnProfile),
                "FacultyCreateViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "FacultyEditViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "MajorManagementViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                "DegreeCreateViewModel" => await Authenticate(PermissionHelper.CanModifyData),
                _ => false,
            };
        }

       
        


        private async Task ChangePageView(object? param)
        {
            if (CurrentPageViewModel is TableViewModel table)
            {
                if (!table.ConfirmExit()) return;
            }

            if (param is not string pageViewModelName || Wrapper == null) return;
            if (!await CheckPageModelPermissions(pageViewModelName)) return;


         
            Type? viewModelType = Type.GetType($"Projekt.ViewModels.{pageViewModelName}");
            if (viewModelType == null) return;

            CurrentPageViewModel = Activator.CreateInstance(viewModelType, Wrapper) as IPageViewModel;


            // przypadki szczególne - gdzie samo zbudowanie ViewModela nie wystarczy
            // TODO: pozbyć się ich z użyciem NotifyTaskCompletion
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
            return Wrapper != null && await Wrapper.Authenticate(requiredPermissions);
        }
        #endregion
         
    }


}
