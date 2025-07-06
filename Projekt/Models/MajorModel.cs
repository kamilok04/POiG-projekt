using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class MajorModel : ObservableObject, IEquatable<MajorModel>
    {
        public bool Equals(MajorModel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as MajorModel);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private int _id;
        private int _nameId;
        private string? _name;
        private string _facultyId;
        private FacultyModel? _faculty;
        public  LoginWrapper? LoginWrapper { get; set; }

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

        public MajorModel(Dictionary<string, object> data, LoginWrapper wrapper )
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

        
        private void GetFacultyAsync_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NotifyTaskCompletion<FacultyModel>.IsSuccessfullyCompleted) && GetFacultyAsync.IsSuccessfullyCompleted)
            {
                Faculty = GetFacultyAsync.Result;
            }
        }
        private void GetNameAsync_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NotifyTaskCompletion<string>.IsSuccessfullyCompleted) && GetNameAsync.IsSuccessfullyCompleted)
            {
                Name = GetNameAsync.Result;
            }
        }

        public NotifyTaskCompletion<string> GetNameAsync { get; private set; }
        public NotifyTaskCompletion<FacultyModel> GetFacultyAsync { get; private set; }

        private async Task<string> GetName()
        {
            if (LoginWrapper == null)
            {
                throw new InvalidOperationException("LoginWrapper cannot be null when retrieving the Name.");
            }
            var majorDataModel = await RetrieveService.GetAsync<MajorDataModel>(LoginWrapper, Id);
            if (majorDataModel == null)
            {
                throw new InvalidOperationException($"No MajorDataModel found for Id {Id}.");
            }
            return majorDataModel.Name;
        }

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

        public override string ToString()
        {
            return $"{Name} [{FacultyId}]";
        }
    }
}
