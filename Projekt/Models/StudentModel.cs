using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class StudentModel(Dictionary<string,object> data) : UserModel(data)
    {
        public int? _studentId = (int?)data["Nr indeksu"];

        public override string ToString()
        {
            return $"{_name} {_surname} ({_login}), nr indeksu: {_studentID}";
        }
    }
}
