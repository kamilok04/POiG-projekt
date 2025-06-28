using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public interface ITable
    {
        public string TableName { get; }

        public string? DefaultQuery { get;  }

        public Dictionary<string, object>? DefaultParameters { get; }

       //  public ICommand 

    }
}
