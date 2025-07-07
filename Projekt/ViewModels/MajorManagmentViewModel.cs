using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    /// <summary>
    /// ViewModel do zarządzania kierunkami studiów oraz prezentacji ich w tabeli.
    /// </summary>
    public class MajorManagmentViewModel : ObservableObject, IPageViewModel, ITable
    {
        /// <summary>
        /// Domyślne zapytanie SQL pobierające kierunki i wydziały.
        /// </summary>
        string ITable.DefaultQuery => "SELECT dk.nazwa AS kierunek, w.nazwa AS wydział FROM kierunek k JOIN dane_kierunku dk ON k.id_danych_kierunku = dk.id JOIN wydzial w ON k.id_wydzialu = w.nazwa_krotka;";

        /// <summary>
        /// Nazwa tabeli w bazie danych.
        /// </summary>
        string ITable.TableName => "kierunek";

        /// <summary>
        /// Domyślne parametry do zapytania SQL.
        /// </summary>
        Dictionary<string, object>? ITable.DefaultParameters => null;

        /// <summary>
        /// Nazwa widoku strony.
        /// </summary>
        string IPageViewModel.Name => nameof(MajorManagmentViewModel);

        private UsersEditModel Model { get; init; }

        private DataTable? _data;

        /// <summary>
        /// Dane tabeli wyświetlane w widoku.
        /// </summary>
        public DataTable? TableData
        {
            get => _data;
            private set
            {
                _data = value;
                OnPropertyChanged(nameof(TableData));
            }
        }

        /// <summary>
        /// Konstruktor inicjalizujący ViewModel z wrapperem logowania.
        /// </summary>
        /// <param name="loginWrapper">Obiekt logowania.</param>
        public MajorManagmentViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Konstruktor domyślny (do projektanta).
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public MajorManagmentViewModel() { }
        /// <summary>
        /// Pobiera dane do tabeli asynchronicznie.
        /// </summary>
        private async Task GetDataAsync()
        {
            var query = ((ITable)this).DefaultQuery;
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new InvalidOperationException("DefaultQuery cannot be null or empty.");
            }

            if (Model?.LoginWrapper?.DBHandler == null)
            {
                throw new InvalidOperationException("DatabaseHandler is not initialized.");
            }

            TableData = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(query);
        }
    }
}
