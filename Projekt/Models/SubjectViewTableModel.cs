using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class SubjectViewTableModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => String.Empty;
        public string? DefaultQuery => String.Empty;
        Dictionary<string, object>? ITable.DefaultParameters => null;

    }
}
