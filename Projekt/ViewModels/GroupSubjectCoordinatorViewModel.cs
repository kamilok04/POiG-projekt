using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class GroupSubjectCoordinatorViewModel(LoginWrapper wrapper) : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(GroupSubjectCoordinatorViewModel);
    }
}
