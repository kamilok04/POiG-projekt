using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class ExtendedStudentModel
    {
        private int _studentID;
        private string _name;
        private string _surname;
        private string? _facultyName;
        private string? _majorName;
        private int? _semester;

        public int StudentID
        {
            get => _studentID;
            set => _studentID = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Surname
        {
            get => _surname;
            set => _surname = value;
        }

        public string? FacultyName
        {
            get => _facultyName;
            set => _facultyName = value;
        }

        public string? MajorName
        {
            get => _majorName;
            set => _majorName = value;
        }

        public int? Semester
        {
            get => _semester;
            set => _semester = value;
        }

        public ExtendedStudentModel(Dictionary<string, object> data)
        {
            _name = (string)data["Imię"];
            _surname = (string)data["Nazwisko"];
            _studentID = (int)data["Nr indeksu"];
            _facultyName = (string?)data["Wydział"];
            _majorName = (string?)data["Kierunek"];
            _semester = (int?)data["Semestr"];
        }
    }
}
