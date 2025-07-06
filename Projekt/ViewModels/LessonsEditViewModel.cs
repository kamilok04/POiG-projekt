using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ZstdSharp.Unsafe;

namespace Projekt.ViewModels
{
    public class LessonsEditViewModel : ObservableObject, IPageViewModel
    {
        #region Interface Implementations
        string IPageViewModel.Name => nameof(UsersEditViewModel);


        #endregion

        #region Private Fields
        private LessonsEditModel Model { get; init; }
        public LessonsCreateViewModel CurrentLessonViewModel {  get; init; }
        private bool ChangesPending = false;
        private LoginWrapper Wrapper { get; init; }
        private LessonModel? _selectedLesson;
        private bool _isLessonSelected;
       


        public NotifyTaskCompletion<List<LessonModel>?> NotifyLessonList { get; private set; }

        #endregion
        #region Constructors
        public LessonsEditViewModel(LoginWrapper wrapper)
        {
            Wrapper = wrapper;
            Model = new(Wrapper);
            CurrentLessonViewModel = new(Wrapper);
            
            CurrentLessonViewModel.SaveCommand = new RelayCommand(
                async f => await Update());
            NotifyLessonList = new(LoadDataAsync());
            
        }



        #endregion
        #region Public Members / Commands
        public LessonModel? SelectedLesson
        {
            get => _selectedLesson;
            set
            {
                _selectedLesson = value;
                OnPropertyChanged(nameof(SelectedLesson));
                if (SelectedLesson == null) CurrentLessonViewModel.Cancel();
                else CurrentLessonViewModel.LoadLesson(SelectedLesson);
                IsLessonSelected = _selectedLesson != null;
            }
        }

       
        public bool IsLessonSelected
        {
            get => _isLessonSelected;
            set
            {
                _isLessonSelected = value;
                OnPropertyChanged(nameof(IsLessonSelected));
            }
        }


        private async Task<List<LessonModel>?> LoadDataAsync()
        {
            var lessons = await RetrieveService.GetAllAsync<LessonModel>(Wrapper);
            if (lessons == null) return null;
            await CurrentLessonViewModel.LoadAll();
            foreach (var lesson in lessons) await lesson.LoadDataAsync(Wrapper);
            return lessons;


        }


        private async Task Update()
        {
            var m = CurrentLessonViewModel;

            if (m.SelectedDayOfWeek is null)
            {
                CurrentLessonViewModel.ErrorString = "Dzień tygodnia nie został wybrany!";
                return;
            }
            if (m.SelectedSubject is null)
            {

                CurrentLessonViewModel.ErrorString = "Przedmiot nie został wybrany!";
                return;
            }
            if (m.SelectedGroup is null)
            {
                CurrentLessonViewModel.ErrorString = "Grupa nie została wybrana!";
                return;
            }
            if (m.SelectedPlace is null)
            {
                CurrentLessonViewModel.ErrorString = "Nie prowadzimy lekcji na zewnątrz";
                return;
            }
            if (SelectedLesson is null)
            {
                CurrentLessonViewModel.ErrorString = "Nie wybrano lekcji do edycji!";
                return;
            }

            bool success = await Model.UpdateAsync(
                m.SelectedDayOfWeek,
                m.StartTime,
                m.EndTime,
                m.SelectedPlace.Id,
                m.SelectedSubject.Id,
                m.SelectedGroup.GroupId,
                SelectedLesson.Id
            );

            if (success)
            {
                CurrentLessonViewModel.SuccessString = "Dodano pomyślnie!";

                // Reload lessons and reselect the updated lesson
                var lessons = await LoadDataAsync();
                NotifyLessonList = new NotifyTaskCompletion<List<LessonModel>?>(Task.FromResult(lessons));
                OnPropertyChanged(nameof(NotifyLessonList));

            }
            else
            {
                CurrentLessonViewModel.ErrorString = "Błąd podczas dodawania!";
            }
        }
    }




        #endregion
    }

