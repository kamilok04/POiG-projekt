using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class UsersCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "UsersCreate";

        private string? _name;
        private string? _email;
        private string? _password;
        private string? _surname;
        private string? _currentRole;
        private UsersCreateModel _usersCreateModel;
        #region Fields

        public string? Name { get => _name; set => _name = value; }
        public string? Email { get => _email; set => _email = value; }
        public string? Password { get => _password; set => _password = value; }
        public string? Surname { get => _surname; set => _surname = value; }
        public string? CurrentRole { get => _currentRole; set => _currentRole = value; }
        public UsersCreateModel UsersCreateModel { get => _usersCreateModel; set => _usersCreateModel = value; }

        public readonly string[] Roles = ["Administrator", "Prowadzący", "Student"];
        #endregion

        #region Public Properties/Commands
        public UsersCreateViewModel(LoginWrapper loginWrapper)
        {
            UsersCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));

        }
        // for designer only
        public UsersCreateViewModel() { }

        #endregion

        #region Private Methods

        #endregion


    }
}
