using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public interface ITable
    {
        public string TableName { get; }

        public string? DefaultQuery { get;  }
        public Dictionary<string, object>? DefaultParameters { get; }



    }
}
