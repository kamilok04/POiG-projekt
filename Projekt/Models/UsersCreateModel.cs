using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class UsersCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => "users";

        public string? DefaultQuery => String.Empty;

        public Dictionary<string, object>? DefaultParameters => null;
    }
}
