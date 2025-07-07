using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    /// <summary>
    /// Reprezentuje rozszerzony model studenta z dodatkowymi informacjami o wydziale, kierunku i semestrze.
    /// </summary>
    public class ExtendedStudentModel
    {
        private int _studentID;
        private string _name;
        private string _surname;
        private string? _facultyName;
        private string? _majorName;
        private int? _semester;

        /// <summary>
        /// Identyfikator studenta.
        /// </summary>
        public int StudentID
        {
            get => _studentID;
            set => _studentID = value;
        }

        /// <summary>
        /// Imię studenta.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Nazwisko studenta.
        /// </summary>
        public string Surname
        {
            get => _surname;
            set => _surname = value;
        }

        /// <summary>
        /// Nazwa wydziału studenta.
        /// </summary>
        public string? FacultyName
        {
            get => _facultyName;
            set => _facultyName = value;
        }

        /// <summary>
        /// Nazwa kierunku studenta.
        /// </summary>
        public string? MajorName
        {
            get => _majorName;
            set => _majorName = value;
        }

        /// <summary>
        /// Semestr studenta.
        /// </summary>
        public int? Semester
        {
            get => _semester;
            set => _semester = value;
        }

        /// <summary>
        /// Tworzy nową instancję ExtendedStudentModel na podstawie słownika danych.
        /// </summary>
        /// <param name="data">Słownik z danymi studenta.</param>
        public ExtendedStudentModel(Dictionary<string, object> data)
        {
            _name = (string)data["Imię"];
            _surname = (string)data["Nazwisko"];
            _studentID = (int)data["Nr indeksu"];
            _facultyName = (string?)data["Wydział"];
            _majorName = (string?)data["Kierunek"];
            _semester = (Int16?)data["Semestr"];
        }
    }
}
