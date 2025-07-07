using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    /// <summary>
    /// Model studenta dziedziczący po UserModel.
    /// </summary>
    public class StudentModel(Dictionary<string, object> data) : UserModel(data)
    {
        /// <summary>
        /// Identyfikator studenta (numer indeksu).
        /// </summary>
        public int? _studentId = (int?)data["Nr indeksu"];

        /// <summary>
        /// Zwraca reprezentację tekstową studenta.
        /// </summary>
        /// <returns>Imię, nazwisko, login i numer indeksu studenta.</returns>
        public override string ToString()
        {
            return $"{_name} {_surname} ({_login}), nr indeksu: {_studentID}";
        }
    }
}
