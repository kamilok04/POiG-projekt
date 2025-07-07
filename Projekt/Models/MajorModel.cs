using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    /// <summary>
    /// Reprezentuje model kierunku studiów, zawierający informacje o nazwie, wydziale oraz powiązaniach z innymi modelami.
    /// </summary>
    public class MajorModel : ObservableObject, IEquatable<MajorModel>
    {
        /// <summary>
        /// Sprawdza równość z innym obiektem MajorModel.
        /// </summary>
        /// <param name="other">Obiekt MajorModel do porównania.</param>
        /// <returns>True, jeśli obiekty są równe; w przeciwnym razie false.</returns>
        public bool Equals(MajorModel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        /// <summary>
        /// Sprawdza równość z innym obiektem.
        /// </summary>
        /// <param name="obj">Obiekt do porównania.</param>
        /// <returns>True, jeśli obiekty są równe; w przeciwnym razie false.</returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as MajorModel);
        }

        /// <summary>
        /// Zwraca kod skrótu dla obiektu.
        /// </summary>
        /// <returns>Kod skrótu.</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private int _id;
        private int _nameId;
        private string? _name;
        private string _facultyId;
        private FacultyModel? _faculty;

        /// <summary>
        /// Wrapper logowania powiązany z modelem.
        /// </summary>
        public LoginWrapper? LoginWrapper { get; set; }

        /// <summary>
        /// Identyfikator kierunku.
        /// </summary>
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        /// <summary>
        /// Identyfikator nazwy kierunku.
        /// </summary>
        public int NameId
        {
            get => _nameId;
            set
            {
                if (_nameId != value)
                {
                    _nameId = value;
                    OnPropertyChanged(nameof(NameId));
                }
            }
        }

        /// <summary>
        /// Nazwa kierunku.
        /// </summary>
        public string? Name
        {
            get => _name ?? "Czekaj...";
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Identyfikator wydziału.
        /// </summary>
        public string FacultyId
        {
            get => _facultyId;
            set
            {
                if (_facultyId != value)
                {
                    _facultyId = value;
                    OnPropertyChanged(nameof(FacultyId));
                }
            }
        }

        /// <summary>
        /// Model wydziału powiązany z kierunkiem.
        /// </summary>
        public FacultyModel? Faculty
        {
            get => _faculty ?? new();
            set
            {
                if (_faculty != value)
                {
                    _faculty = value;
                    OnPropertyChanged(nameof(Faculty));
                }
            }
        }

        /// <summary>
        /// Tworzy nowy obiekt MajorModel na podstawie słownika danych i wrappera logowania.
        /// </summary>
        /// <param name="data">Słownik z danymi kierunku.</param>
        /// <param name="wrapper">Obiekt LoginWrapper.</param>
        public MajorModel(Dictionary<string, object> data, LoginWrapper wrapper)
        {
            _id = (int)data["id"];
            _nameId = (int)data["id_danych_kierunku"];
            _facultyId = (string)data["id_wydzialu"];
            LoginWrapper = wrapper;

            GetNameAsync = new NotifyTaskCompletion<string>(GetName());
            GetFacultyAsync = new NotifyTaskCompletion<FacultyModel>(GetFaculty());
            GetNameAsync.PropertyChanged += GetNameAsync_PropertyChanged;
            GetFacultyAsync.PropertyChanged += GetFacultyAsync_PropertyChanged;
        }

        /// <summary>
        /// Obsługuje zdarzenie zmiany właściwości dla asynchronicznego pobierania wydziału.
        /// </summary>
        private void GetFacultyAsync_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NotifyTaskCompletion<FacultyModel>.IsSuccessfullyCompleted) && GetFacultyAsync.IsSuccessfullyCompleted)
            {
                Faculty = GetFacultyAsync.Result;
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie zmiany właściwości dla asynchronicznego pobierania nazwy kierunku.
        /// </summary>
        private void GetNameAsync_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NotifyTaskCompletion<string>.IsSuccessfullyCompleted) && GetNameAsync.IsSuccessfullyCompleted)
            {
                Name = GetNameAsync.Result;
            }
        }

        /// <summary>
        /// Zwraca asynchronicznie nazwę kierunku.
        /// </summary>
        public NotifyTaskCompletion<string> GetNameAsync { get; private set; }

        /// <summary>
        /// Zwraca asynchronicznie model wydziału.
        /// </summary>
        public NotifyTaskCompletion<FacultyModel> GetFacultyAsync { get; private set; }

        /// <summary>
        /// Asynchronicznie pobiera nazwę kierunku z bazy danych.
        /// </summary>
        /// <returns>Nazwa kierunku.</returns>
        private async Task<string> GetName()
        {
            if (LoginWrapper == null)
            {
                throw new InvalidOperationException("LoginWrapper cannot be null when retrieving the Name.");
            }
            var majorDataModel = await RetrieveService.GetAsync<MajorDataModel>(LoginWrapper, Id) ?? throw new InvalidOperationException($"No MajorDataModel found for Id {Id}.");
            return majorDataModel.Name;
        }

        /// <summary>
        /// Asynchronicznie pobiera model wydziału z bazy danych.
        /// </summary>
        /// <returns>Model wydziału.</returns>
        private async Task<FacultyModel> GetFaculty()
        {
            if (LoginWrapper == null)
            {
                throw new InvalidOperationException("LoginWrapper cannot be null when retrieving the Faculty.");
            }
            var facultyModel = await RetrieveService.GetAsync<FacultyModel>(LoginWrapper, FacultyId);
            if (facultyModel == null)
            {
                throw new InvalidOperationException($"No FacultyModel found for Id {FacultyId}.");
            }
            return facultyModel;
        }

        /// <summary>
        /// Zwraca nazwę kierunku i identyfikator wydziału jako tekst.
        /// </summary>
        /// <returns>Nazwa kierunku i identyfikator wydziału.</returns>
        public override string ToString()
        {
            return $"{Name} [{FacultyId}]";
        }
    }
}
