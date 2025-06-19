using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class SubjectCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => "subjects"; 
        public string? DefaultQuery => String.Empty; // No default query for creation
        public Dictionary<string, object>? DefaultParameters => null; // No default parameters for creation
    }
}
