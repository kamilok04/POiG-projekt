using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.Models
{
    public class SubjectDeleteModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => String.Empty;
        public int _id;

        public string? DefaultQuery => "CALL DeleteSubject(@id)";

        public Dictionary<string, object>? DefaultParameters => new() { {"@id", _id } };
    }
}
