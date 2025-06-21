using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.Models
{
    public class PlaceCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => "Places";
        public string? DefaultQuery => String.Empty; // No default query for creation
        public Dictionary<string, object>? DefaultParameters => null;
    }
}
